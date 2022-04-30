using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun, IPunObservable
{
    public float speed;
    public float jumpForce; 
    public float radius;
    public int layer;
    public Player localPlayer;
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
        }
    }

    private void Run()
    {
        dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
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

    public static void RefreshInstance(ref Player player, Player playerPrefab, bool withMasterClient = false)
    {
        if (!PhotonNetwork.IsMasterClient || withMasterClient)
        {
            print("Respawn");
            var pos = FindObjectOfType<GameManager>().spawns[Random.Range(0, FindObjectOfType<GameManager>().spawns.Length)].transform.position;
            var rot = Quaternion.identity;
            if (player != null)
            {
                pos = player.transform.position;
                rot = player.transform.rotation;
                PhotonNetwork.Destroy(player.gameObject);
            }
            player = PhotonNetwork.Instantiate(playerPrefab.gameObject.name, pos, rot).GetComponent<Player>();

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(localPlayer == null)
        {
            return;
        }
        if(stream.IsWriting)
        {
            stream.SendNext(localPlayer.sprite.flipX);
        }
        else
        {
            localPlayer.sprite.flipX = (bool)stream.ReceiveNext();            
        }
    }
}
