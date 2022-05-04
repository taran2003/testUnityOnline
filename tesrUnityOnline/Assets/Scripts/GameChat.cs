using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameChat : MonoBehaviour
{
    public PhotonView photonView;
    public const int MaxMessage = 7;
    private static GameChat _instance;
    public static GameChat Instance { get { return _instance; } }

    private float time=0f;
    private readonly List<string> _messages = new List<string>();
    private string _messageText = "";
    private bool _showInput;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        _instance = this;
    }
   

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Slash) && !_showInput)
        {
            _showInput = true;
        }

        if(_messages.Count > 0)
        time += Time.deltaTime;

        if (time > 10f && photonView.IsMine)
        {
            photonView.RPC("Delete", RpcTarget.All);
            time = 0f;
        }
    }

    
     void OnGUI()
     {
        GUILayout.BeginArea(new Rect(10, Screen.height - 20 * MaxMessage - 30, 250, 20 * MaxMessage));
        GUILayout.Label(string.Join("\n", _messages.ToArray()));
        GUILayout.EndArea();

        if (_showInput)
        {
            GUI.SetNextControlName("tbMessage");
            _messageText = GUI.TextField(new Rect(10, Screen.height - 25, 150, 20), _messageText, 20);
            GUI.FocusControl("tbMessage");


            if(Event.current !=null && Event.current.keyCode == KeyCode.Return)
            {
                if(!string.IsNullOrEmpty(_messageText))
                {
                    SendChatMessage($"{photonView.Owner.NickName}:   {_messageText}");
                }

                _messageText = "";
                _showInput = false;
            }

        }
     }


    void OnLeftRoom()
    {
        _messages.Clear();
    }

    public void SendChatMessage(string message)
    {
        photonView.RPC("ReceivedMessage", RpcTarget.All, message);
    }

    [PunRPC]
    private void ReceivedMessage(string message)
    {
        _messages.Add(message);
    }

    [PunRPC]
    private void Delete()
    {
        _messages.RemoveAt(0);
    }
}