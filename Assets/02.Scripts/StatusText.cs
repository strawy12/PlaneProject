using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusText : MonoBehaviourPunCallbacks
{
    private TMP_Text _currentText;
    private bool _isLeftRoom = false;

    private void Awake()
    {
        _currentText = GetComponent<TMP_Text>();
    }

    public override void OnConnected()
    {
        if(_isLeftRoom)
        {
            _isLeftRoom = false;
            return;
        }    
        _currentText.text = "Server Connected";
    }

    public override void OnJoinedRoom()
    {
        _currentText.text = "Joined Room";

        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            _currentText.text = "Joined Room\nIt'll start soon";
        }
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        _currentText.text = "Other Player Entered Room\nIt'll start soon";
    }

    public override void OnLeftRoom()
    {
        _isLeftRoom = true;
        _currentText.text = "Retry Click the Play Button";
    }
}
