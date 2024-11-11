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
        Opened, Closed, Walled, Locked, Blocked
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

    protected float entryOffset = 2.75f;
    public static bool isGoingThroughDoor = false;

    private void Start()
    {
        SetChildPosition();
        SetChildCollider();
        SetDoorSprite("Grass Field");
    }

    public void Open()
    {
        doorCollider.SetActive(false);
        doorState = DoorState.Opened;
    }

    public void Close()
    {
        doorCollider.SetActive(true);
        doorState = DoorState.Closed;
    }

    public void Wall()
    {
        doorCollider.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        wall.SetActive(true);
        doorState = DoorState.Walled;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isGoingThroughDoor)
        {
            isGoingThroughDoor = true;
            Vector2 targetPos = GetTargetPosition(other.transform.position, entryOffset);
            UpdatePlayerPosition(other.transform, targetPos);
        }
    }

    protected void SetChildPosition()
    {
        Vector3 setupVector = new Vector3(0.5f, 0.5f, 0);
        doorCollider.transform.position = GetTargetPosition(setupVector, 0);
        wall.transform.position = GetTargetPosition(setupVector, 0);
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

    protected void SetChildCollider()
    {
        doorCollider.GetComponent<BoxCollider2D>().size = GetSize();
        wall.GetComponent<BoxCollider2D>().size = GetSize(2);
    }
    protected Vector2 GetSize(int scale = 1)
    {
        switch (doorDirection)
        {
            case DoorDirection.Left:    return new Vector2(scale, 3);
            case DoorDirection.Right:   return new Vector2(scale, 3);
            case DoorDirection.Top:     return new Vector2(3, scale);
            case DoorDirection.Bottom:  return new Vector2(3, scale);
        }
        return Vector2.zero;
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
        while (elapsed < transitionDuration)
        {
            playerTransform.position = Vector2.Lerp(startPos, targetPos, elapsed / transitionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerTransform.position = targetPos;
        isGoingThroughDoor = false;
    }

    
}
