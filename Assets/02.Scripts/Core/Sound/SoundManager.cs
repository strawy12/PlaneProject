using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    private List<SoundPlayer> _soundPlayerTempList;

    private List<SoundPlayer> _soundPlayerList;
    private Dictionary<ESoundType, Queue<SoundPlayer>> _soundPlayerPool;

    private void Awake()
    {
        _soundPlayerPool = new Dictionary<ESoundType, Queue<SoundPlayer>>();
        _soundPlayerList = new List<SoundPlayer>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(ESoundType type)
    {
        SoundPlayer player = GetSoundPlayer(type);
        player.gameObject.SetActive(true);
        _soundPlayerList.Add(player);
        player.Play();
    }

    public void StopSound(ESoundType type)
    {
        List<SoundPlayer> playerList = _soundPlayerList.FindAll(x => x.soundType == type);
        playerList.ForEach(x => x.Stop());
    }

    private SoundPlayer GetSoundPlayer(ESoundType type)
    {
        if(_soundPlayerPool.ContainsKey(type))
        {
            if(_soundPlayerPool[type].Count > 0)
            {
                return _soundPlayerPool[type].Dequeue();
            }
        }
        return CreateSoundPlayer(type);
    }

    private SoundPlayer CreateSoundPlayer(ESoundType type)
    {
        SoundPlayer player = Instantiate(_soundPlayerTempList.Find(x => x.soundType == type), transform);
        player.OnCompleted += EnqueueSoundPlayer;
        return player;
    }

    private void EnqueueSoundPlayer(SoundPlayer player)
    {
        player.gameObject.SetActive(false);
        _soundPlayerList.Remove(player);

        if (!_soundPlayerPool.ContainsKey(player.soundType))
        {
            _soundPlayerPool.Add(player.soundType, new Queue<SoundPlayer>());
        }

        _soundPlayerPool[player.soundType].Enqueue(player);
    }
}
