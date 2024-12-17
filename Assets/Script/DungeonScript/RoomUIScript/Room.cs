using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
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

    public RoomSize roomSize;
    public RoomType roomType;
    
    public static int baseWidth = 19;
    public static int baseHeight = 11;

    [HideInInspector] public Vector2Int startPos;

    private string enviromentName;
    private Grid background;
    private InteriorTemplate roomInteriorTemplate;

    public List<Door> doorPrefabs;      //      3
    private List<Door> doors;           //  1 <= => 2
    private List<Door> activeDoors;     //      0

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
        
        // There's more
    }

    public void SpawnDoor()
    {

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

    private void SetCameraBound()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        switch (roomSize)
        {
            case RoomSize.BigHorizontal: CamController.Instance.SetBounds(x - 9, x + 10, y + 0.5f, y + 0.5f); break;
            case RoomSize.BigVertical:   CamController.Instance.SetBounds(x + 0.5f, x + 0.5f, y - 5, y + 6); break;
            case RoomSize.ExtraBig:      CamController.Instance.SetBounds(x - 9, x + 10, y - 5, y + 6); break;
            default:    CamController.Instance.SetBounds(x + 0.5f, x + 0.5f, y + 0.5f, y + 0.5f); break;
        }
    }

    public bool IsDifferentRoom(Room other) { return !(this == other); }
}
