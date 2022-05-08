using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Menumanager : MonoBehaviourPunCallbacks
{
    public InputField create;
    public InputField join;
    public InputField userName;
    public ItemList itemPref;
    public Transform content;
    public Text errorText;
    public GameObject errorPref;

    public void UpdateLobiList()
    {
        OnConnectedToMaster();
    }

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

    public void SetUserName()
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

    public void Quit()
    {
        Application.Quit();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorPref.SetActive(true);
        errorText.text = message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorPref.SetActive(true);
        errorText.text = message;
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        HashSet<RoomInfo> rooms = new HashSet<RoomInfo>(); 
        foreach(RoomInfo info in roomList)
        {
            rooms.Add(info);
            
        }
        foreach (RoomInfo info in rooms)
        {
            ItemList itemList = Instantiate(itemPref, content);
            if (itemList != null) itemList.SetInfo(info);
        }
    }
}
