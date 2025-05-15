using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    public static MiniMapCam Instance;
    [SerializeField] private int zoomOut;
    [SerializeField] private int zoomIn;
    private float posX = 0;
    private float posY = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void DefaultZoom()
    {
        if (transform.position.x == 0 && transform.position.y == 0)
        {
            transform.position = new Vector3(posX, posY, -10);
        }
        gameObject.GetComponent<Camera>().orthographicSize = zoomIn;
    }

    public void ExpandZoom()
    {
        if (transform.position.x != 0 && transform.position.y != 0)
        {
            posX = transform.position.x;
            posY = transform.position.y;
        }
        gameObject.GetComponent<Camera>().orthographicSize = zoomOut;
        transform.position = new Vector3 (0, 0, -10);
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
