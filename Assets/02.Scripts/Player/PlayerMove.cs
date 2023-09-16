using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private float _moveSpeed;

    private void Start()
    {
        GetComponent<PlayerInput>().OnMove += OnMove;
    }

    public void OnMove(float horizontal)
    {
        transform.Translate(Vector3.right * horizontal * _moveSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, Define.MIN_POS_X, Define.MAX_POS_X);

        transform.position = pos;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

}
