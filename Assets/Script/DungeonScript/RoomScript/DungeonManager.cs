using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;
    [SerializeField] private Transform dungeonTransform;
    private int spaceLimit = 16;
    public int GridSizeX { get; } = 5;
    public int GridSizeY { get; } = 5;
    public Room[] roomPrefabs;
    public Room[,] Dungeon { get; private set; }
    // private List<Room> endRooms = new List<Room>();
    private List<Vector2Int> occupiedSpaces = new List<Vector2Int>();
    private List<Vector2Int> unavailableSpaces = new List<Vector2Int>();
    private List<Vector3Int> roomsWithDistance = new List<Vector3Int>();
    private int maxDistance = 0;

    public string enviromentName;
    private List<Grid> backgrounds;
    [SerializeField] private List<InteriiorList> roomTemplates;

    public static Room currentRoom;
    public static Vector3 playerLastPos;
    public static Vector2 mainCamLastMinPos;
    public static Vector2 mainCamLastMaxPos;
    public static float minimapCamLastX;
    public static float minimapCamLastY;

    public Room endPlaceholder;

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
        NewCurrentRoom(Dungeon[GridSizeX, GridSizeY]);
        playerLastPos = new(0.5f, 0.5f, 0);
        mainCamLastMinPos = new(0.5f, 0.5f);
        mainCamLastMaxPos = new(0.5f, 0.5f);
        minimapCamLastX = 0.5f;
        minimapCamLastY = 0.5f;
    }

    public void NewCurrentRoom(Room room)
    {
        OpenCurrentRoom();
        currentRoom.HighlightKnownRoom();
        currentRoom.DeactivateMonster();
        currentRoom = room;
        currentRoom.DiscoverNewRoom();
        currentRoom.HighlightCurrentRoom();
        currentRoom.ActivateMonster();
    }

    public void OnLoadDungeon()
    {
        dungeonTransform.gameObject.SetActive(true);
        MainCamController.Instance.RestorePositionInDungeon();
        MiniMapCam.Instance.RestorePositionInDungeon();
        LoadCurrentRoom();
    }
    public void OnUnloadDungeon()
    {
        if (dungeonTransform != null) dungeonTransform.gameObject.SetActive(false);
    }
    public void SavePositionInDungeon()
    {
        if (MainCamController.Instance != null) MainCamController.Instance.SavePositionInDungeon();
        if (MiniMapCam.Instance != null) MiniMapCam.Instance.SavePositionInDungeon();
    }
    public void LoadCurrentRoom()
    {
        currentRoom.DeactivateMonster();
        currentRoom.OpenAllDoors();
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
    public void RemoveDungeon()
    {
        foreach (Room room in Dungeon)
        {
            if (room != null) Destroy(room.gameObject);
        }
        unavailableSpaces.Clear();
        occupiedSpaces.Clear();
        roomsWithDistance.Clear();
        maxDistance = 0;
    }
    public void CreateDungeon()
    {
        // Setup // Change this later
        LoadBackground();
        Dungeon = new Room[GridSizeX * 2 + 1, GridSizeY * 2 + 1];
        //PlaceRoomIntoScene(Room.RoomSize.Medium, 0, 0);
        Room startRoom = Instantiate(roomPrefabs[0], dungeonTransform);
        AddEndRoomToDungeon(startRoom, 0, 0, 1, 1, 0, 0);
        Dungeon[GridSizeX, GridSizeY].roomType = RoomType.Starting;
        Dungeon[GridSizeX, GridSizeY].name = "Starting Room";
        currentRoom = Dungeon[GridSizeX, GridSizeY];
        currentRoom.EnableMiniMapIcon();

        int safeblock = 100;

        // Room generation
        do
        {
            RoomSize randomSize = GetRandomRoomSize();
            Vector2Int pos = GetRandomPostion(randomSize);
            if (pos == Vector2Int.zero) continue;
            // Set rrom position
            PlaceRoomIntoScene(randomSize, pos.x, pos.y);
            safeblock--;
        } while (occupiedSpaces.Count != spaceLimit && safeblock > 0);

        // Connect all normal rooms in the dungeon & Set distance from it to the starting room
        ConnectAllRooms();

        // Generate special room
        int distance = maxDistance;
        do
        {
            Vector2Int pos = GetRandomPostion(distance);
            distance--;
            if (distance < 0)
            {
                Debug.Log("Too much room. Can't place end room");
                break;
            }
            if (pos == Vector2Int.zero) continue;
            // Change this later
            if (endPlaceholder == null) Debug.Log("Make a end room please!!!");
            else
            {
                Room endRoom = Instantiate(endPlaceholder, dungeonTransform);
                AddEndRoomToDungeon(endRoom, pos.x, pos.y, 1, 1, 0, 0);
                Dungeon[GridSizeX + pos.x, GridSizeY + pos.y].name = "End Room";
                foreach (Vector2Int v in occupiedSpaces)
                {
                    Dungeon[GridSizeX + v.x, GridSizeY + v.y].SetRoomConnection();
                }
            }
            break;
        } while (true);
    }
    private void ConnectAllRooms()
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        int distance = 0;
        int size = 1;
        q.Enqueue(Vector2Int.zero);
        do
        {
            if (size == 0)
            {
                size = q.Count;
                distance++;
            }
            Vector2Int pos = q.Dequeue();
            size--;
            Room room = Dungeon[GridSizeX + pos.x, GridSizeY + pos.y];
            if (!room.Calculated)
            {
                // Set distancer
                room.SetDistance(distance);
                // Set connection
                room.SetRoomConnection();
                roomsWithDistance.Add(new Vector3Int(pos.x, pos.y, distance));
                if (maxDistance < distance) maxDistance = distance;
                foreach (Door door in room.activeDoors)
                {
                    q.Enqueue(door.TargetCoordinate);
                }
            }
        } while (q.Count > 0);
    }
    private void PlaceRoomIntoScene(RoomSize size, int x, int y)
    {
        Room newRoom = Instantiate(roomPrefabs[(int)size], dungeonTransform);
        switch (size)
        {
            case RoomSize.Medium:
                AddRoomToDungeon(newRoom, x, y, 1, 1, 0, 0);
                break;
            case RoomSize.SmallHorizontal:
                AddRoomToDungeon(newRoom, x, y, 1, 1, 0, 0);
                unavailableSpaces.Add(new Vector2Int(x, y - 1));
                unavailableSpaces.Add(new Vector2Int(x, y + 1));
                break;
            case RoomSize.SmallVertical:
                AddRoomToDungeon(newRoom, x, y, 1, 1, 0, 0);
                unavailableSpaces.Add(new Vector2Int(x - 1, y));
                unavailableSpaces.Add(new Vector2Int(x + 1, y));
                break;
            case RoomSize.BigHorizontal:
                AddRoomToDungeon(newRoom, x, y, 2, 1, 0.5f, 0);
                break;
            case RoomSize.BigVertical:
                AddRoomToDungeon(newRoom, x, y, 1, 2, 0, 0.5f);
                break;
            case RoomSize.ExtraBig:
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
            foreach (RoomSize size in Enum.GetValues(typeof(RoomSize)))
            {
                if (a.name.Contains(size.ToString())) priorityA = (int)size;
                if (b.name.Contains(size.ToString())) priorityB = (int)size;
            }
            return priorityA.CompareTo(priorityB);
        });
    }
    // Init room: Give room position and assets
    private void AddRoomToDungeon(Room newRoom, int x, int y, int width, int height, float offsetX, float offsetY)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Dungeon[x + j + GridSizeX, y + i + GridSizeY] = newRoom;
                occupiedSpaces.Add(new Vector2Int(x + j, y + i));
                unavailableSpaces.Add(new Vector2Int(x + j, y + i));
            }
        }
        Dungeon[x + GridSizeX, y + GridSizeY].transform.position = new Vector3((x + offsetX) * Room.BASE_WIDTH, (y + offsetY) * Room.BASE_HEIGHT, 0);
        Dungeon[x + GridSizeX, y + GridSizeY].GridPos = new Vector2Int(x, y);
        Dungeon[x + GridSizeX, y + GridSizeY].name = $"{newRoom.roomSize} Room {x} {y}";
        Dungeon[x + GridSizeX, y + GridSizeY].SetEnviroment(
            enviromentName,
            backgrounds[(int)newRoom.roomSize],
            roomTemplates[(int)newRoom.roomSize].templates[UnityEngine.Random.Range(0, roomTemplates[(int)newRoom.roomSize].templates.Count)]
        );
        Dungeon[x + GridSizeX, y + GridSizeY].SetMiniMapIcon();
    }

    private void AddEndRoomToDungeon(Room newRoom, int x, int y, int width, int height, float offsetX, float offsetY)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Dungeon[x + j + GridSizeX, y + i + GridSizeY] = newRoom;
                occupiedSpaces.Add(new Vector2Int(x + j, y + i));
                unavailableSpaces.Add(new Vector2Int(x + j, y + i));
            }
        }
        Dungeon[x + GridSizeX, y + GridSizeY].transform.position = new Vector3((x + offsetX) * Room.BASE_WIDTH, (y + offsetY) * Room.BASE_HEIGHT, 0);
        Dungeon[x + GridSizeX, y + GridSizeY].GridPos = new Vector2Int(x, y);
        Dungeon[x + GridSizeX, y + GridSizeY].name = $"{newRoom.roomSize} Room {x} {y}";
        Dungeon[x + GridSizeX, y + GridSizeY].SetEnviroment(
            enviromentName,
            backgrounds[(int)newRoom.roomSize],
            roomTemplates[(int)newRoom.roomSize].templates[0]
        );
        Dungeon[x + GridSizeX, y + GridSizeY].SetMiniMapIcon();
    }

    // Helper functions
    private Vector2Int GetRandomPostion(int distance, RoomSize roomSize = RoomSize.Medium)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        int index = roomsWithDistance.FindIndex(v => v.z == distance);
        while (possiblePositions.Count == 0 && index != 1 && index < roomsWithDistance.Count && roomsWithDistance[index].z == distance)
        {
            int x = roomsWithDistance[index].x;
            int y = roomsWithDistance[index].y;
            switch (roomSize)
            {
                case RoomSize.BigVertical:
                    if (IsValidRoom(roomSize, x - 1, y)) possiblePositions.Add(new Vector2Int(x - 1, y));
                    if (IsValidRoom(roomSize, x, y - 2)) possiblePositions.Add(new Vector2Int(x, y - 2));
                    if (IsValidRoom(roomSize, x + 1, y)) possiblePositions.Add(new Vector2Int(x + 1, y));
                    if (IsValidRoom(roomSize, x, y + 1)) possiblePositions.Add(new Vector2Int(x, y + 1));
                    if (IsValidRoom(roomSize, x - 1, y - 1)) possiblePositions.Add(new Vector2Int(x - 1, y - 1));
                    if (IsValidRoom(roomSize, x + 1, y - 1)) possiblePositions.Add(new Vector2Int(x + 1, y - 1));
                    break;
                case RoomSize.BigHorizontal:
                    if (IsValidRoom(roomSize, x - 2, y)) possiblePositions.Add(new Vector2Int(x - 2, y));
                    if (IsValidRoom(roomSize, x, y - 1)) possiblePositions.Add(new Vector2Int(x, y - 1));
                    if (IsValidRoom(roomSize, x + 1, y)) possiblePositions.Add(new Vector2Int(x + 1, y));
                    if (IsValidRoom(roomSize, x, y + 1)) possiblePositions.Add(new Vector2Int(x, y + 1));
                    if (IsValidRoom(roomSize, x - 1, y - 1)) possiblePositions.Add(new Vector2Int(x - 1, y - 1));
                    if (IsValidRoom(roomSize, x - 1, y + 1)) possiblePositions.Add(new Vector2Int(x - 1, y + 1));
                    break;
                case RoomSize.ExtraBig:
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
            index++;
        }
        if (possiblePositions.Count == 0) { return Vector2Int.zero; }
        return possiblePositions[UnityEngine.Random.Range(0, possiblePositions.Count)];
    }
    private Vector2Int GetRandomPostion(RoomSize roomSize)
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
                case RoomSize.BigVertical:
                    if (IsValidRoom(roomSize, x - 1, y)) possiblePositions.Add(new Vector2Int(x - 1, y));
                    if (IsValidRoom(roomSize, x, y - 2)) possiblePositions.Add(new Vector2Int(x, y - 2));
                    if (IsValidRoom(roomSize, x + 1, y)) possiblePositions.Add(new Vector2Int(x + 1, y));
                    if (IsValidRoom(roomSize, x, y + 1)) possiblePositions.Add(new Vector2Int(x, y + 1));
                    if (IsValidRoom(roomSize, x - 1, y - 1)) possiblePositions.Add(new Vector2Int(x - 1, y - 1));
                    if (IsValidRoom(roomSize, x + 1, y - 1)) possiblePositions.Add(new Vector2Int(x + 1, y - 1));
                    break;
                case RoomSize.BigHorizontal:
                    if (IsValidRoom(roomSize, x - 2, y)) possiblePositions.Add(new Vector2Int(x - 2, y));
                    if (IsValidRoom(roomSize, x, y - 1)) possiblePositions.Add(new Vector2Int(x, y - 1));
                    if (IsValidRoom(roomSize, x + 1, y)) possiblePositions.Add(new Vector2Int(x + 1, y));
                    if (IsValidRoom(roomSize, x, y + 1)) possiblePositions.Add(new Vector2Int(x, y + 1));
                    if (IsValidRoom(roomSize, x - 1, y - 1)) possiblePositions.Add(new Vector2Int(x - 1, y - 1));
                    if (IsValidRoom(roomSize, x - 1, y + 1)) possiblePositions.Add(new Vector2Int(x - 1, y + 1));
                    break;
                case RoomSize.ExtraBig:
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
    private RoomSize GetRandomRoomSize()
    {
        int random = UnityEngine.Random.Range(0, 100);
        int avaiableSpace = spaceLimit - occupiedSpaces.Count;
        if (avaiableSpace > 8){
            switch (random)
            {
                case int n when (n >= 30 && n <= 49): return RoomSize.SmallHorizontal;
                case int n when (n >= 50 && n <= 69): return RoomSize.SmallVertical;
                case int n when (n >= 70 && n <= 79): return RoomSize.BigHorizontal;
                case int n when (n >= 80 && n <= 89): return RoomSize.BigVertical;
                case int n when (n >= 90 && n <= 99): return RoomSize.ExtraBig;
                default: return RoomSize.Medium;
            }
        } else
        return  RoomSize.Medium;
    }
    private bool IsValidRoom(RoomSize roomSize, int x, int y)
    {
        return (CheckAvailableSpace(roomSize, x, y) && CheckGridBorder(roomSize, x, y));
    }
    private bool CheckAvailableSpace(RoomSize roomSize, int x, int y)
    {
        switch (roomSize)
        {
            case RoomSize.BigVertical:     return CanRoomBeHere(x, y, 1, 2);
            case RoomSize.BigHorizontal:   return CanRoomBeHere(x, y, 2, 1);
            case RoomSize.ExtraBig:        return CanRoomBeHere(x, y, 2, 2);
            case RoomSize.SmallHorizontal: return CanRoomBeHere(x, y - 1, 1, 3);
            case RoomSize.SmallVertical:   return CanRoomBeHere(x - 1, y, 3, 1);
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
    private bool CheckGridBorder(RoomSize roomSize, int x, int y)
    {
        switch (roomSize)
        {
            case RoomSize.BigVertical:     return IsWithInBorderHere(x, y, 1, 2);
            case RoomSize.BigHorizontal:   return IsWithInBorderHere(x, y, 2, 1);
            case RoomSize.ExtraBig:        return IsWithInBorderHere(x, y, 2, 2);
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

    public void CreateNewDungeon()
    {
        RemoveDungeon();
        CreateDungeon();
    }
}