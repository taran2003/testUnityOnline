using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{ 
    public float timeToLive;
    public string tag;
    public float speed;
    float time;
    Photon.Realtime.Player senderPh;
    PhotonView view;
    Vector3 dir;

    void Update()
    {
        if (time > timeToLive)
        {
            GetComponent<PhotonView>().RPC("Del", RpcTarget.AllBuffered);
        }
        time += Time.deltaTime;
        transform.Translate(dir * speed * Time.deltaTime);
    }

    [PunRPC]
    public void Set(Photon.Realtime.Player sender, Vector3 direction)
    {
        senderPh = sender;
        dir = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            foreach (var item in FindObjectsOfType<Player>())
            {
                item.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, (Vector2)transform.position, senderPh);
            }    
            if (collision.tag == tag)
            {
                GetComponent<PhotonView>().RPC("Del", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void Del()
    {
        time = 0;
        Destroy(gameObject);
    }
}
