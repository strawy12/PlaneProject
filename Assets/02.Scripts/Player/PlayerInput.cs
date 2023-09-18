using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Action<float> OnMove;
    public Action OnAttack;

    private Player _currentPalyer;

    private void Awake()
    {
        _currentPalyer = GetComponent<Player>();
    }

    private void Update()
    {
        if (_currentPalyer == null) return;
     
        InputMove();
        InputAttack();
    }

    private void InputMove()
    {
        OnMove?.Invoke(Input.GetAxisRaw(Define.HORIZONTAL));
    }

    private void InputAttack()
    {
        if (_currentPalyer.CurrentPhotonView.IsMine == false) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnAttack?.Invoke();
        }
    }

}
