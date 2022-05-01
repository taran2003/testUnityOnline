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

    public IEnumerator Respawn()
    {
        canRespawn = false;
        dead = true;
        yield return new WaitForSeconds(2);
        //deadCam.GetComponentInChildren<TMPro.TMP_Text>().text = "Вы мертвы";
        canRespawn = true;
        yield return null;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Player.RefreshInstance(ref LocalPlayer, playerPrefab);
    }

}
