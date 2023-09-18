using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Block : MonoBehaviour, IHitable
{
    [SerializeField]
    private TMP_Text _lifeText;

    [SerializeField]
    private Collider2D _collider;

    public Vector2Int currentIdx { get; private set; }
    public Action<Block> OnDestroy;

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

    private int _currentHp = 3;
    private int _maxHp = 3;
    private bool _isHiding = false;
    private bool _isDestroyed = false;


    private void Start()
    {
        _maxHp = 3;
        _currentHp = _maxHp;
        SetLifeText();
    }

    public void SetCoord()
    {
        Vector2 newPos = transform.position;
        newPos.x = currentIdx.x * 1.25f;
        newPos.y = currentIdx.y * 1.25f;
        newPos.x += -4.375f;
        newPos.y += -5.625f;

        transform.position = newPos;

        if (PhotonNetwork.IsMasterClient == false)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
    }

    [PunRPC]
    public void SetIdx(int x, int y)
    {
        currentIdx = new Vector2Int(x, y);
    }

    [PunRPC]
    public void Show()
    {
        transform.localScale = Vector3.zero;
        SetCoord();

        transform.DOScale(Vector3.one * 1.25f, 1f).SetEase(Ease.OutElastic);
    }

    [PunRPC]
    public void Hide()
    {
        if (_isHiding) return;
        _isHiding = true;

        transform.DOScale(Vector3.zero, 1f).OnComplete(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (_isDestroyed && Random.Range(0, 3) == 0)
                {
                    MapManager.Inst.SpawnItem(currentIdx.x, currentIdx.y);
                }
                
                PhotonNetwork.Destroy(gameObject);
            }
        });
    }

    public void SetLifeText()
    {
        _lifeText.text = $"{_currentHp}/{_maxHp}";
    }

    public void OnHit(int damage)
    {
        if (_currentHp <= 0) return;
        SoundManager.Inst.PlaySound(ESoundType.BlockHit);
        CurrentPhotonView.RPC("Hit_RPC", RpcTarget.All);
    }

    [PunRPC]
    private void Hit_RPC()
    {
        _currentHp--;
        SetLifeText();
        if (_currentHp <= 0)
        {
            _collider.enabled = false;
            _isDestroyed = true;
            DestroyBlock();
        }
    }

    private void DestroyBlock()
    {
        SoundManager.Inst.PlaySound(ESoundType.BlockDestroy);
        OnDestroy?.Invoke(this);
        Hide();
    }
}
