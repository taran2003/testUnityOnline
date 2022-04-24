using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce; 
    public float radius;
    public int layer;
    SpriteRenderer sprite;
    Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 dir = new Vector3();
    PhotonView view;


    void Start()
    {
        view = GetComponent<PhotonView>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        layer = LayerMask.NameToLayer("Ground");
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
            if (Input.GetKey(KeyCode.Space) && isGrounded)
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
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, 1 << layer);
        isGrounded = colliders.Length >= 1;
    }
}
