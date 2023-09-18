using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    private Image _guageTemp;
    private List<Image> _gaugeImageList = new List<Image>();

    private bool isInit = false;

    public void Init(int maxHp)
    {
        if (isInit) return;
        isInit = true;

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
            _gaugeImageList[i].color = (i < hp) ? new Color(1,0,0,1) : new Color(1, 0, 0, 0);
            _gaugeImageList[i].transform.GetChild(0).gameObject.SetActive(i < armor);
        }
    }
}
