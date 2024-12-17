
using UnityEngine;
using UnityEngine.AI;

public class navtest : MonoBehaviour
{
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        agent.SetDestination(mouse);

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 forwardDirection = agent.velocity.normalized;

            // Xoay sprite theo hướng di chuyển
            float angle = Mathf.Atan2(forwardDirection.y, forwardDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
