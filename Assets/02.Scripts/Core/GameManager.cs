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
        ChangeRound,
    }

    public EGameState gameState;

    [SerializeField]
    private List<Transform> playerSpawnPosList;

    [SerializeField]
    private List<Camera> playerCameraList;
    [SerializeField]
    private Canvas _uiCanvas;

    public Camera mainCam { get; private set; }
    private PhotonView _photonView;
    public PhotonView CurrentPhotonView => _photonView;

    private int _nowRound = 0;

    private int _currentWinCnt = 0;
    private int _otherWinCnt = 0;

    public int CurrentWinCnt => _currentWinCnt;
    public int OtherWinCnt => _otherWinCnt;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        SetCamera(); 
    }

    void Start()
    { 
        SpawnPlayer();
        EventManager.StartListening(EGameEvent.Explosion, Expolsion);

        if (PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC("StartRound", RpcTarget.All);
        }

        UIManager.Inst.Init();
    }

    private void OnDestroy()
    {
        EventManager.StopListening(EGameEvent.Explosion, Expolsion);
    }

    [PunRPC]
    private void StartRound()
    {
        gameState = EGameState.Game;
        _nowRound++;
        EventManager.TriggerEvent(EGameEvent.StartRound, new object[] { _nowRound });
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

        mainCam = playerCameraList[PhotonNetwork.IsMasterClient ? 0 : 1];
        _uiCanvas.worldCamera = mainCam;
    }

    private void Expolsion(object[] ps)
    {
        ShakeCamera(0.3f, 0.1f, 20);
    }


    public void ShakeCamera(float duration, float strength, int vibrato)
    {
        Camera.main.DOKill(true);
        Camera.main.DOShakePosition(duration, strength, vibrato);
    }

    [PunRPC]
    public void RoundWin(bool isMaster)
    {
        int winCnt = 0;
        if(isMaster == PhotonNetwork.IsMasterClient)
        {
            _currentWinCnt++;
            winCnt = _currentWinCnt;
        }

        else
        {
            _otherWinCnt++;
            winCnt = _otherWinCnt;
        }

        gameState = EGameState.ChangeRound;
        Time.timeScale = 0.2f;
        EventManager.TriggerEvent(EGameEvent.RoundWin, new object[] { winCnt, isMaster });
    }
}
