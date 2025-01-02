using UnityEngine;

public class PlayerDungeonManager : CharacterDungeonManager
{
    public float moveSpeed = 5f;

    // Start is called before the first frame update
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

            rb.MovePosition(transform.position + moveSpeed * Time.deltaTime * Vector3.Normalize(move));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collided" + collision.gameObject);
    }
}
