using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[Serializable]
public class PlayerData
{
    public float moveSpeed;
    public float attackDamage;
    public float attackDelay;
    public float projectileSpeed;
    public int maxHp;
}

public class Player : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private PlayerData _playerData;

    private PhotonView _photonView;
    public PhotonView CurrentPhotonView => _photonView;

    private int _health;

    private void Awake()
    {
        _health = _playerData.maxHp;
        _photonView = GetComponent<PhotonView>();

        PlayerAttack attack = GetComponent<PlayerAttack>();
        attack.SetAttackDamage(_playerData.attackDamage);
        attack.SetProjectileSpeed(_playerData.projectileSpeed);
        attack.SetAttackDelay(_playerData.attackDelay);

        GetComponent<PlayerMove>().SetMoveSpeed(_playerData.moveSpeed);
    }

    [PunRPC]
    public void ToAttack(int damage)
    {
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, 100);

        if (_health <= 0)
        {
            if (_photonView.IsMine)
            {
                _photonView.RPC("OtherPlayerDie", RpcTarget.Others);
                GameManager.Inst.SetMyLife(-1);
            }
        }
    }



    [PunRPC]
    private void OtherPlayerDie()
    {
        GameManager.Inst.SetOtherLife(-1);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_health);
        }

        else
        {
            _health = (int)stream.ReceiveNext();
        }
    }
}
