using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform target;
    public Vector3 offset; // Adjust the camera's relative position
    public float smoothSpeed; // For smooth camera movement
    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate the desired position based on the target and offset
        if (target)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
