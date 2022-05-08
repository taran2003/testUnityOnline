using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class ItemList : MonoBehaviour
{
    public Text roomName;
    public Text coutPlayers;


    public void SetInfo(RoomInfo info)
    {
        if (info.MaxPlayers == 0) return;
        roomName.text = info.Name;
        coutPlayers.text = info.PlayerCount + "/" + info.MaxPlayers;
    }


    public void Distroy()
    {
        Destroy(this.gameObject);
    }

    public void ConetToRoom()
    {
        FindObjectOfType<Menumanager>().SetUserName();
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
