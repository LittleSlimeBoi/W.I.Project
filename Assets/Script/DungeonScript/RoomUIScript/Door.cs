using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        left, right, top, bottom
    }

    public DoorType doorType;
    public GameObject doorCollider;
    public GameObject wall;

    protected float entryOffset = 3f;
    protected float colliderOffset = 0.5f;
    public static bool isGoingThroughDoor = false;

    private void Start()
    {
        doorCollider.transform.position = GetTargetPosition(-colliderOffset);
        wall.transform.position = GetTargetPosition(colliderOffset);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isGoingThroughDoor)
        {
            isGoingThroughDoor = true;
            Vector2 targetPos = GetTargetPosition(entryOffset);
            UpdatePlayerPosition(other.transform, targetPos);
        }
    }

    protected Vector2 GetTargetPosition(float offset)
    {
        switch (doorType)
        {
            case DoorType.left:     return new Vector2(transform.position.x - offset, transform.position.y);
            case DoorType.right:    return new Vector2(transform.position.x + offset, transform.position.y);
            case DoorType.top:      return new Vector2(transform.position.x, transform.position.y + offset);
            case DoorType.bottom:   return new Vector2(transform.position.x, transform.position.y - offset);
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
