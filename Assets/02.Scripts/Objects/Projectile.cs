using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private LayerMask _blockLayer;

    private float _moveSpeed;
    private float _attackDamage;
    private bool isFire = false;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Light2D _light;

    [SerializeField]
    private List<Color> _lightColorList;

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
        if (CurrentPhotonView.IsMine == false) return;
        if (transform.position.y <= Define.MIN_POS.y
        || transform.position.y >= Define.MAX_POS.y)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Inst.gameState == GameManager.EGameState.ChangeRound) return;
        if (CurrentPhotonView.IsMine == false) return;

        IHitable hit = collision.GetComponent<IHitable>();
        if (hit != null)
        {
            if (hit is Player)
            {
                if ((hit as Player).CurrentPhotonView.IsMine)
                    return;
            }

            SpawnExplosionEffect();
            hit.OnHit((int)_attackDamage);
            PhotonNetwork.Destroy(gameObject);
        }

    }
    private void SpawnExplosionEffect()
    {
        Debug.DrawRay(transform.position, transform.up, Color.red, 10f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 10f, 1 << _blockLayer);

        if (hit.collider != null)
        {
            int idx = Define.CheckMasterAndMine(CurrentPhotonView) ? 1 : 2;
            PhotonNetwork.Instantiate($"ExplosionEffect_{idx}", hit.point, Quaternion.identity);
        }

        EventManager.TriggerEvent(EGameEvent.Explosion);
    }
    [PunRPC]
    public void Fire(float attackDamage, float moveSpeed)
    {
        int idx;
        if (Define.CheckMasterAndMine(CurrentPhotonView))
        {
            idx = 3;
            _light.color = _lightColorList[0];
        }

        else
        {
            idx = 2;
            _light.color = _lightColorList[1];
        }
        _spriteRenderer.sprite = Resources.LoadAll<Sprite>($"Characters-export")[idx];
        _attackDamage = attackDamage;
        _moveSpeed = moveSpeed;
        isFire = true;
    }
}
