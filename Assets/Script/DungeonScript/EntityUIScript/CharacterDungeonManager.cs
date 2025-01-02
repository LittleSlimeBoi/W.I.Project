using UnityEngine;

public class CharacterDungeonManager : MonoBehaviour
{
    protected Rigidbody2D rb;
    public Animator animator;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
