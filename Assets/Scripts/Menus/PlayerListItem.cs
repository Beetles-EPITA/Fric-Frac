using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;
    private Player _player;
    
    public void SetUp(Player player, bool death = false)
    {
        _player = player;
        text.text = player.NickName + (player == PhotonNetwork.LocalPlayer ? " (You)" : "");
        if(death) text.fontStyle = FontStyles.Strikethrough;
        image.gameObject.SetActive(player.IsMasterClient);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (Equals(_player, otherPlayer))
        {
            Destroy(gameObject);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (Equals(_player, newMasterClient))
        {
            image.gameObject.SetActive(true);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
