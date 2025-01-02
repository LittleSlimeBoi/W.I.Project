using System.Collections;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public static CamController Instance;
    public Transform player;

    public float MinX { get; private set; } = 0.5f;
    public float MinY { get; private set; } = 0.5f;
    public float MaxX { get; private set; } = 0.5f;
    public float MaxY { get; private set; } = 0.5f;
    public float TransitionX => Mathf.Abs(transform.position.x - MinX) < Mathf.Abs(transform.position.x - MaxX) ? MinX : MaxX;
    public float TransitionY => Mathf.Abs(transform.position.y - MinY) < Mathf.Abs(transform.position.y - MaxY) ? MinY : MaxY;

    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.MinX = minX;
        this.MaxX = maxX;
        this.MinY = minY;
        this.MaxY = maxY;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Camera.main.orthographicSize = 19;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            if (!Door.isGoingThroughDoor)
            {
                FollowPlayerWithinBounds();
            }
        }
    }

    private void FollowPlayerWithinBounds()
    {
        Vector3 targetPos = new Vector3(
            Mathf.Clamp(player.position.x, MinX, MaxX),
            Mathf.Clamp(player.position.y, MinY, MaxY),
            transform.position.z
        );

        // Move the camera to the clamped position smoothly
        transform.position = Vector3.Lerp(transform.position, targetPos, 1f);
    }

}