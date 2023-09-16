using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour, IPunObservable
{
    private float _moveSpeed;
    private Player _currentPlayer;

    private Vector3 _currentPos;

    private void Start()
    {
        _currentPlayer = GetComponent<Player>();

        if (_currentPlayer.CurrentPhotonView.IsMine)
        { 
            GetComponent<PlayerInput>().OnMove += OnMove;
        }

        else
        {
            GetComponent<PlayerInput>().OnMove += (a) => SyncCurrentPos();
        }
    }

    public void OnMove(float horizontal)
    {
        transform.Translate(Vector3.right * horizontal * _moveSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, Define.MIN_POS.x, Define.MAX_POS.x);

        transform.position = pos;

    }

    private void SyncCurrentPos()
    {
        if (Mathf.Abs(_currentPos.x - transform.position.x) > 5f)
        {
            transform.position = _currentPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _currentPos, Time.deltaTime * _moveSpeed * 3);
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, Define.MIN_POS.x, Define.MAX_POS.x);

        transform.position = pos;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }

        else
        {
            _currentPos = (Vector3)stream.ReceiveNext();
        }
    }
}
