using UnityEngine;
using System;

public class ConnectFourMinigame : Minigame
{
    public MinigameTeamContainer TeamContainerA;
    public MinigameTeamContainer TeamContainerB;
    [SerializeField] private GameObject PlayerSpotA;
    [SerializeField] private GameObject PlayerSpotB;
    [SerializeField] private GameObject CameraPivot;
    [SerializeField] private GameObject CameraPivotLength;
    [SerializeField] private ConnectFourBoard Board;

    protected override void LocalPlayerJoin(MinigameTeam team)
    {
        // Handle UI
        base.LocalPlayerJoin(team);

        CameraManager.SetViewPosition(this.CameraPivot.transform.position);
        CameraManager.SetViewForwardImmediate(this.Board.transform.forward);
        CameraManager.SetPivotRadius((this.CameraPivotLength.transform.position - this.CameraPivot.transform.position).magnitude);
        CameraManager.SetViewLookAngleMax(90f);

        // Join as A
        if (team == this.TeamContainerA.Team)
        {
            // Walk player to designated spot for A
            PlayerManager.GetMainPlayer(true).ThirdPersonCharacter.AIController.SetTarget(this.PlayerSpotA.transform);
        }
        // Otherwise join as B
        else
        {
            // Walk player to designated spot for B
            PlayerManager.GetMainPlayer(true).ThirdPersonCharacter.AIController.SetTarget(this.PlayerSpotB.transform);
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
