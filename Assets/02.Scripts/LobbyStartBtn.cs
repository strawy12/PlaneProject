using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyStartBtn : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(NetworkManager.Inst.QuickMatch);
        EventManager.StartListening(ENetworkEvent.JoinedLobby, Show);
        gameObject.SetActive(false);
    }

    public void Show(object[] ps)
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        EventManager.StopListening(ENetworkEvent.JoinedLobby, Show);
    }
}
