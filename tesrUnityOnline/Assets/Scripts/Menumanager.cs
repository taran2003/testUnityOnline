using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class Menumanager : MonoBehaviourPunCallbacks
{
    public InputField create;
    public InputField join;
    public InputField userName;
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        SetUserName();
        PhotonNetwork.CreateRoom(create.text, roomOptions);
    }

    public void JoinRoom()
    {
        SetUserName();
        PhotonNetwork.JoinRoom(join.text);
    }

    private void SetUserName()
    {
        if (PlayerPrefs.HasKey("NickName"))
        {
            if (userName.text.Length == 0)
            {
                PhotonNetwork.NickName = Random.Range(int.MinValue, int.MaxValue).ToString();
            }
            else
            {
                PhotonNetwork.NickName = userName.text;
            }
        }
    }

    public override void OnJoinedRoom()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("K", 0);
        h.Add("D", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(h);
        PhotonNetwork.LoadLevel("Game");
        base.OnJoinedRoom();
    }
}
