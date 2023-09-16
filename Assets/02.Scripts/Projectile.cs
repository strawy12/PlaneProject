using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _moveSpeed;
    private float _attackDamage;
    private bool isFire = false;

    private PhotonView _photonView;
    public PhotonView CurrentPhotonView
    {
        get
        {
            if (_photonView == null)
            {
                _photonView = GetComponent<PhotonView>();
            }

            return _photonView;
        }
    }

    private void Update()
    {
        if (isFire == false) return;

        transform.position += (transform.up * _moveSpeed * Time.deltaTime);
        CheckDestroy();
    }

    private void CheckDestroy()
    {
        if (_photonView.IsMine == false) return;
        if (transform.position.y <= Define.MIN_POS.y
        || transform.position.y >= Define.MAX_POS.y)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_photonView.IsMine == false) return;
        if (collision.tag == Define.PLAYER_TAG)
        {
            collision.GetComponent<Player>().CurrentPhotonView.RPC("ToAttack", RpcTarget.All, (int)_attackDamage);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void Fire(float attackDamage, float moveSpeed)
    {
        _attackDamage = attackDamage;
        _moveSpeed = moveSpeed;
        isFire = true;
    }
}
