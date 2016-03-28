using UnityEngine;
using System.Collections.Generic;
using System;

public class BeerPongMinigame : TurnBasedMinigame
{
    public MinigameTeam TeamA;
    public MinigameTeam TeamB;
    [SerializeField] private List<PongBall> PongBalls;
    [SerializeField] private List<PongCup> CupsA;
    [SerializeField] private List<PongCup> CupsB;
    [SerializeField] private List<PongCup> ScoredCups;

    /*
    /// <summary>
    ///  An operation that does...
    /// </summary>
    [PunRPC]
    public void SetTurnA()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///  An operation that does...
    /// </summary>
    [PunRPC]
    public void SetTurnB()
    {
        throw new NotImplementedException();
    }
    */

    /// <summary>
    ///  RPC called by the client that threw once the ball has stopped moving on their end.
    /// </summary>
    /// <param name="info"> The info provided by the client who threw.
    /// </param>
    [PunRPC]
    protected void FinishThrow(PhotonMessageInfo info)
    {
        throw new NotImplementedException();
    }

    protected override bool StartGame(PhotonMessageInfo info)
    {
        throw new NotImplementedException();
    }

    protected override void LocalPlayerJoin(MinigameTeam team)
    {
        throw new NotImplementedException();
    }

    protected override void LocalPlayerLeave()
    {
        throw new NotImplementedException();
    }

    protected override void HandleRemotePlayerLeaveDetails(MinigameTeam team, PhotonPlayer player)
    {
        throw new NotImplementedException();
    }

    protected override bool ValidToStart()
    {
        throw new NotImplementedException();
    }
}