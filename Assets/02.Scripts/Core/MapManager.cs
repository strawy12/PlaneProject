using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    private GameObject[,] _mapGrid = new GameObject[8,8];
    [SerializeField]
    private List<Block> _mapObjectList;

    [SerializeField]
    private List<Item> _itemObjectList;

    private void Awake()
    {
        EventManager.StartListening(EGameEvent.StartRound, MakeMap);
    }


    public void MakeMap(object[] ps)
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        ResetMap();
        StartCoroutine(MakeMapCo());
    }

    private void ResetMap()
    {
        StopAllCoroutines();
        for(int x = 0; x < 8; x++)
        {
            for(int  y = 0; y < 8; y++)
            {
                if(_mapGrid[x, y] != null)
                {
                    _mapGrid[x, y].transform.DOKill();
                    PhotonNetwork.Destroy(_mapGrid[x, y]);
                    _mapGrid[x, y] = null;
                }
            }
        }
    }

    private IEnumerator MakeMapCo()
    {
        while (true)
        {
            UIManager.Inst.CurrentPhotonView.RPC("MakeBlock", RpcTarget.All);

            List<Block> list = new List<Block>();
            for (int i = 0; i < 10; i++)
            {
                int x = Random.Range(0, 8);
                int y = Random.Range(0, 8);

                int idx = Random.Range(0, 3);

                if (_mapGrid[x, y] == null)
                {
                    Block block = NetworkManager.Inst.SpawnObject(_mapObjectList[idx], transform);
                    _mapGrid[x, y] = block.gameObject;
                    block.CurrentPhotonView.RPC("SetIdx", RpcTarget.All,x,y);
                    block.OnDestroy += (a) => list.Remove(a);
                    block.OnDestroy += (a) => _mapGrid[x, y] = null;
                    list.Add(block);
                }

                else
                {
                    i--;
                }
            }

            list.ForEach(x => x.CurrentPhotonView.RPC("Show", RpcTarget.All));
            yield return new WaitForSeconds(Define.BLOCK_SPAWN_DELAY);
            list.ForEach(x => 
            {
                Vector2Int idx = x.currentIdx;
                _mapGrid[idx.x, idx.y] = null;
                x.CurrentPhotonView.RPC("Hide", RpcTarget.All);
            });
        }
    }

    public void SpawnItem(int x, int y)
    {
        if (_mapGrid[x, y] != null)
            return;

        int idx = Random.Range(0, 2);
        Item item = NetworkManager.Inst.SpawnObject(_itemObjectList[idx], transform);
        _mapGrid[x, y] = item.gameObject;
        item.CurrentPhotonView.RPC("SpawnItem", RpcTarget.All, x, y);
        item.OnDestroy += () => _mapGrid[x, y] = null;
    }
}
