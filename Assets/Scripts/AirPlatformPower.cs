using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlatformPower : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.name == "Player")
        {
            player.AddForce(new Vector2(0, 1000));
        }
    }
}
