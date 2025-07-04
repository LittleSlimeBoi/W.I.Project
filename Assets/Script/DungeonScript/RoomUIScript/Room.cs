using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour, IClampCamera
{
    public enum RoomType
    {
        Normal, Starting, Boss, Treasure, Secret
    }
    public enum RoomSize
    {
        Medium = 0,
        SmallHorizontal = 1,
        SmallVertical = 2,
        BigHorizontal = 3,
        BigVertical = 4,
        ExtraBig = 5
    }
    public enum RoomState
    {
        Incomplete, Complete
    }

    public RoomSize roomSize;
    public RoomType roomType;
    public RoomState roomState;
    
    public static readonly int baseWidth = 19;
    public static readonly int baseHeight = 11;

    public Vector2Int GridPos { get; set; }
    public int PosX { get { return GridPos.x; } }
    public int PosY { get { return GridPos.y; } }
    public int Distance { get; private set; } = 999;
    public bool Calculated { get; private set; } = false;

    private string enviromentName;
    private Grid background;
    private InteriorTemplate roomInteriorTemplate;
    public List<Door> activeDoors = new();
    [SerializeField] private List<Door> doors; // index from bottom up, left to right
    [SerializeField] private MiniMapIcon minimapIcon;

    public void SetDistance(int dis)
    {
        Distance = dis;
        Calculated = true;
    }

    public Vector2 GetPositionOnMiniMap()
    {
        return minimapIcon.GetPosition();
    }
    public void HighlightKnownRoom()
    {
        minimapIcon.HighlightKnownRoom();
    }
    public void HighlightCurrentRoom()
    {
        minimapIcon.HighlightCurrentRoom();
    }
    public void DiscoverNewRoom()
    {
        foreach (Door door in activeDoors)
        {
            Room neighbor = door.GetNeighboringRoom();
            neighbor.EnableMiniMapIcon();
        }
    }
    public void EnableMiniMapIcon()
    {
        minimapIcon.ShowUndiscoveredRoom();
    }

    public List<CombatInfo> GetMonsterInfoInRoom()
    {
        return roomInteriorTemplate.monsters;
    }
    public void ActivateMonster()
    {
        if(roomState == RoomState.Incomplete)
        {
            foreach (CombatInfo mon in roomInteriorTemplate.monsters)
            {
                mon.gameObject.SetActive(true);
            }
        }
    }
    public void DeactivateMonster()
    {
        foreach (CombatInfo mon in roomInteriorTemplate.monsters)
        {
            mon.gameObject.SetActive(false);
        }
    }
    public void OpenAllDoors()
    {
        foreach (Door door in activeDoors)
        {
            door.Open();
        }
    }
    public void CloseAllDorrs()
    {
        foreach (Door door in activeDoors)
        {
            door.Close();
        }
    }

    public void SetEnviroment(string enviroment, Grid backgroundPrefab, InteriorTemplate roomTemplatePrefab)
    {
        enviromentName = enviroment;

        // Instantiate and set background local position
        background = Instantiate(backgroundPrefab, transform);
        background.transform.localPosition = GetChildLocalOffset(roomSize);

        // Instatiate interior template and render sprites
        if(roomTemplatePrefab != null )
        {
            roomInteriorTemplate = Instantiate(roomTemplatePrefab, transform);
            roomInteriorTemplate.RenderInterior(enviroment);
        }

        // Set door sprite
        SetDoorEnviroment(enviroment);
    }

    public void SetDoorEnviroment(string enviroment)
    {
        foreach(Door door in doors)
        {
            door.SetDoorSprite(enviroment);
        }
    }
    public void SetRoomConnection()
    {
        switch (roomSize)
        {
            case RoomSize.Medium:
                SetDoorConnection(0, -1, 0);
                SetDoorConnection(-1, 0, 1);
                SetDoorConnection(1, 0, 2);
                SetDoorConnection(0, 1, 3);
                break;
            case RoomSize.SmallHorizontal:
                SetDoorConnection(-1, 0, 0);
                SetDoorConnection(1, 0, 1);
                break;
            case RoomSize.SmallVertical:
                SetDoorConnection(0, -1, 0);
                SetDoorConnection(0, 1, 1);
                break;
            case RoomSize.BigHorizontal:
                SetDoorConnection(0, -1, 0);
                SetDoorConnection(1, -1, 1);
                SetDoorConnection(-1, 0, 2);
                SetDoorConnection(2, 0, 3);
                SetDoorConnection(0, 1, 4);
                SetDoorConnection(1, 1, 5);
                break;
            case RoomSize.BigVertical:
                SetDoorConnection(0, -1, 0);
                SetDoorConnection(-1, 0, 1);
                SetDoorConnection(1, 0, 2);
                SetDoorConnection(-1, 1, 3);
                SetDoorConnection(1, 1, 4);
                SetDoorConnection(0, 2, 5);
                break;
            case RoomSize.ExtraBig:
                SetDoorConnection(0, -1, 0);
                SetDoorConnection(1, -1, 1);
                SetDoorConnection(-1, 0, 2);
                SetDoorConnection(2, 0, 3);
                SetDoorConnection(-1, 1, 4);
                SetDoorConnection(2, 1, 5);
                SetDoorConnection(0, 2, 6);
                SetDoorConnection(1, 2, 7);
                break;
            default: Debug.Log("Unknown eoom size");
                break;
        }
    }
    private void SetDoorConnection(int x, int y, int i)
    {
        if (!activeDoors.Contains(doors[i]))
        {
            if (!IsNeighbourAt(x, y))
            {
                doors[i].Wall();
            }
            else
            {
                doors[i].Open();
                activeDoors.Add(doors[i]);
                doors[i].ClampCamera = DungeonManager.Instance.Dungeon[PosX + DungeonManager.Instance.GridSizeX + x,
                                                                       PosY + DungeonManager.Instance.GridSizeY + y];
                doors[i].TargetCoordinate = new Vector2Int(PosX + x, PosY + y);
            }
        }
    }
    private bool IsNeighbourAt(int offsetX, int offsetY)
    {
        if(PosX + offsetX < -DungeonManager.Instance.GridSizeX || PosY + offsetY < -DungeonManager.Instance.GridSizeY || 
            PosX + offsetX > DungeonManager.Instance.GridSizeX || PosY + offsetY > DungeonManager.Instance.GridSizeY)
            return false;
        if (DungeonManager.Instance.Dungeon[PosX + offsetX + DungeonManager.Instance.GridSizeX,
                                              PosY + offsetY + DungeonManager.Instance.GridSizeY] == null) 
            return false;
        return true;
    }

    private Vector3 GetChildLocalOffset(RoomSize size)
    {
        switch(size)
        {
            case RoomSize.BigHorizontal:    return new Vector3 (0.5f, 0, 0);
            case RoomSize.BigVertical:      return new Vector3 (0, 0.5f, 0);
            case RoomSize.ExtraBig:         return new Vector3 (0.5f, 0.5f, 0);
            default:                        return Vector3.zero;
        }
    }

    public void SetMiniMapIcon()
    {
        minimapIcon.SetSizeAndPosition(roomSize, GridPos);
    }

    public bool IsDifferentRoom(Room other) { return !(this == other); }

    public void SetCameraBound()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        switch (roomSize)
        {
            case RoomSize.BigHorizontal: MainCamController.Instance.SetBounds(x - 9, x + 10, y + 0.5f, y + 0.5f); break;
            case RoomSize.BigVertical: MainCamController.Instance.SetBounds(x + 0.5f, x + 0.5f, y - 5, y + 6); break;
            case RoomSize.ExtraBig: MainCamController.Instance.SetBounds(x - 9, x + 10, y - 5, y + 6); break;
            default: MainCamController.Instance.SetBounds(x + 0.5f, x + 0.5f, y + 0.5f, y + 0.5f); break;
        }
    }
}