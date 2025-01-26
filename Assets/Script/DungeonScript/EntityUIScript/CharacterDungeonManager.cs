using UnityEngine;

public class CharacterDungeonManager : MonoBehaviour
{
    protected Rigidbody2D rb;
    public Animator animator;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
