using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private HPBar _hpBar; 

    [SerializeField]
    private CanvasGroup _root;

    private Player _currentPalyer;

    private bool isShow = false;

    private void Awake()
    {
        _currentPalyer = GetComponent<Player>();
        _currentPalyer.OnChangeHp += SetHpBarGauge;
        _currentPalyer.OnChangeHp += (a,b,c) => Show();
    }

    private void Start()
    {
        SetNameText(PhotonNetwork.NickName);

        if (!_currentPalyer.CurrentPhotonView.IsMine)
        {
            _root.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }
    }

    public void Show()
    {
        if (isShow)
        {
            StopAllCoroutines();
            StartCoroutine(HideDelay());
            return;
        }
        isShow = true;
        _root.DOKill();
        _root.DOFade(1f, 1f);
    }

    private IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(4f);
        _root.DOFade(0f, 1f);
        isShow = false;
    }

    public void SetHpBarGauge(int hp, int maxHp, int armor)
    {
        _hpBar.Init(maxHp);
        _hpBar.SetGauge(hp, armor);
    }

    private void SetNameText(string name)
    {
        _nameText.text = name;

        if (_currentPalyer.CurrentPhotonView.IsMine)
        {
            _nameText.color = Color.green;
        }

        else
        {
            _nameText.color = Color.red;
        }
    }
}
