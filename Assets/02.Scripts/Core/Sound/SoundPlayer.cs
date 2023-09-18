using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public ESoundType soundType = ESoundType.None;
    [SerializeField]
    private AudioClip _audioClip;
    [SerializeField]
    private AudioSource _audioSource;

    public Action<SoundPlayer> OnCompleted;

    private void Awake()
    {
        _audioSource ??= GetComponent<AudioSource>();
    }

    public void Play()
    {
        _audioSource.clip = _audioClip;
        _audioSource.Play();

        if(_audioSource.loop == false)
        {
            StartCoroutine(CompleteSoundDelay());
        }
    }

    private IEnumerator CompleteSoundDelay()
    {
        yield return new WaitForSeconds(_audioClip.length);
        Stop();
    }

    public void Stop()
    {
        _audioSource.Stop();
        OnCompleted?.Invoke(this);
    }

}
