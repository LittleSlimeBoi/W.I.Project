using UnityEngine;

public class PlayerDungeonManager : CharacterDungeonManager
{
    public static PlayerDungeonManager Instance;
    public float moveSpeed = 5f;
    private Vector2Int lastPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void Start()
    {
        base.Start();

    }

    private void FixedUpdate()
    {
        if (!Door.isGoingThroughDoor)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            Vector3 move = transform.right * x + transform.up * y;
            rb.MovePosition(transform.position + moveSpeed * Time.deltaTime * Vector3.Normalize(move));

            UpdateDistanceMap();

            if (x == 0 && y == 0)
                animator.SetBool("Idle", true);
            else
                animator.SetBool("Idle", false);

            if (x > 0)
            {
                transform.localScale = Vector3.one;
            }
            else if (x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 0);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collided" + collision.gameObject);
    }

    private void UpdateDistanceMap()
    {
        int minWidth = -(DungeonManager.currentRoom.GetRoomWidth() - 1) / 2;
        int maxWidth = DungeonManager.currentRoom.GetRoomWidth() / 2;
        int minHeight = -(DungeonManager.currentRoom.GetRoomHeight() - 1) / 2;
        int maxHeight = DungeonManager.currentRoom.GetRoomHeight() / 2;

        Vector2Int currentPos = new Vector2Int(
            Mathf.RoundToInt(transform.localPosition.x),
            Mathf.RoundToInt(transform.localPosition.y)
        );

        if (currentPos.x < minWidth || currentPos.x > maxWidth ||
            currentPos.y < minHeight || currentPos.y > maxHeight)
        {
            return;
        }

        if (lastPos != currentPos)
        {
            lastPos = currentPos;
            Vector2Int translatedPos = new(currentPos.x - minWidth, maxHeight - currentPos.y);
            //Debug.Log(translatedPos.ToString());
            DungeonManager.currentRoom.UpdateDistanceMap(translatedPos);
        }
    }
}
