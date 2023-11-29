using UnityEngine;

public class TraversalCameraController : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f,0f,-10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    void Update(){
        //Vector3 targetPosition = target.position+offset;
        Vector3 targetPosition = new Vector3(target.position.x + offset.x, transform.position.y, offset.z);

        transform.position = Vector3.SmoothDamp(transform.position,targetPosition,ref velocity, smoothTime);
    }

}
