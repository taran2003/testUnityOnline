using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun, IPunObservable
{
    public float speed;
    public float jumpForce; 
    public float radius;
    //public Player localPlayer; 
    public Rigidbody2D bullet;
    public float fireSpeed;
    public Transform firePoint;
    SpriteRenderer sprite;
    Rigidbody2D rb;
    int layer;
    private bool isGrounded;
    private Vector3 dir = new Vector3();
    PhotonView view;
    string lastDamagePlayer;
    private float cd;
    bool dead = false;
    bool isHit = false;

    void Start()
    {
        view = GetComponent<PhotonView>();
        sprite = GetComponentInChildren<SpriteRenderer>();
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
            if(Input.GetKeyDown(KeyCode.J))
            {
                Fire();
            }
            cd += Time.deltaTime;
            if(isHit)
            {
                Dead();
                isHit = false;
            }
        }
        else if (isHit)
        {
            dead = true;
        }
    }

    void Dead()
    {
        if(photonView.IsMine && !dead)
        {
            PhotonNetwork.Destroy(gameObject);
            dead = true;
            FindObjectOfType<GameManager>().StartCoroutine(FindObjectOfType<GameManager>().Respawn());
        }
    }

    private void Run()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal")*speed, rb.velocity.y);
        if (Input.GetAxis("Horizontal") < 0) transform.localRotation = Quaternion.Euler(0, 180, 0);
        if (Input.GetAxis("Horizontal") > 0) transform.localRotation = Quaternion.Euler(0, 0, 0);
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
        if(stream.IsWriting)
        {
            stream.SendNext(transform.localRotation);
            stream.SendNext(isHit);
        }
        else
        {
            transform.localRotation = (Quaternion)stream.ReceiveNext();       
            isHit = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void TakeDamage(Vector2 position ,string actorName)
    {
        if (photonView.IsMine)
        {
            float dist = Vector2.Distance(position, transform.position);
            if (dist < 0.3 && actorName != photonView.Owner.UserId)
            {
                lastDamagePlayer = actorName;
                isHit = true;
            }
        }
    }

    void Fire()
    {
        cd += Time.deltaTime;
        if (cd > fireSpeed)
        {
            cd = 0;
            var bl = PhotonNetwork.Instantiate(bullet.name, firePoint.position, transform.rotation);
            bl.GetPhotonView().RPC("Set", RpcTarget.AllBuffered, GetComponent<PhotonView>().Owner.UserId);
        }
    }
}
