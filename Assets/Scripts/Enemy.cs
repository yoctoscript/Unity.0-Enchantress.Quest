using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    Rigidbody2D rigidbody = null;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rigidbody.velocity = new Vector2(moveSpeed, 0.0f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Map"))
        {
            moveSpeed = -moveSpeed;
            transform.localScale = new Vector2(-Math.Sign(rigidbody.velocity.x), 1.0f);
        }
    }
}
