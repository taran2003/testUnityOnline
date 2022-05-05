using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatItem : MonoBehaviour
{
    public Photon.Realtime.Player player;

    public Text nm, k, d;

    private void Start()
    {
        if (player.CustomProperties["K"] != null)
        {
            nm.text = player.NickName.Split('-')[0];
            k.text = ((int)player.CustomProperties["K"]).ToString("0");
            d.text = ((int)player.CustomProperties["D"]).ToString("0");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}