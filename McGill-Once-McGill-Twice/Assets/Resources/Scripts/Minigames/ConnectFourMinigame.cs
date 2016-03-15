using UnityEngine;
using System;

public class ConnectFourMinigame : Minigame
{
    public MinigameTeamContainer TeamContainerA;
    public MinigameTeamContainer TeamContainerB;
    [SerializeField] private GameObject PlayerViewA;
    [SerializeField] private GameObject PlayerViewB;
    [SerializeField] private ConnectFourBoard Board;

    protected override void LocalPlayerJoin(MinigameTeam team)
    {
        // Handle UI
        base.LocalPlayerJoin(team);

        CameraManager.SetViewPosition(this.Board.transform.position);

        // Join as A
        if (team == this.TeamContainerA.Team)
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

    protected override void HandleRemotePlayerLeaveDetails(MinigameTeam team, PhotonPlayer player)
    {
        if (this.Board.Playing)
        {
            this.Board.StopPlaying();
            // this.ReturnToMinigameLobby();    // Return to lobby after early game termination is dismissed
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

        this.Board.StartPlaying(this.TeamContainerA.Team.GetEnumerator().Current, this.TeamContainerB.Team.GetEnumerator().Current);
    }
}
