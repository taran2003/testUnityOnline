using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatManager : MonoBehaviour
{

    public PhotonView photonView;
    public GameObject Bubble;
    public Text UpdatedText;

    private InputField ChatInputField;
    private bool DisableSend;

    private void Awake()
    {
        ChatInputField = GameObject.Find("ChatInputField").GetComponent<InputField>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (!DisableSend && ChatInputField.isFocused)
            {
                if (ChatInputField.text != "" && ChatInputField.text.Length > 0 && Input.GetKey(KeyCode.Return))
                {
                    photonView.RPC("SendMessage", RpcTarget.AllBuffered, ChatInputField.text);
                    Bubble.SetActive(true);

                    ChatInputField.text = "";
                    DisableSend = true;
                }
            }
        }
    }

    [PunRPC]
    private void SendMessage(string message)
    {
        UpdatedText.text = message;
        StartCoroutine("Remove");
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(4f);
        Bubble.SetActive(false);
        DisableSend = false;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Bubble.active);
        }
        else if (stream.IsReading)
        {
            Bubble.SetActive((bool)stream.ReceiveNext());
        }
    }
}


