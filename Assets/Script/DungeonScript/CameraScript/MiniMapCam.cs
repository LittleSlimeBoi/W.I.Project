using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    public static MiniMapCam Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdatePosition()
    {
        Vector2 newPosition = DungeonManager.currentRoom.GetPositionOnMiniMap();
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    public void SavePositionInDungeon()
    {
        DungeonManager.minimapCamLastX = transform.position.x;
        DungeonManager.minimapCamLastY = transform.position.y;
    }
    public void RestorePositionInDungeon()
    {
        transform.position = new Vector3(DungeonManager.minimapCamLastX, DungeonManager.minimapCamLastY, transform.position.z);
    }
}
