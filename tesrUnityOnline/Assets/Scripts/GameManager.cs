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
    public Player[] playerPrefs;
    public bool canRespawn,dead;
    public Player playerPrefab;
    public Player localPlayer;
    public GameChat chat;
    public GameObject buton;

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("LoadSkrin");
            return;
        }
        playerPrefab = playerPrefs[PlayerPrefs.GetInt("PlayerModel")];
    }

    private void Update()
    {
        Application.targetFrameRate = 60;
        if (dead && canRespawn)
        {
            RespawnPlayer();
            dead = false;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!buton.active) buton.SetActive(true);
            else buton.SetActive(false);
        }
    }

    public void RespawnPlayer()
    {
        if (canRespawn)
        {
            Player.RefreshInstance(ref localPlayer, playerPrefab, true);
            dead = false;
        }
    }

    public void Disconnect()
    {
        if (localPlayer != null)
        {
            PhotonNetwork.Destroy(localPlayer.gameObject);
        }
        PhotonNetwork.LeaveRoom();
        Destroy(GameObject.Find("MenuManager"));
        Cursor.visible = true;
        SceneManager.LoadScene("LoadSkrin");
    }

    public IEnumerator Respawn()
    {
        canRespawn = false;
        dead = true;
        yield return new WaitForSeconds(2);
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
        Player.RefreshInstance(ref localPlayer, playerPrefab);
    }

}
