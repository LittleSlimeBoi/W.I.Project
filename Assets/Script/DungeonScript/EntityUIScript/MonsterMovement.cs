using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Vector2[] speeds; // x: speed, y: end frame
    [SerializeField] private float defaultSpeed = 3f;
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float viewDistance = 4f;
    [SerializeField] private int rayCount = 5;
    [SerializeField] private LayerMask layer;
    private RaycastHit2D[] hits = new RaycastHit2D[1];
    private int patrolIndex = 0;
    private Vector2 lastDirection;
    private Vector2 currentDirection;
    private float moveTime = 0f;
    private Animator animator;
    private bool initState = true;

    private void OnEnable()
    {
        moveState = initMoveState;
        patrolIndex = 0;
        transform.position = patrolPoints[0].position;
        lastDirection = (patrolPoints[0].position - transform.position).normalized;
        animator = GetComponent<Animator>();
        initState = true;
    }

    private void Update()
    {
        if (moveTime > 100f) moveTime = 0;
        moveTime += Time.deltaTime;
        switch (moveState)
        {
            case MonsterMoveState.Idle:
                if (initState)
                {
                    initState = false;
                    animator.SetBool("Idle", true);
                }
                break;
            case MonsterMoveState.Patrol:
                if (initState)
                {
                    initState = false;
                    animator.SetBool("Idle", false);
                }
                Patrol();
                break;
            case MonsterMoveState.Noticed:
                if (initState)
                {
                    initState = false;
                    //animator.SetBool("Idle", false);
                }
                break;
        }
    }

    private float SpeedSyncWithAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1f;
        float clipLengthInFrames = stateInfo.length * 60;
        float currentFrame = normalizedTime * clipLengthInFrames;

        for (int i = 0; i < speeds.Length; i++)
        {
            float phaseEndInFrames = speeds[i].y;
            if (currentFrame <= phaseEndInFrames)
            {
                defaultSpeed = speeds[i].x;
                break;
            }
        }
        return defaultSpeed;
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPos = patrolPoints[patrolIndex];
        Vector2 direction = ((Vector2)targetPos.position - (Vector2)transform.position).normalized;
        GetComponent<SpriteRenderer>().flipX = direction.x < -0.001f;

        transform.position = Vector2.MoveTowards(transform.position, targetPos.position, SpeedSyncWithAnimation() * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos.position) < 0.1f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            moveTime = 0f;
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
        
        VisionWithRaycast(currentDirection);
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

            int hitCount = Physics2D.RaycastNonAlloc(transform.position, angleDirection, hits, viewDistance, layer);
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
