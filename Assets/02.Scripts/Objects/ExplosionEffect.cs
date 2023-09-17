using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField]
    private Light2D _explosionLight;
    [SerializeField]
    private float _explosionLightIntensity;

    void Start()
    {
        DOVirtual.Float(0, _explosionLightIntensity, 0.05f, ChangeLight).OnComplete(() => DOVirtual.Float(_explosionLightIntensity, 0, 0.1f, ChangeLight));
    }

    void ChangeLight(float x)
    {
        _explosionLight.intensity = x;
    }
}
