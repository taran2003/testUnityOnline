using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public float speed;
    private float jumpForce = 15f; 
    public float radius; 
    PhotonView view;
    SpriteRenderer sprite;
    private Vector3 dir = new Vector3();
    Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        view = GetComponent<PhotonView>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    void Update()
    {
        if (view.IsMine) 
        { 
            if (Input.GetButton("Horizontal"))
            {
                Run();
            }
            if (isGrounded && Input.GetButton("Jump"))
            {
                Jump();
            }
            //Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            //Vector2 moveAmount = moveInput.normalized * speed * Time.deltaTime;
            //transform.position += (Vector3)moveAmount;
        }
        if(Input.GetButton("Horizontal"))
        {
            sprite.flipX = dir.x < 0.0f;
        }
    }

    private void Run()
    {
        dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        isGrounded = colliders.Length > 1;
    }
}
