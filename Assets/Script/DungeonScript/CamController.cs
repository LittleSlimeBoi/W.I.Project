using System.Collections;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public static CamController Instance;
    public Transform player;

    private float minX = 0.5f, maxX = 0.5f, minY = 0.5f, maxY = 0.5f;
    public void SetRoomBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
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

    private void LateUpdate()
    {
        if (player != null)
        {
            FollowPlayerWithinBounds();
        }
    }

    private void FollowPlayerWithinBounds()
    {
        Vector3 targetPos = new Vector3(
            Mathf.Clamp(player.position.x, minX, maxX),
            Mathf.Clamp(player.position.y, minY, maxY),
            transform.position.z
        );

        // Move the camera to the clamped position smoothly
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
    }
}