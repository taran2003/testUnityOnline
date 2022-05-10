using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

public class Player : MonoBehaviourPun, IPunObservable
{
    public PlayerLocal playerLocal;
    PhotonView view;
    Photon.Realtime.Player lastDamagePlayer;
    bool dead = false;
    bool isHit = false;
    public int k, d;
    public SpriteRenderer sprite;

    private void Awake()
    {        
        if (!photonView.IsMine)
        {
            playerLocal.enabled = false;
            GetComponent<PlayerUI>().enabled = false;
            Destroy(playerLocal.cam.gameObject);
            
        }
        
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
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
            if (photonView.Owner.CustomProperties["K"] != null)
            {
                k = (int)photonView.Owner.CustomProperties["K"];
                d = (int)photonView.Owner.CustomProperties["D"];
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
            if (lastDamagePlayer != null)
            {
                var ldp = PhotonNetwork.PlayerList.ToList().Find(x => x.UserId == lastDamagePlayer.UserId);
                if (ldp != null)
                {
                    ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                    h.Add("K", (int)ldp.CustomProperties["K"] +1);
                    h.Add("D", (int)ldp.CustomProperties["D"]);
                    ldp.SetCustomProperties(h);
                }
            }
            d++;
            SaveKD();
            PhotonNetwork.Destroy(gameObject);
            GameChat.Instance.SendChatMessage($"{view.Owner.NickName} killed by {lastDamagePlayer.NickName}");
            dead = true;
            FindObjectOfType<GameManager>().StartCoroutine(FindObjectOfType<GameManager>().Respawn());
        }
    }

    public void Kick(Photon.Realtime.Player pl)
    {
        PhotonNetwork.CloseConnection(pl);
    }

    [PunRPC]
    public void AddKill()
    {
        k++;
        SaveKD();
    }
    public void SaveKD()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("K", k);
        h.Add("D", d);
        photonView.Owner.SetCustomProperties(h);
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
            stream.SendNext(k);
            stream.SendNext(d);
        }
        else
        {
            sprite.transform.localRotation = (Quaternion)stream.ReceiveNext();       
            isHit = (bool)stream.ReceiveNext();
            k = (int)stream.ReceiveNext();
            d = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void TakeDamage(Vector2 position ,Photon.Realtime.Player sender)
    {
        if (photonView.IsMine)
        {
            float dist = Vector2.Distance(position, transform.position);
            if (dist < 0.5 && sender.UserId != photonView.Owner.UserId)
            {
                lastDamagePlayer = sender;
                isHit = true;
            }
        }
    }
}
