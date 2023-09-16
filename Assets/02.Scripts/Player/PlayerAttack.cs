using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Transform _firePos;
    private float _attackDamage;
    private float _attackDelay;
    private float _projectileSpeed;

    private bool _isWaitDelay;

    private Player _currentPlayer;

    private void Awake()
    {
        GetComponent<PlayerInput>().OnAttack += OnAttack;
        _currentPlayer = GetComponent<Player>();
    }

    public void OnAttack()
    {
        if (_isWaitDelay) return;
        _isWaitDelay = true;

        Projectile projectile = PhotonNetwork.Instantiate(Define.PROJECTILE, _firePos.position, transform.rotation).GetComponent<Projectile>();
        projectile.CurrentPhotonView.RPC("Fire", RpcTarget.All, _attackDamage, _projectileSpeed);
        StartCoroutine(AttackDelay());
    }

    public IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(_attackDelay);  

        _isWaitDelay = false;
    }


    public  void SetAttackDamage(float attackDamage)
    {
        _attackDamage = attackDamage; 
    }

    public void SetAttackDelay(float attackDelay)
    {
        _attackDelay = attackDelay;
    }

    public void SetProjectileSpeed(float projectileSpeed)
    {
        _projectileSpeed = projectileSpeed;
    }

}
