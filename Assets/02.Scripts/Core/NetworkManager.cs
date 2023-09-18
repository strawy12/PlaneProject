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

    private int _nicknameIndex = 0;

    public bool dontLeaveRoom = false;

    private void Awake()
    {
       if(FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Destroy(gameObject);
        }

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
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
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
        EventManager.TriggerEvent(ENetworkEvent.CreateRoom);
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(CrossCheckRoom());
    }

    public void LeaveRoom()
    {
        if(SceneManager.GetActiveScene().name != Define.LOBBY_SCENE)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            SceneManager.LoadScene(Define.LOBBY_SCENE);
        }

        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Rename();
        EventManager.TriggerEvent(ENetworkEvent.JoinedRoom);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
    {
        if (dontLeaveRoom) return;
        LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
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
        float time = 30f;
        yield return new WaitForSeconds(time);
        LeaveRoom();
    }
}
