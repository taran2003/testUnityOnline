using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPun, IPunObservable
{
    public PlayerLocal playerLocal;
    PhotonView view;
    string lastDamagePlayer;
    bool dead = false;
    bool isHit = false;
    public SpriteRenderer sprite;
    

    void Start()
    {
        view = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            playerLocal.enabled = false;
            Destroy(playerLocal.cam.gameObject);
        }
    }

    void Update()
    {
        if (view.IsMine) 
        { 
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
            GameChat.Instance.SendChatMessage("Player killed");
            dead = true;
            FindObjectOfType<GameManager>().StartCoroutine(FindObjectOfType<GameManager>().Respawn());
        }
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
            stream.SendNext(sprite.transform.localRotation);
            stream.SendNext(isHit);
        }
        else
        {
            sprite.transform.localRotation = (Quaternion)stream.ReceiveNext();       
            isHit = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void TakeDamage(Vector2 position ,Photon.Realtime.Player sender)
    {
        if (photonView.IsMine)
        {
            float dist = Vector2.Distance(position, transform.position);
            if (dist < 0.25 && sender != photonView.Owner)
            {
                lastDamagePlayer = sender.NickName;
                isHit = true;
            }
        }
    }
}
