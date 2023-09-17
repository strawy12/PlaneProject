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

public class Player : MonoBehaviour, IPunObservable, IHitable
{
    [SerializeField]
    private PlayerData _playerData;

    private PhotonView _photonView;
    private SpriteRenderer _spriteRenderer;

    public PhotonView CurrentPhotonView => _photonView;

    public Action<int, int, int> OnChangeHp;

    private int _health;
    private int _armorAmount;

    private void Awake()
    {
        _health = _playerData.maxHp;
        _armorAmount = 0;
        _photonView = GetComponent<PhotonView>();
        _spriteRenderer = transform.Find("VisualSprite").GetComponent<SpriteRenderer>();

        PlayerAttack attack = GetComponent<PlayerAttack>();
        attack.SetAttackDamage(_playerData.attackDamage);
        attack.SetProjectileSpeed(_playerData.projectileSpeed);
        attack.SetAttackDelay(_playerData.attackDelay);

        GetComponent<PlayerMove>().SetMoveSpeed(_playerData.moveSpeed);

        EventManager.StartListening(EGameEvent.UseItem, Useitem);
    }

    private void Start()
    {
        int idx = Define.CheckMasterAndMine(CurrentPhotonView) ? 0 : 1;
        _spriteRenderer.sprite = Resources.LoadAll<Sprite>($"Characters-export")[idx];
        OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);
    }

    [PunRPC]
    public void ToAttack(int damage)
    {
        if (_armorAmount > 0)
        {
            _armorAmount--;
            OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);
            if(CurrentPhotonView.IsMine)
                GameManager.Inst.ShakeCamera(0.3f, 0.1f, 20);
            return;
        }
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, _playerData.maxHp);
        OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);

        if (CurrentPhotonView.IsMine)
            GameManager.Inst.ShakeCamera(0.5f, 0.3f, 30);

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

    public void OnHit(int damage)
    {
        _photonView.RPC("ToAttack", RpcTarget.All, damage);
    }

    private void Useitem(object[] ps)
    {
        if (ps.Length != 2)
            return;

        Item.EItemType type = (Item.EItemType)ps[0];
        bool isMine = (bool)ps[1];

        if (isMine != CurrentPhotonView.IsMine)
            return;


        switch (type)
        {
            case Item.EItemType.HealthKit:
                _health++;
                _health = Mathf.Clamp(_health, 0, _playerData.maxHp);
                OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);
                break;

            case Item.EItemType.ArmorKit:
                _armorAmount++;
                _armorAmount = Mathf.Clamp(_armorAmount, 0, _playerData.maxHp);
                OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);
                break;

            case Item.EItemType.CooldownKit:
                // 스킬 쿨타임 초기화
                break;
        }
    }
}
