using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    private Image _guageTemp;
    private List<Image> _gaugeImageList = new List<Image>();

    private bool _isInit = false;

    public void Init(int maxHp)
    {
        if (_isInit) return;
        _isInit = true;

        for (int i = 0; i < maxHp; i++)
        {
            Image gauge = Instantiate(_guageTemp, _guageTemp.transform.parent);
            _gaugeImageList.Add(gauge);
            gauge.gameObject.SetActive(true);
        }
    }

    public void SetGauge(int hp, int armor)
    {
        for (int i = 0; i < _gaugeImageList.Count; i++)
        {
            _gaugeImageList[i].gameObject.SetActive(i < hp);
            _gaugeImageList[i].transform.GetChild(0).gameObject.SetActive(i < armor);
        }
    }
}
