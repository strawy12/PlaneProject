using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

[Serializable]
public class PlayerData
{
    public float moveSpeed;
    public float attackDamage;
    public float attackDelay;
    public float projectileSpeed;
    public int maxHp;
    public float invincibleTime;
}

public class Player : MonoBehaviour, IPunObservable, IHitable
{
    [SerializeField]
    private PlayerData _playerData;

    [SerializeField]
    private Collider2D _collider;

    [SerializeField]
    private PlayerUI _playerUI;

    private PhotonView _photonView;
    private SpriteRenderer _spriteRenderer;

    public PhotonView CurrentPhotonView => _photonView;

    public Action<int, int, int> OnChangeHp;

    private int _health;
    private int _armorAmount;


    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _spriteRenderer = transform.Find("VisualSprite").GetComponent<SpriteRenderer>();

        EventManager.StartListening(EGameEvent.UseItem, Useitem);
        EventManager.StartListening(EGameEvent.StartRound, Init);
    }

    private void Start()
    {
        PlayerAttack attack = GetComponent<PlayerAttack>();
        attack.SetAttackDamage(_playerData.attackDamage);
        attack.SetProjectileSpeed(_playerData.projectileSpeed);
        attack.SetAttackDelay(_playerData.attackDelay);

        GetComponent<PlayerMove>().SetMoveSpeed(_playerData.moveSpeed);

        int idx = Define.CheckMasterAndMine(CurrentPhotonView) ? 0 : 1;
        _spriteRenderer.sprite = Resources.LoadAll<Sprite>($"Characters-export")[idx];
    }

    private void OnDestroy()
    {
        EventManager.StopListening(EGameEvent.UseItem, Useitem);
    }

    public void Init(object[] ps)
    {
        _health = _playerData.maxHp;
        _armorAmount = 0;
        OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);
        UIManager.Inst.SetPlayerLifeText(_health, _playerData.maxHp, CurrentPhotonView.IsMine);
    }

    [PunRPC]
    public void ToAttack(int damage)
    {
        StartCoroutine(InvincibleDelay());

        if (_armorAmount > 0)
        {
            _armorAmount--;
            OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);

            if (CurrentPhotonView.IsMine)
            {
                GameManager.Inst.ShakeCamera(0.3f, 0.1f, 20);
            }
            return;
        }
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, _playerData.maxHp);
        OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);
        EventManager.TriggerEvent(EGameEvent.AttackedPlayer, new object[] { _health, _playerData.maxHp, CurrentPhotonView.IsMine });

        if (CurrentPhotonView.IsMine)
        {
            GameManager.Inst.ShakeCamera(0.5f, 0.3f, 30);
            UIManager.Inst.StartHitEffect();
        }

        if (_health <= 0)
        {
            if (!_photonView.IsMine)
            {
                GameManager.Inst.CurrentPhotonView.RPC("RoundWin", RpcTarget.All, PhotonNetwork.IsMasterClient);
            }
        }
    }

    public IEnumerator InvincibleDelay()
    {
        if (_playerData.invincibleTime == 0f)
        {
            _playerData.invincibleTime = 3f;
        }
        _collider.enabled = false;
        float divideTime = _playerData.invincibleTime / 6f;

        for (int i = 0; i < 3; i++)
        {
            _spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(divideTime);
            _spriteRenderer.color = new Color(1, 1, 1, 0.8f);
            yield return new WaitForSeconds(divideTime);
        }

        _spriteRenderer.color = new Color(1, 1, 1, 1f);
        _collider.enabled = true;
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
        bool isMaster = (bool)ps[1];

        if ((isMaster == PhotonNetwork.IsMasterClient) != CurrentPhotonView.IsMine)
            return;

        switch (type)
        {
            case Item.EItemType.HealthKit:
                _health++;
                _health = Mathf.Clamp(_health, 0, _playerData.maxHp);
                OnChangeHp?.Invoke(_health, _playerData.maxHp, _armorAmount);
                EventManager.TriggerEvent(EGameEvent.AttackedPlayer, new object[] { _health, _playerData.maxHp, CurrentPhotonView.IsMine });
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
