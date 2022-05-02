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
    string senderId;
    string senderName;
    PhotonView view;
    //void Start()
    //{
    //    view = GetComponent<PhotonView>();
    //    Destroy(gameObject, timeToLive);
    //}

    void Update()
    {
        if (time > timeToLive)
        {
            GetComponent<PhotonView>().RPC("Del", RpcTarget.AllBuffered);
        }
        time += Time.deltaTime;
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    [PunRPC]
    public void Set(string snId, string snName)
    {
        senderId = snId;
        senderName = snName;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            foreach (var item in FindObjectsOfType<Player>())
            {
                item.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, (Vector2)transform.position, senderId, senderName);
            }
            GetComponent<PhotonView>().RPC("Del", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void Del()
    {
        Destroy(gameObject);
    }
}
