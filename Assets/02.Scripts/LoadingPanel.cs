using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoadingPanel : MonoBehaviour
{
    private Action<object[]> OnShow;
    private Action<object[]> OnHide;

    private void Awake()
    {
        OnShow += (a) => Show();
        OnHide += (a) => Hide();
        EventManager.StartListening(ENetworkEvent.Connecting, OnShow);
        EventManager.StartListening(ENetworkEvent.JoinedLobby, OnHide);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
