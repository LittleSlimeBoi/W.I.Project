using UnityEngine;

public class CharacterDungeonManager : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
