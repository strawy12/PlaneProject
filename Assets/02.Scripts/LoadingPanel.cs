using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class LoadingPanel : MonoBehaviour
{
    private Action<object[]> OnShow;
    private Action<object[]> OnHide;

    [SerializeField]
    private TMP_Text _loadingText;

    private bool isHide = false;

    private void Awake()
    {
        OnShow += (a) => Show();
        OnHide += (a) => Hide();
        EventManager.StartListening(ENetworkEvent.Connecting, OnShow);
        EventManager.StartListening(ENetworkEvent.JoinedLobby, OnHide);
    }

    private void OnDestroy()
    {
        EventManager.StopListening(ENetworkEvent.Connecting, OnShow);
        EventManager.StopListening(ENetworkEvent.JoinedLobby, OnHide);
    }

    public void Show()
    {
        isHide = false;
        gameObject.SetActive(true);
        StartCoroutine(LoadingText());
    }

    public void Hide()
    {
        isHide = true;
        gameObject.SetActive(false);
    }

    private IEnumerator LoadingText()
    {
        while (!isHide)
        {
            DOTween.To(
                 () => _loadingText.maxVisibleCharacters,
                 (value) => _loadingText.maxVisibleCharacters = value,
                 _loadingText.text.Length,
                 2f);
            yield return new WaitForSeconds(2f);
        }
    }
}
