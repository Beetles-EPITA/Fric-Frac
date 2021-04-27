using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviourPun
{

    [SerializeField] private TextMeshProUGUI nameTagText;
    private void Start()
    {
        if (photonView.IsMine) return;
        SetName();
    }

    private void SetName()
    {
        nameTagText.text = photonView.Owner.NickName;
    }
}

