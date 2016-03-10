using UnityEngine;
using System.Collections;

public class ConnectFourMinigame : Minigame
{
    [SerializeField] private GameObject PlayerViewA;
    [SerializeField] private GameObject PlayerViewB;

    protected override void LocalPlayerJoin(PhotonPlayer player, MinigameTeam team)
    {
        // Do walk/take away control
    }

    protected override void LocalPlayerLeave()
    {
        // Do walk/give control back
    }

    protected override void StartGame(PhotonMessageInfo info)
    {
        // Pure start, checks have already been handled
    }
}
