using System;
using UnityEngine;

public class MonsterMovementManager : MonoBehaviour
{
    [SerializeField] private MonsterMoveState moveState;
    [SerializeField] private MonsterMoveType moveTypes;
    [SerializeField] private MonsterMovementBehavior moveBehavior;
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
    private MonsterMoveType currentType;
    private Vector3Int pursuitDirection;
    private Vector2Int targetGridPos = new(-1, -1);

    public MonsterMoveType MoveTypes => moveTypes;

    private void OnEnable()
    {
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
                    animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                }
                Idle();
                break;
            case MonsterMoveState.Patrol:
                if (newState)
                {
                    newState = false;
                    animator.SetBool("Idle", false);
                    animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                }
                Patrol();
                break;
            case MonsterMoveState.Pursuit:
                if (newState)
                {
                    newState = false;
                    animator.SetTrigger("Noticed");
                    animator.SetBool("Idle", false);
                    animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                }
                Pursuit();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (moveState != MonsterMoveState.Pursuit) VisionWithRaycast(currentDirection);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            ObstacleType obstacleType = obstacle.type;
            //moveBehavior.OnObstacleEnter(obstacleType, moveTypes, ref currentType);
        }
    }

    private void Pursuit()
    {
        if (targetGridPos != new Vector2Int(-1, -1) && 
            Vector2.Distance(WorldToGridFloat(transform.localPosition), targetGridPos) > 0.1)
        {
            if (pursuitDirection.x != 0)
                spriteRenderer.flipX = pursuitDirection.x < 0;

            transform.localPosition = Vector2.MoveTowards(
                transform.localPosition, 
                GridToWorld(targetGridPos), 
                moveSpeed * Time.deltaTime * 2
            );
            return;
        }

        Vector2Int pos = WorldToGridInt(transform.localPosition);
        Vector2Int bestDir = Vector2Int.zero, bestVDir = Vector2Int.zero, bestHDir = Vector2Int.zero;
        int record = Room.MAX_DISTANCE;
        Vector2Int[] vDirs = { Vector2Int.up, Vector2Int.down };
        Vector2Int[] hDirs = { Vector2Int.left, Vector2Int.right };

        foreach (MonsterMoveType movetype in Enum.GetValues(typeof(MonsterMoveType)))
        {
            if ((moveTypes & movetype) == 0) continue;
            int[,] myDistanceMap = DungeonManager.currentRoom.DistanceMap[movetype];

            foreach (Vector2Int dir in hDirs)
            {
                Vector2Int next = pos + dir;
                if (next.x < 0 || next.x >= DungeonManager.currentRoom.GetRoomWidth()) continue;
                if (record >= myDistanceMap[next.y, next.x])
                {
                    record = myDistanceMap[next.y, next.x];
                    bestHDir = dir;
                    bestDir = dir;
                }
            }
            foreach (Vector2Int dir in vDirs)
            {
                Vector2Int next = pos + dir;
                if (next.y < 0 || next.y >= DungeonManager.currentRoom.GetRoomHeight()) continue;
                if (record >= myDistanceMap[next.y, next.x])
                {
                    record = myDistanceMap[next.y, next.x];
                    bestVDir = dir;
                    bestDir = dir;
                }
            }
            if (myDistanceMap[pos.y, pos.x + bestHDir.x] != Room.MAX_DISTANCE &&
                myDistanceMap[pos.y + bestVDir.y, pos.x] != Room.MAX_DISTANCE &&
                myDistanceMap[pos.y + bestVDir.y, pos.x + bestHDir.x] != Room.MAX_DISTANCE)
            {
                Vector2Int diagDir = bestHDir + bestVDir;
                Vector2Int next = pos + diagDir;
                if (record > myDistanceMap[next.y, next.x])
                {
                    record = myDistanceMap[next.y, next.x];
                    bestDir = diagDir;
                }
            }
            pursuitDirection = new Vector3Int(bestDir.x, -bestDir.y);
            targetGridPos = pos + bestDir;
        }
    }

    private void Idle()
    {
        idleTime += Time.deltaTime;
        if (idleTime >= patrolPoints[patrolIndex].stopTime)
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

        Array.Clear(hits, 0, hits.Length);

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
                        moveState = MonsterMoveState.Pursuit;
                        newState = true;
                    }
                }
            }
            // Visualize the ray
            Debug.DrawRay(transform.position, angleDirection * viewDistance, hitSomething ? Color.red : Color.green);
        }
    }

    private Vector2Int WorldToGridInt(Vector2 pos)
    {
        return new Vector2Int(
            (DungeonManager.currentRoom.GetRoomWidth() - 1) / 2 + Mathf.RoundToInt(pos.x),
            DungeonManager.currentRoom.GetRoomHeight() / 2 - Mathf.RoundToInt(pos.y)
        );
    }
    private Vector2 WorldToGridFloat(Vector2 pos)
    {
        return new Vector2(
            (DungeonManager.currentRoom.GetRoomWidth() - 1) / 2 + pos.x,
            DungeonManager.currentRoom.GetRoomHeight() / 2 - pos.y
        );
    }
    private Vector2Int GridToWorld(Vector2Int pos)
    {
        return new Vector2Int(
            pos.x - (DungeonManager.currentRoom.GetRoomWidth() - 1) / 2,
            DungeonManager.currentRoom.GetRoomHeight() / 2 - pos.y
        );
    }
}
public enum MonsterMoveState
{
    Patrol, Pursuit, Idle
}

[Flags]
public enum MonsterMoveType
{
    Walking = 1 << 0,
    Flying = 1 << 1,
    Swimming = 1 << 2
}