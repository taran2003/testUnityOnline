using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerLocal : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float radius;
    Rigidbody2D rb;
    int layer;
    private bool isGrounded;
    private float cd;
    public float fireSpeed;
    public Transform firePoint;
    public Rigidbody2D bullet;
    public Camera cam;
    Vector3 diff;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        layer = LayerMask.NameToLayer("Ground");
        cd = 0;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }


    void Update()
    {
        if (Input.GetButton("Horizontal"))
        {
            Run();
            diff = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            diff.Normalize();
        }
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Fire();
        }
        cd += Time.deltaTime;
    }

    private void Run()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
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
    void Fire()
    {
        cd += Time.deltaTime;
        if (cd > fireSpeed)
        {
            cd = 0;
            var bl = PhotonNetwork.Instantiate(bullet.name, firePoint.position, transform.rotation);
            bl.GetPhotonView().RPC("Set", RpcTarget.AllBuffered, GetComponent<PhotonView>().Owner.UserId, GetComponent<PhotonView>().Owner.NickName);
        }
    }

}
