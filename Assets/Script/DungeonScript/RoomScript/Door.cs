using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] protected Animator doorCollider;
    [SerializeField] protected SpriteRenderer doorFrame;
    [SerializeField] protected SpriteRenderer wall;
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
        doorCollider.SetBool("Open", true);
        raycastBlocker.enabled = true;
        wall.gameObject.SetActive(false);
        doorFrame.enabled = true;
        doorState = DoorState.Opened;
    }

    public void Close()
    {
        doorCollider.SetBool("Open", false);
        raycastBlocker.enabled = false;
        wall.gameObject.SetActive(false);
        doorFrame.enabled = true;
        doorState = DoorState.Closed;
    }

    public void Wall()
    {
        doorCollider.gameObject.SetActive(false);
        wall.gameObject.SetActive(true);
        doorFrame.enabled = false;
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
        SetWallSprite(enviromentName);
        SetFrameSprite(enviromentName, doorType);
        SetDoorAnimation(enviromentName, doorType);
    }
    public void SetWallSprite(string enviromentName)
    {
        string path = "Stage Recources/" + enviromentName + "/" + enviromentName + "_Wall";
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        string spritename = enviromentName + "_Wall_" + doorDirection.ToString();
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == spritename)
            {
                wall.sprite = sprite;
            }
        }
    }
    public void SetFrameSprite(string enviromentName, DoorType type)
    {
        string path;
        // Insert a switch case here
        path = "Stage Recources/" + enviromentName + "/" + enviromentName + "_Frame";

        doorFrame.sprite = Resources.Load<Sprite>(path);
    }
    public void SetDoorAnimation(string enviromentName, DoorType type)
    {
        string path;
        // Insert a switch case here
        path = "Stage Recources/" + enviromentName;

        // Load animation clips
        AnimationClip switchAnim = Resources.Load<AnimationClip>(path + "/Door Switch");

        AnimatorOverrideController aoc = new AnimatorOverrideController(doorCollider.runtimeAnimatorController);
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        aoc.GetOverrides(overrides);
        for (int i = 0; i < overrides.Count; i++)
        {
            if (overrides[i].Key.name == "Closed" || overrides[i].Key.name == "Open")
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, switchAnim);
                continue;
            }
            // Overide more animation clip here
        }
        aoc.ApplyOverrides(overrides);
        doorCollider.runtimeAnimatorController = aoc;
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
            ? camStartPos.x + (doorDirection == DoorDirection.Left ? -1 : 1) * Room.BASE_WIDTH 
            : MainCamController.Instance.TransitionX);
        float camTargetY = (doorDirection == DoorDirection.Top || doorDirection == DoorDirection.Bottom 
            ? camStartPos.y + (doorDirection == DoorDirection.Bottom ? -1 : 1) * Room.BASE_HEIGHT 
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
