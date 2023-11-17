using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaController : MonoBehaviour
{
    private Animator animator;
    private bool isMovingRight;
    private float maxDistance = 6.0f;
    private float startXPosition;
    private float currentXPosition;
    private float direction;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        startXPosition = gameObject.transform.position.x;
        isMovingRight = animator.GetBool("isMovingRight");
    }

    // Update is called once per frame
    void Update()
    {
        // get current direction from the animator
        isMovingRight = animator.GetBool("isMovingRight");
        
        // update direction if passed a max displacement
        currentXPosition = gameObject.transform.position.x;
        float displacement = Math.Abs(currentXPosition - startXPosition);
        if (displacement >= maxDistance)
        {
            if (isMovingRight)
            {
                animator.SetBool("isMovingRight", false);
            }
            else
            {
                animator.SetBool("isMovingRight", true);
            }

            startXPosition = currentXPosition;
        }
        
        // update position
        direction = isMovingRight ? 1.0f : -1.0f;
        gameObject.transform.position += new Vector3(direction * Time.deltaTime + (direction * 0.002f), 0.0f, 0.0f);
    }
}
