using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    private int spaceLimit = 16;
    public int GridSizeX { get; } = 5;
    public int GridSizeY { get; } = 5;
    public Room[] roomPrefabs;
    public Room[,] Dungeon { get; private set; }
    private List<Room> endRooms = new List<Room>();
    private List<Vector2Int> occupiedSpaces = new List<Vector2Int>();
    private List<Vector2Int> unavailableSpaces = new List<Vector2Int>();

    public string enviromentName;
    private List<Grid> backgrounds;
    [SerializeField] private List<InteriiorList> roomTemplates;

    public static Room currentRoom;

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
    public void NewCurrentRoom(Room room)
    {
        OpenCurrentRoom();
        // To do: disable monster inside incomplete room
        currentRoom = room;
    }
    public void CloseCurrentRoom()
    {
        currentRoom.CloseAllDorrs();
    }
    public void OpenCurrentRoom()
    {
        currentRoom.OpenAllDoors();
    }

    public void SetSpaceLimit(int level = 1)
    {
        spaceLimit = Mathf.RoundToInt(UnityEngine.Random.Range(1,3) + level * 2.6f);
    }
    public void CreateDungeon()
    {
        // Setup
        LoadBackground();
        Dungeon = new Room[GridSizeX * 2 + 1, GridSizeY * 2 + 1];
        PlaceRoomIntoScene(Room.RoomSize.Medium, 0, 0);
        Dungeon[GridSizeX, GridSizeY].roomType = Room.RoomType.Starting;
        currentRoom = Dungeon[GridSizeX, GridSizeY];

        // Room generation
        do
        {
            Room.RoomSize randomSize = GetRandomRoomSize();
            Vector2Int pos = GetRandomPostion(randomSize);
            if (pos == Vector2Int.zero) continue;
            // Set rrom position
            PlaceRoomIntoScene(randomSize, pos.x, pos.y);
        }while(occupiedSpaces.Count != spaceLimit);


        // Wall the dungeon
        foreach (var room in Dungeon)
        {
            if(room != null)
            {
                room.SetRoomConnection();
            }
        }
    }
    private void PlaceRoomIntoScene(Room.RoomSize size, int x, int y)
    {
        Room newRoom = Instantiate(roomPrefabs[(int)size], transform);
        switch (size)
        {
            case Room.RoomSize.Medium:
                AddRoomToDungeon(newRoom, x, y, 1, 1, 0, 0);
                break;
            case Room.RoomSize.SmallHorizontal:
                AddRoomToDungeon(newRoom, x, y, 1, 1, 0, 0);
                unavailableSpaces.Add(new Vector2Int(x, y - 1));
                unavailableSpaces.Add(new Vector2Int(x, y + 1));
                break;
            case Room.RoomSize.SmallVertical:
                AddRoomToDungeon(newRoom, x, y, 1, 1, 0, 0);
                unavailableSpaces.Add(new Vector2Int(x - 1, y));
                unavailableSpaces.Add(new Vector2Int(x + 1, y));
                break;
            case Room.RoomSize.BigHorizontal:
                AddRoomToDungeon(newRoom, x, y, 2, 1, 0.5f, 0);
                break;
            case Room.RoomSize.BigVertical:
                AddRoomToDungeon(newRoom, x, y, 1, 2, 0, 0.5f);
                break;
            case Room.RoomSize.ExtraBig:
                AddRoomToDungeon(newRoom, x, y, 2, 2, 0.5f, 0.5f);
                break;
        }
    }
    // Asset loading
    private void LoadBackground()
    {
        backgrounds = Resources.LoadAll<Grid>("Loading Prefab/Background/" + enviromentName).ToList();
        backgrounds.Sort((a, b) =>
        {
            int priorityA = 0, priorityB = 0;
            foreach (Room.RoomSize size in Enum.GetValues(typeof(Room.RoomSize)))
            {
                if (a.name.Contains(size.ToString())) priorityA = (int)size;
                if (b.name.Contains(size.ToString())) priorityB = (int)size;
            }
            return priorityA.CompareTo(priorityB);
        });
    }

    // Give room position and assets
    private void AddRoomToDungeon(Room newRoom, int x, int y, int width, int height, float offsetX, float offsetY)
    {
        for (int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                Dungeon[x + j + GridSizeX, y + i + GridSizeY] = newRoom;
                occupiedSpaces.Add(new Vector2Int(x + j, y + i));
                unavailableSpaces.Add(new Vector2Int(x + j, y + i));
            }
        }
        Dungeon[x + GridSizeX, y + GridSizeY].transform.position = new Vector3((x + offsetX) * Room.baseWidth, (y + offsetY) * Room.baseHeight, 0);
        Dungeon[x + GridSizeX, y + GridSizeY].gridPos = new Vector2Int(x, y);
        Dungeon[x + GridSizeX, y + GridSizeY].name = $"{newRoom.roomSize} Room {x} {y}";
        Dungeon[x + GridSizeX, y + GridSizeY].SetEnviroment(
            enviromentName,
            backgrounds[(int)newRoom.roomSize],
            roomTemplates[(int)newRoom.roomSize].templates[UnityEngine.Random.Range(0, roomTemplates[(int)newRoom.roomSize].templates.Count)]
        );
    }

    // Helper functions
    private Vector2Int GetRandomPostion(Room.RoomSize roomSize)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        int safeblock = 50;
        while (possiblePositions.Count == 0 && safeblock > 0)
        {
            int index = UnityEngine.Random.Range(0, occupiedSpaces.Count);
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
        return possiblePositions[UnityEngine.Random.Range(0, possiblePositions.Count)];
    }
    private Room.RoomSize GetRandomRoomSize()
    {
        int random = UnityEngine.Random.Range(0, 100);
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
        for (int i = 0; i < width ; i++) { if (x + i > GridSizeX || x + i < -GridSizeX) return false; }
        for (int i = 0; i < height; i++) { if (y + i > GridSizeY || y + i < -GridSizeY) return false; }
        return true;
    }
    
    public void UnloadAsset()
    {
        backgrounds.Clear();
        Resources.UnloadUnusedAssets();
    }
}