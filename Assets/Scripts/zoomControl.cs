using UnityEngine;

public class zoomControl : MonoBehaviour
{
    public float zoomSpeed = 0.5f;
    public float minOrthoSize = 1.0f;
    public float maxOrthoSize = 10.0f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            float newZoom = cam.orthographicSize + zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newZoom, minOrthoSize, maxOrthoSize);
        }
        else if (Input.GetKey(KeyCode.X))
        {
            float newZoom = cam.orthographicSize - zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newZoom, minOrthoSize, maxOrthoSize);
        }
    }
}
