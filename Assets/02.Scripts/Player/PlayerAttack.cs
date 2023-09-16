using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float _attackDamage;
    private float _attackDelay;

    private bool _isWaitDelay;



    public void OnAttack()
    {
        if (_isWaitDelay) return;
        _isWaitDelay = true;



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
}
