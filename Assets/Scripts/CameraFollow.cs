using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followSpeed = 2f;
    public Transform target;

    private float originalXPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Save the original x position of the camera
        originalXPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Keep the camera's x position constant
        Vector3 newPos = new Vector3(originalXPosition, target.position.y, -20f);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
    }
}
