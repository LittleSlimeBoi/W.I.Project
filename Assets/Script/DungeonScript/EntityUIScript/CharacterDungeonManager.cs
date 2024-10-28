using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDungeonManager : MonoBehaviour
{
    public float moveSpeed = 5f;

    protected Rigidbody2D rb;
    protected BoxCollider2D bc;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
