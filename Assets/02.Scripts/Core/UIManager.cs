using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private TMP_Text _roundText;

    [SerializeField]
    private TMP_Text _blockSpawnTimeText;

    [SerializeField]
    private TMP_Text _masterLifeText;

    [SerializeField]
    private TMP_Text _otherLifeText;

    [SerializeField]
    private List<Image> _masterRoundWinUI;
    [SerializeField]
    private List<Image> _otherRoundWinUI;

    [SerializeField]
    private Image _blackPanel;

    [SerializeField]
    private TMP_Text _noticeText;

    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private Image _hitEffect;

    [SerializeField]
    private Button _goLobbyBtn;

    private Color _defaultColor = new Color(0, 0, 0, 0.4f);
    private Color _otherColor = new Color(0, 1, 0, 0.7f);
    private Color _masterColor = new Color(1, 0, 0, 0.7f);

    private Action<object[]> OnMakeBlock;
    private Action<object[]> OnStartRound;
    private Action<object[]> OnRoundWin;
    private void Awake()
    {
        OnMakeBlock += (a) => StopAllCoroutines();
        OnMakeBlock += (a) => StartCoroutine(SetBlockSpawnTimeTextCo());

        OnStartRound += SetRoundText;

        OnRoundWin += SetRoundWinUI;
        OnRoundWin += (a) => StartCoroutine(RoundEndCo(a));

        _goLobbyBtn.onClick.AddListener(() => NetworkManager.Inst.LeaveRoom());

        EventManager.StartListening(EGameEvent.StartRound, OnStartRound);
        EventManager.StartListening(EGameEvent.MakeBlock, OnMakeBlock);
        EventManager.StartListening(EGameEvent.AttackedPlayer, SetPlayerLifeText);
        EventManager.StartListening(EGameEvent.RoundWin, OnRoundWin);
    }

    public void Init()
    {
        SetRoundWinUI(0, true);
        SetRoundWinUI(0, false);
    }

    private void SetRoundText(object[] ps)
    {
        if (ps.Length != 1) return;

        int nowRound = (int)ps[0];

        SetRoundText(nowRound);
    }

    public void SetRoundText(int nowRound)
    {
        _roundText.text = $"{nowRound.ToString()}<size=44>R</size>";
    }

    private void SetBlockSpawnTimeText(float time)
    {
        int minute = (int)(time / 60f);
        int second = (int)(time % 60f);

        _blockSpawnTimeText.text = $"{minute.ToString()}:{second.ToString()}";
    }

    private void SetPlayerLifeText(object[] ps)
    {
        if (ps.Length != 3) return;

        int hp = (int)ps[0];
        int maxHp = (int)ps[1];
        bool isMine = (bool)ps[2];

        SetPlayerLifeText(hp, maxHp, isMine);
    }

    public void SetPlayerLifeText(int hp, int maxHp, bool isMine)
    {
        if (isMine == PhotonNetwork.IsMasterClient)
        {
            _masterLifeText.text = $"{hp.ToString()}/{maxHp.ToString()}";
        }

        else
        {
            _otherLifeText.text = $"{hp.ToString()}/{maxHp.ToString()}";
        }
    }

    private void SetRoundWinUI(object[] ps)
    {
        if (ps.Length != 2) return;

        int winCnt = (int)ps[0];
        bool isMaster = (bool)ps[1];

        SetRoundWinUI(winCnt, isMaster);
    }

    public void SetRoundWinUI(int winCnt, bool isMaster)
    {
        if (isMaster)
        {
            for (int i = 0; i < _masterRoundWinUI.Count; i++)
            {

                _masterRoundWinUI[i].color = (i < winCnt) ? _masterColor : _defaultColor;
            }
        }

        else
        {
            for (int i = 0; i < _otherRoundWinUI.Count; i++)
            {
                _otherRoundWinUI[i].color = (i < winCnt) ? _otherColor : _defaultColor;
            }
        }
    }

    private IEnumerator SetBlockSpawnTimeTextCo()
    {
        float time = Define.BLOCK_SPAWN_DELAY;
        while (time > 0f)
        {
            time -= 1f;
            SetBlockSpawnTimeText(time);
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private IEnumerator RoundEndCo(object[] ps)
    {
        if (ps.Length != 2) yield break;

        int winCnt = (int)ps[0];
        bool isMaster = (bool)ps[1];

        _scoreText.gameObject.SetActive(false);
        _scoreText.text = "";
        _noticeText.gameObject.SetActive(false);
        FadeBlackPanel(0.6f, 1f, true);
        yield return new WaitForSecondsRealtime(1f);
        _noticeText.gameObject.SetActive(true);
        _noticeText.text = winCnt < 2 ? "Round End" : ((isMaster == PhotonNetwork.IsMasterClient) ? "<color=#00FF00>You Win" : "<color=#FF0000>You Lose");
        yield return new WaitForSecondsRealtime(3f);
        _scoreText.gameObject.SetActive(true);

        string text;
        if (PhotonNetwork.IsMasterClient)
        {
            text = $"Score\n<size=100>{GameManager.Inst.CurrentWinCnt} : {GameManager.Inst.OtherWinCnt}";
        }
        else
        {
            text = $"Score\n<size=100>{GameManager.Inst.OtherWinCnt} : {GameManager.Inst.CurrentWinCnt}";
        }

        bool isSkip = false;
        for(int i = 0; i < text.Length; i++)
        {
            if (text[i] == '<')
            {
                isSkip = true;
            }
            if (text[i] == '>')
            {
                isSkip = false;
            }
            _scoreText.text += text[i];

            if (!isSkip)
            {
                yield return new WaitForSecondsRealtime(0.04f);
            }
        }

        if(winCnt >= 2)
        {
            NetworkManager.Inst.dontLeaveRoom = true;
            _goLobbyBtn.gameObject.SetActive(true);
            yield break;
        }
        yield return new WaitForSecondsRealtime(text.Length * 0.03f + 1f);
        FadeBlackPanel(0f, 0.7f, false);
        yield return new WaitForSecondsRealtime(0.7f);


        GameManager.Inst.CurrentPhotonView.RPC("StartRound", RpcTarget.All);
        Time.timeScale = 1f;
    }

    public void FadeBlackPanel(float alpha, float duration, bool isActive)
    {
        if (isActive)
        {
            _blackPanel.gameObject.SetActive(true);
            _blackPanel.DOFade(alpha, duration).SetUpdate(true);
        }
        else
        {
            _blackPanel.DOFade(alpha, duration).SetUpdate(true).OnComplete(() =>
            {
                _blackPanel.gameObject.SetActive(false);
            });
        }
    }

    public void StartHitEffect()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_hitEffect.DOFade(1f, 0.2f));
        seq.Append(_hitEffect.DOFade(0f, 1f));
    }
}
