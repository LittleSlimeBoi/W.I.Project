using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance;

    private int spaceLimit = 16;
    private int gridSizeX = 5, gridSizeY = 5;
    public Room[] roomPrefabs;
    private Room[,] dungeon;
    private List<Room> endRooms = new List<Room>();
    private List<Vector2Int> occupiedSpaces = new List<Vector2Int>();
    private List<Vector2Int> unavailableSpaces = new List<Vector2Int>();

    public string enviromentName;
    List<AssetReferenceGrid> backgrounds;
    List<AssetReferenceInteriorTemplate> roomTemplates;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CreateDungeon();
    }

    public void SetSpaceLimit(int level = 1)
    {
        spaceLimit = Mathf.RoundToInt(Random.Range(1,3) + level * 2.6f);
    }
    public void CreateDungeon()
    {
        // Setup
        dungeon = new Room[gridSizeX * 2 + 1, gridSizeY * 2 + 1];
        dungeon[gridSizeX, gridSizeY] = Instantiate(roomPrefabs[0], transform);
        dungeon[gridSizeX, gridSizeY].roomType = Room.RoomType.Starting;
        occupiedSpaces.Add(Vector2Int.zero);
        unavailableSpaces.Add(Vector2Int.zero);

        do
        {
            Room.RoomSize randomSize = GetRandomRoomSize();
            Vector2Int pos = GetRandomPostion(randomSize);
            if (pos == Vector2Int.zero) continue;
            PlaceRoomIntoScene(randomSize, pos.x, pos.y);
        }while(occupiedSpaces.Count != spaceLimit);

    }
    private void PlaceRoomIntoScene(Room.RoomSize size, int x, int y)
    {
        Room newRoom = Instantiate(roomPrefabs[(int)size], transform);
        switch (size)
        {
            case Room.RoomSize.Medium:
                dungeon[x + gridSizeX, y + gridSizeY] = newRoom;
                dungeon[x + gridSizeX, y + gridSizeY].transform.position = new Vector3(x * Room.baseWidth, y * Room.baseHeight, 0);
                occupiedSpaces.Add(new Vector2Int(x, y));
                unavailableSpaces.Add(new Vector2Int(x, y));
                break;
            case Room.RoomSize.SmallHorizontal:
                dungeon[x + gridSizeX, y + gridSizeY] = newRoom;
                dungeon[x + gridSizeX, y + gridSizeY].transform.position = new Vector3(x * Room.baseWidth, y * Room.baseHeight, 0);
                occupiedSpaces.Add(new Vector2Int(x, y));
                unavailableSpaces.Add(new Vector2Int(x, y));
                unavailableSpaces.Add(new Vector2Int(x, y - 1));
                unavailableSpaces.Add(new Vector2Int(x, y + 1));
                break;
            case Room.RoomSize.SmallVertical:
                dungeon[x + gridSizeX, y + gridSizeY] = newRoom;
                dungeon[x + gridSizeX, y + gridSizeY].transform.position = new Vector3(x * Room.baseWidth, y * Room.baseHeight, 0);
                occupiedSpaces.Add(new Vector2Int(x, y));
                unavailableSpaces.Add(new Vector2Int(x, y));
                unavailableSpaces.Add(new Vector2Int(x - 1, y));
                unavailableSpaces.Add(new Vector2Int(x + 1, y));
                break;
            case Room.RoomSize.BigHorizontal:
                dungeon[x + gridSizeX, y + gridSizeY] = newRoom; dungeon[x + gridSizeX + 1, y + gridSizeY] = newRoom;
                dungeon[x + gridSizeX, y + gridSizeY].transform.position = new Vector3((x + 0.5f) * Room.baseWidth, y * Room.baseHeight, 0);
                occupiedSpaces.Add(new Vector2Int(x, y));
                occupiedSpaces.Add(new Vector2Int(x + 1, y));
                unavailableSpaces.Add(new Vector2Int(x, y));
                unavailableSpaces.Add(new Vector2Int(x + 1, y));
                break;
            case Room.RoomSize.BigVertical:
                dungeon[x + gridSizeX, y + gridSizeY] = newRoom; dungeon[x + gridSizeX, y + gridSizeY + 1] = newRoom;
                dungeon[x + gridSizeX, y + gridSizeY].transform.position = new Vector3(x * Room.baseWidth, (y + 0.5f) * Room.baseHeight, 0);
                occupiedSpaces.Add(new Vector2Int(x, y));
                occupiedSpaces.Add(new Vector2Int(x, y + 1));
                unavailableSpaces.Add(new Vector2Int(x, y));
                unavailableSpaces.Add(new Vector2Int(x, y + 1));
                break;
            case Room.RoomSize.ExtraBig:
                dungeon[x + gridSizeX, y + gridSizeY] = newRoom; dungeon[x + gridSizeX + 1, y + gridSizeY] = newRoom; dungeon[x + gridSizeX, y + gridSizeY + 1] = newRoom; dungeon[x + gridSizeX + 1, y + gridSizeY + 1] = newRoom;
                dungeon[x + gridSizeX, y + gridSizeY].transform.position = new Vector3((x + 0.5f) * Room.baseWidth, (y + 0.5f) * Room.baseHeight, 0);
                occupiedSpaces.Add(new Vector2Int(x, y));
                occupiedSpaces.Add(new Vector2Int(x + 1, y));
                occupiedSpaces.Add(new Vector2Int(x, y + 1));
                occupiedSpaces.Add(new Vector2Int(x + 1, y + 1));
                unavailableSpaces.Add(new Vector2Int(x, y));
                unavailableSpaces.Add(new Vector2Int(x + 1, y));
                unavailableSpaces.Add(new Vector2Int(x, y + 1));
                unavailableSpaces.Add(new Vector2Int(x + 1, y + 1));
                break;
        }
    }
    private Vector2Int GetRandomPostion(Room.RoomSize roomSize)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        int safeblock = 50;
        while (possiblePositions.Count == 0 && safeblock > 0)
        {
            int index = Random.Range(0, occupiedSpaces.Count);
            int x = occupiedSpaces[index].x;
            int y = occupiedSpaces[index].y;
            switch (roomSize)
            {
                case Room.RoomSize.BigVertical:
                    if (IsValidRoom(roomSize, x - 1, y)) possiblePositions.Add(new Vector2Int(x - 1, y));
                    if (IsValidRoom(roomSize, x, y - 2)) possiblePositions.Add(new Vector2Int(x, y - 2));
                    if (IsValidRoom(roomSize, x + 1, y)) possiblePositions.Add(new Vector2Int(x + 1, y));
                    if (IsValidRoom(roomSize, x, y + 1)) possiblePositions.Add(new Vector2Int(x, y + 1));
                    if (IsValidRoom(roomSize, x - 1, y - 1)) possiblePositions.Add(new Vector2Int(x - 1, y - 1));
                    if (IsValidRoom(roomSize, x + 1, y - 1)) possiblePositions.Add(new Vector2Int(x + 1, y - 1));
                    break;
                case Room.RoomSize.BigHorizontal:
                    if (IsValidRoom(roomSize, x - 2, y)) possiblePositions.Add(new Vector2Int(x - 2, y));
                    if (IsValidRoom(roomSize, x, y - 1)) possiblePositions.Add(new Vector2Int(x, y - 1));
                    if (IsValidRoom(roomSize, x + 1, y)) possiblePositions.Add(new Vector2Int(x + 1, y));
                    if (IsValidRoom(roomSize, x, y + 1)) possiblePositions.Add(new Vector2Int(x, y + 1));
                    if (IsValidRoom(roomSize, x - 1, y - 1)) possiblePositions.Add(new Vector2Int(x - 1, y - 1));
                    if (IsValidRoom(roomSize, x - 1, y + 1)) possiblePositions.Add(new Vector2Int(x - 1, y + 1));
                    break;
                case Room.RoomSize.ExtraBig:
                    if (IsValidRoom(roomSize, x + 1, y)) possiblePositions.Add(new Vector2Int(x + 1, y));
                    if (IsValidRoom(roomSize, x, y + 1)) possiblePositions.Add(new Vector2Int(x, y + 1));
                    if (IsValidRoom(roomSize, x - 2, y)) possiblePositions.Add(new Vector2Int(x - 2, y));
                    if (IsValidRoom(roomSize, x, y - 2)) possiblePositions.Add(new Vector2Int(x, y - 2));
                    if (IsValidRoom(roomSize, x + 1, y - 1)) possiblePositions.Add(new Vector2Int(x + 1, y - 1));
                    if (IsValidRoom(roomSize, x - 1, y + 1)) possiblePositions.Add(new Vector2Int(x - 1, y + 1));
                    if (IsValidRoom(roomSize, x - 2, y - 1)) possiblePositions.Add(new Vector2Int(x - 2, y - 1));
                    if (IsValidRoom(roomSize, x - 1, y - 2)) possiblePositions.Add(new Vector2Int(x - 1, y - 2));
                    break;
                default:
                    if (IsValidRoom(roomSize, x - 1, y)) possiblePositions.Add(new Vector2Int(x - 1, y));
                    if (IsValidRoom(roomSize, x, y - 1)) possiblePositions.Add(new Vector2Int(x, y - 1));
                    if (IsValidRoom(roomSize, x + 1, y)) possiblePositions.Add(new Vector2Int(x + 1, y));
                    if (IsValidRoom(roomSize, x, y + 1)) possiblePositions.Add(new Vector2Int(x, y + 1));
                    break;
            }
            safeblock--;
        }
        if (possiblePositions.Count == 0) { return Vector2Int.zero; }
        return possiblePositions[Random.Range(0, possiblePositions.Count)];
    }
    private Room.RoomSize GetRandomRoomSize()
    {
        int random = Random.Range(0, 71);
        int avaiableSpace = spaceLimit - occupiedSpaces.Count;
        if (avaiableSpace > 8){
            switch (random)
            {
                case int n when (n >= 30 && n <= 49): return Room.RoomSize.SmallHorizontal;
                case int n when (n >= 50 && n <= 69): return Room.RoomSize.SmallVertical;
                case int n when (n >= 70 && n <= 79): return Room.RoomSize.BigHorizontal;
                case int n when (n >= 80 && n <= 89): return Room.RoomSize.BigVertical;
                case int n when (n >= 90 && n <= 99): return Room.RoomSize.ExtraBig;
                default: return Room.RoomSize.Medium;
            }
        } else
        return  Room.RoomSize.Medium;
    }
    private bool IsValidRoom(Room.RoomSize roomSize, int x, int y)
    {
        return (CheckAvailableSpace(roomSize, x, y) && CheckGridBorder(roomSize, x, y));
    }
    private bool CheckAvailableSpace(Room.RoomSize roomSize, int x, int y)
    {
        switch (roomSize)
        {
            case Room.RoomSize.BigVertical:     return CanRoomBeHere(x, y, 1, 2);
            case Room.RoomSize.BigHorizontal:   return CanRoomBeHere(x, y, 2, 1);
            case Room.RoomSize.ExtraBig:        return CanRoomBeHere(x, y, 2, 2);
            case Room.RoomSize.SmallHorizontal: return CanRoomBeHere(x, y - 1, 1, 3);
            case Room.RoomSize.SmallVertical:   return CanRoomBeHere(x - 1, y, 3, 1);
            default:                            return CanRoomBeHere(x, y);
        }
    }
    private bool CanRoomBeHere(int x, int y, int width = 1, int height = 1)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (unavailableSpaces.Contains(new Vector2Int(x + j, y + i))) return false;
            }
        }
        return true;
    }
    private bool CheckGridBorder(Room.RoomSize roomSize, int x, int y)
    {
        switch (roomSize)
        {
            case Room.RoomSize.BigVertical:     return IsWithInBorderHere(x, y, 1, 2);
            case Room.RoomSize.BigHorizontal:   return IsWithInBorderHere(x, y, 2, 1);
            case Room.RoomSize.ExtraBig:        return IsWithInBorderHere(x, y, 2, 2);
            default:                            return IsWithInBorderHere(x, y);
        }
    }
    private bool IsWithInBorderHere(int x, int y, int width = 1, int height = 1)
    {
        for (int i = 0; i < width ; i++) { if (x + i > gridSizeX || x + i < -gridSizeX) return false; }
        for (int i = 0; i < height; i++) { if (y + i > gridSizeY || y + i < -gridSizeY) return false; }
        return true;
    }
    
    public void ReleaseAssetReferenceObjects()
    {
        foreach (var background in  backgrounds) {
            if (background != null)
            {
                background.ReleaseAsset();
            }
        }
        foreach (var template in roomTemplates) {
            if(template != null)
            {
                template.ReleaseAsset();
            }
        }
    }

}