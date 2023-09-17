using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    public enum EGameState
    {
        Game,
        EndGame,
        ChangeMap,
    }   
    
    [SerializeField]
    private List<Transform> playerSpawnPosList;

    [SerializeField]
    private List<Camera> playerCameraList;

    private Camera _mainCam;

    private int currentLife = 3;
    private int otherLife = 3;
    void Start()
    {
        currentLife = 3;
        SetCamera();
        SpawnPlayer();

        EventManager.StartListening(EGameEvent.Explosion, Expolsion);
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

        _mainCam = playerCameraList[PhotonNetwork.IsMasterClient? 0 : 1];
    }

    public void SetMyLife(int value)
    {
        currentLife += value;
    }

    public void SetOtherLife(int value)
    {
        otherLife += value;
    }
    
    private void Expolsion(object[] ps)
    {
        ShakeCamera(0.3f, 0.1f, 20);
    }


    public void ShakeCamera(float duration, float strength ,int vibrato)
    {
        Camera.main.DOKill(true);
        Camera.main.DOShakePosition(duration, strength, vibrato);
    }

}
