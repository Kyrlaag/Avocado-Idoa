using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isFaceRight = true;
    private Rigidbody2D rb;
    private RaycastHit2D hit;
    private float walkDirection;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform rayPoint;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        walkDirection = transform.localScale.x;
    }

    void Update()
    {
        hit = Physics2D.Raycast(rayPoint.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
            Flip();

    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(walkDirection * moveSpeed, rb.velocity.y);
    }
    private void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        isFaceRight = !isFaceRight;
        walkDirection = transform.localScale.x;

    }
}
