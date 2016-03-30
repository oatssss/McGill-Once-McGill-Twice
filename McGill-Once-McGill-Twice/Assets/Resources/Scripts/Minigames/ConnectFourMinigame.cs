using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ConnectFourMinigame : Minigame
{
    public MinigameTeamContainer TeamContainerA;
    public MinigameTeamContainer TeamContainerB;
    [SerializeField] private GameObject PlayerSpotA;
    [SerializeField] private GameObject PlayerSpotB;
    [SerializeField] private GameObject CameraPivot;
    [SerializeField] private GameObject CameraPivotLength;
    [SerializeField] private ConnectFourBoard Board;
    public ConnectFourUI C4PlayingUI;
    public override Overlay PlayingUI {
        get { return (Overlay)this.C4PlayingUI; }
    }

    protected override void LocalPlayerJoin(MinigameTeam team)
    {
        // Handle UI
        base.LocalPlayerJoin(team);

        bool onTeamA = team == this.TeamContainerA.Team;
        CameraManager.SetViewPosition(this.CameraPivot.transform.position);

        Vector3 viewForward = this.Board.transform.forward;

        if (!this.Board.SingleSided && !onTeamA)
            { viewForward = -viewForward; }

        CameraManager.SetViewForwardImmediate(viewForward);
        CameraManager.SetPivotRadius((this.CameraPivotLength.transform.position - this.CameraPivot.transform.position).magnitude);
        // CameraManager.SetViewLookAngleMax(90f); // Do this once the game starts

        // Join as A
        if (onTeamA)
        {
            // Walk player to designated spot for A
            PlayerManager.GetMainPlayer().ThirdPersonCharacter.AIController.SetTarget(this.PlayerSpotA.transform);
        }
        // Otherwise join as B
        else
        {
            // Walk player to designated spot for B
            PlayerManager.GetMainPlayer().ThirdPersonCharacter.AIController.SetTarget(this.PlayerSpotB.transform);
        }
    }

    protected override void HandleRemotePlayerLeaveDetails(MinigameTeam team, PhotonPlayer player)
    {
        this.DisplayEarlyGameTermination(team, player);
        this.ReturnToMinigameLobby();
    }

    protected override void ReturnToMinigameLobby()
    {
        this.Board.StopPlaying();
        this.Board.ResetHighlights();
        base.ReturnToMinigameLobby();
    }

    private void DisplayEarlyGameTermination(MinigameTeam team, PhotonPlayer player)
    {
        GUIManager.Instance.ShowTooltip("The opposing player left the game.");
    }

    [PunRPC]
    protected override bool StartGame(PhotonMessageInfo info)
    {
        if (!base.StartGame(info))
            { return false; }

        // Pure start after this point, checks and UI have already been handled
        // CameraManager.SetViewLookAngleMax(90f);
        IEnumerator<PhotonPlayer> iterateA = this.TeamContainerA.Team.GetEnumerator();
        IEnumerator<PhotonPlayer> iterateB = this.TeamContainerB.Team.GetEnumerator();
        iterateA.MoveNext();    // A newly acquired enumerator points to just before the first element
        iterateB.MoveNext();
        this.Board.StartPlaying(iterateA.Current, iterateB.Current);
        return true;
    }

    public override bool ValidToStart()
    {
        return this.TeamContainerA.Team.Size + this.TeamContainerB.Team.Size == 2;
    }

    protected override void DisplayStartingError()
    {
        GUIManager.Instance.ShowTooltip("Unable to start game, there are not enough players.");
    }
}
