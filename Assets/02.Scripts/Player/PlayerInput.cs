using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Action<float> OnMove;
    public Action OnAttack;

    private void Update()
    {
        InputMove();
        InputAttack();
    }

    private void InputMove()
    {
        OnMove?.Invoke(Input.GetAxisRaw(Define.HORIZONTAL));
    }

    private void InputAttack()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            OnAttack?.Invoke();
        }
    }
}
