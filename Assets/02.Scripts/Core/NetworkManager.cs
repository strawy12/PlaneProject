using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager _inst;

    public static NetworkManager Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = FindObjectOfType<NetworkManager>();
            }

            return _inst;
        }
    }

    [SerializeField]
    private GameObject _lodingPanel;


    private int _nicknameIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    private void Start()
    {
        Connect();
    }

    public void Connect()
    {
        Debug.Log(PhotonNetwork.ConnectUsingSettings());
        EventManager.TriggerEvent(ENetworkEvent.Connecting);
;        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //EventManager.TriggerEvent(ENetworkEvent.Connected);
        Debug.Log("dd");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("ddd");
        EventManager.TriggerEvent(ENetworkEvent.JoinedLobby);
    }

    public void DisConnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void QuickMatch()
    {
        Debug.Log(PhotonNetwork.InRoom);

        if (PhotonNetwork.InRoom)
            return;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }
    public void CreateRoom()
    {
        Debug.Log("Create");

        EventManager.TriggerEvent(ENetworkEvent.CreateRoom);
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(CrossCheckRoom());
    }

    public void LeaveRoom()
    {
        Debug.Log("LeaveRoom");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("JoinedRoom");
        PhotonNetwork.AutomaticallySyncScene = true;
        Rename();
        EventManager.TriggerEvent(ENetworkEvent.JoinedRoom);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
    {
        LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            StopAllCoroutines();
            StartCoroutine(Startgame());
        }
    }

    private void Rename()
    {
        if (PhotonNetwork.PlayerListOthers.Length != 0)
        {
            while (PhotonNetwork.PlayerListOthers[0].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                PhotonNetwork.LocalPlayer.NickName = $"player{_nicknameIndex.ToString()}";
                _nicknameIndex++;
            }
        }

        else
        {
            PhotonNetwork.LocalPlayer.NickName = $"player{_nicknameIndex.ToString()}";
            _nicknameIndex++;
        }
    }

    public T SpawnObject<T>(T gobj, Transform parent) where T : MonoBehaviour
    {
        return SpawnObject<T>(gobj.name, parent);
    }

    public T SpawnObject<T>(string name, Transform parent) where T : MonoBehaviour
    {
        T obj = PhotonNetwork.Instantiate(name, new Vector3(0, 0, -1), Quaternion.identity).GetComponent<T>();
        obj.transform.parent = parent;
        return obj;
    }

    private IEnumerator Startgame()
    {
        yield return new WaitForSeconds(5f);
        PhotonNetwork.LoadLevel(Define.GAME_SCENE);
    }

    private IEnumerator CrossCheckRoom()
    {
        float time = Random.Range(5f, 10f);
        yield return new WaitForSeconds(time);
        LeaveRoom();
        Debug.Log("다시 스타트 누르세요");
    }
}
