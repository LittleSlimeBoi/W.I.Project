using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public enum MonsterMoveState
    {
        Patrol,
        Noticed,
        Idle
    }
    [HideInInspector] public MonsterMoveState moveState;
    [SerializeField] private MonsterMoveState initMoveState;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PatrolPoint[] patrolPoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;
    [SerializeField] private int rayCount;
    [SerializeField] private LayerMask detectLayer;
    private RaycastHit2D[] hits = new RaycastHit2D[1];
    private int patrolIndex = 0;
    private Vector2 lastDirection;
    private Vector2 currentDirection;
    private float moveTime = 0f;
    private float idleTime = 0f;
    private Animator animator;
    private bool newState = true;

    private void OnEnable()
    {
        moveState = initMoveState;
        patrolIndex = 0;
        transform.position = patrolPoints[0].transform.position;
        lastDirection = (patrolPoints[0].transform.position - transform.position).normalized;
        animator = GetComponent<Animator>();
        newState = true;
    }

    private void Update()
    {
        switch (moveState)
        {
            case MonsterMoveState.Idle:
                if (newState)
                {
                    newState = false;
                    animator.SetBool("Idle", true);
                    // Start animation from its begining
                    animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                }
                Idle();
                break;
            case MonsterMoveState.Patrol:
                if (newState)
                {
                    newState = false;
                    animator.SetBool("Idle", false);
                    // Start animation from its begining
                    animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                }
                Patrol();
                break;
            case MonsterMoveState.Noticed:
                if (newState)
                {
                    newState = false;
                    //animator.SetBool("Idle", false);
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        VisionWithRaycast(currentDirection);
    }

    private void Idle()
    {
        idleTime += Time.deltaTime;
        if(idleTime >= patrolPoints[patrolIndex].stopTime)
        {
            moveState = MonsterMoveState.Patrol;
            newState = true;
            idleTime = 0;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        if (moveTime > 100f) moveTime = 0;
        moveTime += Time.deltaTime;

        Transform targetPos = patrolPoints[patrolIndex].transform;
        Vector2 direction = ((Vector2)targetPos.position - (Vector2)transform.position).normalized;
        // Set sprite flip on the first frame if it's starting with idle
        if (moveTime > Time.deltaTime) spriteRenderer.flipX = direction.x < -0.001f;

        transform.position = Vector2.MoveTowards(transform.position, targetPos.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos.position) < 0.1f)
        {
            moveTime = 0f;
            if (patrolPoints[patrolIndex].stopping)
            {
                moveState = MonsterMoveState.Idle;
                newState = true;
                idleTime = 0;
                return;
            }
            else patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        // Smooth rotation
        if (currentDirection != direction)
        {
            currentDirection = Vector2.Lerp(lastDirection, direction, Mathf.Clamp01(rotateSpeed * moveTime));
        }
        else
        {
            lastDirection = direction;
        }
    }

    private void VisionWithRaycast(Vector2 direction)
    {
        float startAngle = -viewAngle / 2;
        float angleStep = viewAngle / (rayCount - 1);

        System.Array.Clear(hits, 0, hits.Length);

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector2 angleDirection = Quaternion.Euler(0, 0, currentAngle) * direction;

            int hitCount = Physics2D.RaycastNonAlloc(transform.position, angleDirection, hits, viewDistance, detectLayer);
            bool hitSomething = false;
            for (int j = 0; j < hitCount; j++)
            {
                RaycastHit2D hit = hits[j];
                if (hit.collider != null)
                {
                    hitSomething = true;
                    if ((1 << hit.collider.gameObject.layer) == LayerMask.GetMask("Player"))
                    {
                        // Do something when the player is detected
                        DungeonManager.Instance.CloseCurrentRoom();
                    }
                }
            }
            // Visualize the ray
            Debug.DrawRay(transform.position, angleDirection * viewDistance, hitSomething ? Color.red : Color.green);
        }
    }
}
