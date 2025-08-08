using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorDirection
    {
        Left, Right, Top, Bottom
    }
    public enum DoorState
    {
        Opened, Closed, Walled, Locked, Blocked, Hidden
    }
    public enum DoorType
    {
        None, Boss, Treasure, Secret
    }

    public DoorState doorState;
    public DoorDirection doorDirection;
    public DoorType doorType;
    [SerializeField] protected GameObject doorCollider;
    [SerializeField] protected GameObject wall;
    [SerializeField] protected BoxCollider2D raycastBlocker;

    protected float entryOffset = 2.75f;
    public static bool isGoingThroughDoor = false;

    public Vector2Int TargetCoordinate { get; set; }
    public IClampCamera ClampCamera { private get; set; }

    public Room GetNeighboringRoom()
    {
        return DungeonManager.Instance.Dungeon[DungeonManager.Instance.GridSizeX + TargetCoordinate.x, 
                                               DungeonManager.Instance.GridSizeY + TargetCoordinate.y];
    }

    public void Open()
    {
        doorCollider.SetActive(false);
        raycastBlocker.enabled = true;
        wall.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        doorState = DoorState.Opened;
    }

    public void Close()
    {
        doorCollider.SetActive(true);
        raycastBlocker.enabled = false;
        wall.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        doorState = DoorState.Closed;
    }

    public void Wall()
    {
        doorCollider.SetActive(false);
        wall.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        doorState = DoorState.Walled;
    }
    // Going into a new room
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isGoingThroughDoor)
        {
            isGoingThroughDoor = true;
            Vector2 targetPos = GetTargetPosition(other.transform.position, entryOffset);
            UpdatePlayerPosition(other.transform, targetPos);
            DungeonManager.Instance.NewCurrentRoom(GetNeighboringRoom());
            MiniMapCam.Instance.UpdatePosition();
        }
    }

    public void SetDoorSprite(string enviromentName, DoorType doorType = DoorType.None)
    {
        SetChildSprite(wall, "", "_Wall", enviromentName, doorType);
        SetChildSprite(doorCollider, "_Closed", "_Door", enviromentName, doorType);
        SetChildSprite(gameObject, "_Opened", "_Door", enviromentName, doorType);
    }
    public void SetChildSprite(GameObject child, string _prefix, string _child, string enviromentName, DoorType doorType)
    {
        string enviromentVar = "";
        string path = "";
        switch (doorType)
        {
            case DoorType.None: 
                enviromentVar = enviromentName;
                path = "Sprite/Enviroment Sprite/" + enviromentName + "/" + enviromentVar + _child;
                break;
            case DoorType.Treasure:
                enviromentVar = doorType.ToString() + " Room";
                path = "Sprite/Enviroment Sprite/" + enviromentVar + "/" + enviromentVar + _child;
                break;
            case DoorType.Boss:
                enviromentVar = doorType.ToString() + " Room";
                path = "Sprite/Enviroment Sprite/" + enviromentVar + "/" + enviromentVar + _child;
                break;
            case DoorType.Secret:
                enviromentVar = enviromentName + " " + doorType.ToString();
                path = "Sprite/Enviroment Sprite/" + enviromentName + "/" + enviromentVar + _child;
                break;
        }
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        SelectSprite(child, enviromentVar + _prefix + _child + "_" + doorDirection.ToString(), sprites);
    }
    protected void SelectSprite(GameObject obj, string name, Sprite[] sprites)
    {
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == name)
            {
                obj.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
    }

    protected Vector2 GetTargetPosition(Vector3 targetPos, float offset)
    {
        switch (doorDirection)
        {
            case DoorDirection.Left:     return new Vector2(transform.position.x - offset, targetPos.y);
            case DoorDirection.Right:    return new Vector2(transform.position.x + offset, targetPos.y);
            case DoorDirection.Top:      return new Vector2(targetPos.x, transform.position.y + offset);
            case DoorDirection.Bottom:   return new Vector2(targetPos.x, transform.position.y - offset);
        }
        return Vector2.zero;
    }

    public void UpdatePlayerPosition(Transform playerTransform, Vector2 targetPos, float transitionDuration = 0.3f)
    {
        StartCoroutine(MovePlayerToNextRoom(playerTransform, targetPos, transitionDuration));
    }

    private IEnumerator MovePlayerToNextRoom(Transform playerTransform, Vector2 targetPos, float transitionDuration)
    {
        float elapsed = 0f;
        Vector2 startPos = playerTransform.position;

        Vector3 camStartPos = new(MainCamController.Instance.TransitionX, MainCamController.Instance.TransitionY, Camera.main.transform.position.z);
        ClampCamera.SetCameraBound();
        float camTargetX = (doorDirection == DoorDirection.Left || doorDirection == DoorDirection.Right 
            ? camStartPos.x + (doorDirection == DoorDirection.Left ? -1 : 1) * Room.baseWidth 
            : MainCamController.Instance.TransitionX);
        float camTargetY = (doorDirection == DoorDirection.Top || doorDirection == DoorDirection.Bottom 
            ? camStartPos.y + (doorDirection == DoorDirection.Bottom ? -1 : 1) * Room.baseHeight 
            : MainCamController.Instance.TransitionY);
        Vector3 camTargetPos = new(camTargetX, camTargetY, camStartPos.z);

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            playerTransform.position = Vector2.Lerp(startPos, targetPos, t);
            Camera.main.transform.position = Vector3.Lerp(camStartPos, camTargetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerTransform.position = targetPos;
        isGoingThroughDoor = false;
    }
}
