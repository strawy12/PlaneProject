using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    private GameObject[,] _mapGrid = new GameObject[12,4];
    [SerializeField]
    private List<Block> _mapObjectList;

    [SerializeField]
    private List<Item> _itemObjectList;

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(MakeMap());
        }
    }

    private IEnumerator MakeMap()
    {
        while (true)
        {
            List<Block> list = new List<Block>();
            for (int i = 0; i < 10; i++)
            {
                int x = Random.Range(0, 12);
                int y = Random.Range(0, 4);

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
            yield return new WaitForSeconds(10f);
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

        int idx = Random.Range(0, 3);
        Item item = NetworkManager.Inst.SpawnObject(_itemObjectList[idx], transform);
        _mapGrid[x, y] = item.gameObject;
        item.CurrentPhotonView.RPC("SpawnItem", RpcTarget.All, x, y);
        item.OnDestroy += () => _mapGrid[x, y] = null;
    }
}
