using UnityEngine;
using System.Collections.Generic;
using System;

public class BeerPongMinigame : TurnBasedMinigame
{
    [SerializeField] private MinigameTeam TeamA;
    [SerializeField] private MinigameTeam TeamB;
    [SerializeField] private List<PongBall> PongBalls;
    [SerializeField] private List<PongCup> CupsA;
    [SerializeField] private List<PongCup> CupsB;
    [SerializeField] private List<PongCup> ScoredCups;

    /// <summary>
    ///  Initializes the minigame with 2 teams each consisting of 2 players maximum.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        this.TeamA = new MinigameTeam(0, 2);
        this.TeamB = new MinigameTeam(1, 2);
        this.AddTeam(this.TeamA);
        this.AddTeam(this.TeamB);
    }

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

    protected override void StartGame(PhotonMessageInfo info)
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
}