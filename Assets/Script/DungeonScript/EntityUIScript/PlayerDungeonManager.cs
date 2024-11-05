using UnityEngine;

public class PlayerDungeonManager : CharacterDungeonManager
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

    }

    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * x + transform.up * y;

        if(x == 0 && y == 0)
            animator.SetBool("Idle", true);
        else
            animator.SetBool("Idle", false);

        if (x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if(x < 0)
        {
            transform.localScale = new Vector3 (-1, 1, 0);
        }

        rb.MovePosition(transform.position + Vector3.Normalize(move) * moveSpeed * Time.deltaTime);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided" + collision.gameObject);
    }
}
