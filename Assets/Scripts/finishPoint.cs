using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finishPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("collide with player");
            levelSwitch.instance.nextLevel();
        }
    }
}
