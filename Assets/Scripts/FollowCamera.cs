using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;      
    public float smoothing = 5f;
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }


    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        }
    }
}