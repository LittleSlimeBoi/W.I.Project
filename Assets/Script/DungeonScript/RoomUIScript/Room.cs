using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Normal, Starting, Boss, Treasure, Secret
    }
    public enum RoomSize
    {
        Medium = 0,
        SmallVertical = 1,
        SmallHorizontal = 2,
        BigVertical = 3,
        BigHorizontal = 4,
        ExtraBig = 5
    }

    public RoomSize roomSize;
    public RoomType roomType;
    private Vector2Int posstionInGrid;
    private Vector2Int sizeInGrid;
    
    static int baseWidth = 19;
    static int baseHeight = 11;

    private string enviromentName;
    private Grid background;
    private InteriorTemplate roomInteriorTemplate;
    private InteriorSprites interiorSprites;

    public List<Door> doorPrefabs;      //      3
    private List<Door> doors;           //  1 <= => 2
    private List<Door> activeDoors;     //      0

    public void LoadEnviroment(string enviroment, RoomSize size, AssetReferenceGrid backgroundPrefab,
                                AssetReferenceInteriorTemplate roomTemplatePrefab)
    {
        enviromentName = enviroment;
        roomSize = size;

        backgroundPrefab.LoadAssetAsync<Grid>().Completed += BackgroundLoadHandle;
        roomTemplatePrefab.LoadAssetAsync<InteriorTemplate>().Completed += InteriorLoadHandle;
    }

    // Load and set background local position
    private void BackgroundLoadHandle(AsyncOperationHandle<Grid> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            background = Instantiate(handle.Result, transform);
            background.transform.localPosition = GetBackgroundLocalOffset(roomSize);
        }
        else
        {
            Debug.Log("Failed to load background");
        }
    }

    // Load and interior template and sprites 
    private void InteriorLoadHandle(AsyncOperationHandle<InteriorTemplate> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            roomInteriorTemplate = Instantiate(handle.Result, transform);
            interiorSprites = Resources.Load<InteriorSprites>("Sprite/Enviroment Sprite/" + enviromentName + "/" + enviromentName);
            // There's more
        }
        else
        {
            Debug.Log("Failed to load background");
        }
    }

    public void SpawnDoor()
    {

    }

    private Vector3 GetBackgroundLocalOffset(RoomSize size)
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
