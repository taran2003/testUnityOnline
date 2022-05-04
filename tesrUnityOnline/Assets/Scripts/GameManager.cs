using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] spawns;
    public bool canRespawn,dead;
    public Player playerPrefab;
    public Player LocalPlayer;
    public GameChat chat;

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("LoadSkrin");
            return;
        }
    }

    private void Update()
    {
        Application.targetFrameRate = 60;
        if (dead && canRespawn)
        {
            RespawnPlayer();
            dead = false;
        }
    }

    public void RespawnPlayer()
    {
        if (canRespawn)
        {
            Player.RefreshInstance(ref LocalPlayer, playerPrefab, true);
            dead = false;
        }
    }

    public void Disconnect()
    {
        if (LocalPlayer != null)
        {
            PhotonNetwork.Destroy(LocalPlayer.gameObject);
        }
        PhotonNetwork.LeaveRoom();
        Destroy(GameObject.Find("MenuManager"));
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    public IEnumerator Respawn()
    {
        canRespawn = false;
        dead = true;
        yield return new WaitForSeconds(2);
        //deadCam.GetComponentInChildren<TMPro.TMP_Text>().text = "Вы мертвы";
        canRespawn = true;
        yield return null;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.IsLocal)
        {
            Disconnect();
        }
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            Disconnect();
        }
        PlayerPrefs.SetString("Disconnect", "Server close connection");
        base.OnMasterClientSwitched(newMasterClient);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        chat.SendChatMessage($"{newPlayer.NickName} connected to lobby");
        Player.RefreshInstance(ref LocalPlayer, playerPrefab);
    }

}
