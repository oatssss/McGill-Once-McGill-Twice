using UnityEngine;
using System.Collections;
using System;

public class ConnectFourMinigame : Minigame
{
    public MinigameTeam TeamA { get; protected set; }
    public MinigameTeam TeamB  { get; protected set; }
    [SerializeField] private GameObject PlayerViewA;
    [SerializeField] private GameObject PlayerViewB;
    [SerializeField] private ConnectFourBoard Board;

    protected override void Awake()
    {
        base.Awake();
        this.TeamA = new MinigameTeam(0, 1);
        this.TeamB = new MinigameTeam(1, 1);
        this.AddTeam(this.TeamA);
        this.AddTeam(this.TeamB);
    }

    protected override void LocalPlayerJoin(MinigameTeam team)
    {
        // Handle UI
        base.LocalPlayerJoin(team);

        CameraManager.SetViewPosition(this.Board.transform.position);

        // Join as A
        if (team == this.TeamA)
        {
            Vector3 directionA = this.Board.transform.position - this.PlayerViewA.transform.position;
            CameraManager.SetViewForwardImmediate(directionA);
            CameraManager.SetPivotRadius(directionA.magnitude);
        }
        // Otherwise join as B
        else
        {
            Vector3 directionB = this.Board.transform.position - this.PlayerViewB.transform.position;
            CameraManager.SetViewForwardImmediate(directionB);
            CameraManager.SetPivotRadius(directionB.magnitude);
        }
    }

    protected override void LocalPlayerLeave()
    {
        // Probably don't need to override

        base.LocalPlayerLeave();
    }

    protected override void HandleRemotePlayerLeaveDetails(MinigameTeam team, PhotonPlayer player)
    {
        if (this.Board.Playing)
        {
            this.Board.EndGame();
            this.ReturnToMinigameLobby();
            this.DisplayEarlyGameTermination(team, player);
        }
    }

    private void DisplayEarlyGameTermination(MinigameTeam team, PhotonPlayer player)
    {
        throw new NotImplementedException();
    }

    protected override void StartGame(PhotonMessageInfo info)
    {
        base.StartGame(info);
        // Pure start, checks and UI have already been handled

        this.Board.StartGame(this.TeamA.GetEnumerator().Current, this.TeamB.GetEnumerator().Current);
    }
}
