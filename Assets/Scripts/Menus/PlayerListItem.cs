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
    private Player _player;
    
    public void SetUp(Player player, bool death = false)
    {
        _player = player;
        text.text = player.NickName + (player == PhotonNetwork.LocalPlayer ? " (You)" : "");
        if(death) text.fontStyle = FontStyles.Strikethrough;
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (Equals(_player, otherPlayer))
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
