using UnityEngine;

public class MiniMapIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color currentRoom;
    [SerializeField] private Color knownRoom;
    [SerializeField] private Color unknownRoom;

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void ShowUndiscoveredRoom()
    {
        if (!spriteRenderer.enabled)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = unknownRoom;
        }
    }

    public void HighlightCurrentRoom()
    {
        spriteRenderer.color = currentRoom;
    }

    public void HighlightKnownRoom()
    {
        spriteRenderer.color = knownRoom;
    }

    public void SetSizeAndPosition(RoomSize size, Vector2 pos)
    {
        switch (size)
        {
            case RoomSize.Medium:
                transform.localScale = new Vector2(10, 10); 
                transform.position = new Vector2(10 * pos.x, 10 * pos.y);
                break;
            case RoomSize.SmallHorizontal:
                transform.localScale = new Vector2(10, 5);
                transform.position = new Vector2(10 * pos.x, 10 * pos.y);
                break;
            case RoomSize.SmallVertical:
                transform.localScale = new Vector2(5, 10);
                transform.position = new Vector2(10 * pos.x, 10 * pos.y);
                break;
            case RoomSize.BigHorizontal:
                transform.localScale = new Vector2(20, 10);
                transform.position = new Vector2(10 * pos.x + 5, 10 * pos.y);
                break;
            case RoomSize.BigVertical:
                transform.localScale = new Vector2(10, 20);
                transform.position = new Vector2(10 * pos.x, 10 * pos.y + 5);
                break;
            case RoomSize.ExtraBig:
                transform.localScale = new Vector2(20, 20);
                transform.position = new Vector2(10 * pos.x + 5, 10 * pos.y + 5);
                break;
        }
    }
}
