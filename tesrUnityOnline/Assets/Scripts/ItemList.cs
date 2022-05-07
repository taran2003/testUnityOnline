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
        roomName.text = info.Name;
        coutPlayers.text = info.PlayerCount + "/" + info.MaxPlayers;
    }

    public void ConetToRoom()
    {
        FindObjectOfType<Menumanager>().SetUserName();
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
