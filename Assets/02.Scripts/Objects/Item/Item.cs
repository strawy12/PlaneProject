using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Item : MonoBehaviour, IHitable
{
    public enum EItemType
    {
        HealthKit,
        ArmorKit,
        CooldownKit,
    }

    [SerializeField]
    private EItemType _itemType;

    [SerializeField]
    private Collider2D _collider;

    [Header("SpawnEffect")]
    [SerializeField]
    private float _duration = 1f;

    [SerializeField]
    private float _minIntensity = 0.3f;

    [SerializeField]
    private float _delay = 0.3f;
    public Vector2Int currentIdx { get; private set; }
    public Action OnDestroy;
    private Light2D _currentLight;

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

    [PunRPC]
    public void SpawnItem(int x, int y)
    {
        _currentLight ??= transform.Find("Light").GetComponent<Light2D>();
        _photonView ??= GetComponent<PhotonView>();

        currentIdx = new Vector2Int(x, y);
        SetCoord();

        StartCoroutine(SpawnEffect());
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

    private IEnumerator SpawnEffect()
    {
        while (true)
        {
            DOTween.To(() => _currentLight.intensity,
                (x) => _currentLight.intensity = x,
                1f, _duration);

            yield return new WaitForSeconds(_duration + _delay);

            DOTween.To(() => _currentLight.intensity,
                (x) => _currentLight.intensity = x,
                _minIntensity, _duration);

            yield return new WaitForSeconds(_duration + _delay);
        }
    }

    public void OnHit(int damage)
    {
        CurrentPhotonView.RPC("UseItem", RpcTarget.All, PhotonNetwork.IsMasterClient);
    }

    [PunRPC]
    private void UseItem(bool isMaster)
    {
        _collider.enabled = false;
        EventManager.TriggerEvent(EGameEvent.UseItem, new object[] { _itemType, isMaster });
        DestroyItem();
    }

    public void Hide()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(CurrentPhotonView);
        }
    }
    protected void DestroyItem()
    {
        OnDestroy?.Invoke();
        Hide();
    }
}
