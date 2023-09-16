using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private List<Transform> playerSpawnPosList;

    [SerializeField]
    private List<Camera> playerCameraList;

    private int currentLife = 3;
    private int otherLife = 3;
    void Start()
    {
        currentLife = 3;
        SetCamera();
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        int spawnPosIdx = PhotonNetwork.IsMasterClient ? 0 : 1;
        Transform spawnPos = playerSpawnPosList[spawnPosIdx];
        PhotonNetwork.Instantiate("PlayerPrefab", spawnPos.position, spawnPos.rotation);
    }

    private void SetCamera()
    {
        playerCameraList[0].enabled = PhotonNetwork.IsMasterClient;
        playerCameraList[1].enabled = !PhotonNetwork.IsMasterClient;
    }

    public void SetMyLife(int value)
    {
        currentLife += value;
    }

    public void SetOtherLife(int value)
    {
        otherLife += value;
    }

}
