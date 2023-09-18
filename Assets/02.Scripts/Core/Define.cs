using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public const string HORIZONTAL = "Horizontal";
    public const string LOBBY_SCENE = "Lobby";
    public const string GAME_SCENE = "Game";
    public const string ROOMNAME = "Gomble";
    public const string PROJECTILE = "Projectile";
    public readonly static Vector2 MIN_POS = new Vector2(-4.3f, -7.65f);
    public readonly static Vector2 MAX_POS = new Vector2(4.3f, 5f);

    public const float BLOCK_SPAWN_DELAY = 30f;

    public static bool CheckMasterAndMine(PhotonView photonView)
    {
        return PhotonNetwork.IsMasterClient == photonView.IsMine;
    }
}
