using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyStartBtn : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(NetworkManager.Inst.QuickMatch);
    }
}
