using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeSwing : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float angle = 20.0f;

    private float currentAngle = 0;
    private float timer;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * speed;
        float angle = Mathf.Sin(timer) * this.angle;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + currentAngle));
    }
}
