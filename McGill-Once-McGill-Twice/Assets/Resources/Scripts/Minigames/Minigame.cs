using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;

public abstract class Minigame : Photon.PunBehaviour
{
    [SerializeField] private List<MinigameTeamContainer> TeamContainers = new List<MinigameTeamContainer>();
    public abstract Overlay PlayingUI { get; }
    [SerializeField] private Menu InstructionMenu;
    [SerializeField] private MinigameInfoMenu InfoMenu;
    [SerializeField] private MinigameZone Zone;
    [ReadOnly] public bool LocalPlayerJoined;
    [ReadOnly] public bool Started;
    private Coroutine InteractForLobbyReturn = null;

    protected virtual void Awake()
    {
        // Only update when the player has joined the game
        this.enabled = false;
    }

    /// <summary>
    ///  Displays the minigame's instruction UI to the player.
    /// </summary>
    public void DisplayInstructions()
    {
        GUIManager.Instance.OpenMenu(this.InstructionMenu);
    }

    /// <summary>
    ///  Displays the minigame's game-info UI to the player.
    /// </summary>
    public void DisplayGameInfo()
    {
        GUIManager.Instance.OpenMenu(this.InfoMenu);
    }

    /// <summary>
    ///  Starts the game for all players on the teams.
    /// </summary>
    public void StartGame()
    {
        this.photonView.RPC("StartGame", PhotonTargets.AllBufferedViaServer);
    }

    public abstract bool ValidToStart();

    protected virtual void DisplayStartingError()
    {
        GUIManager.Instance.ShowTooltip("Unable to start game.");
    }

    /// <summary>
    ///  RPC called by a client in the same game triggering game start.
    /// </summary>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected virtual bool StartGame(PhotonMessageInfo info)
    {
        // Make sure the game is valid to start
        if (!this.ValidToStart())
        {
            if (this.LocalPlayerJoined)
                { this.DisplayStartingError(); }

            return false;
        }

        this.Started = true;

        if (this.LocalPlayerJoined)
        {
            GUIManager.Instance.CloseMenuIfOpen(this.InfoMenu);
            GUIManager.ShowMinigameUI(this);
        }

        return true;
    }

    /// <summary>
    ///  RPC called when this game becomes empty.
    /// </summary>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected void ResetGame(PhotonMessageInfo info)
    {
        this.Started = false;

        foreach (MinigameTeamContainer team in this.TeamContainers)
        {
            team.Team.ResetTeam();
        }

        // Remove buffered RPCs
        if (PhotonNetwork.isMasterClient)
            { PhotonNetwork.RemoveRPCs(this.photonView); }

        // Perform any supplemental events such as awarding achievements
    }

    public void RequireInteractForLobby()
    {
        this.InteractForLobbyReturn = StartCoroutine(InteractToReturnToLobby());
    }

    private IEnumerator InteractToReturnToLobby()
    {
        while (!CustomInputManager.GetButtonDown("Interact Main", CustomInputManager.InputMode.Gameplay))
        {
            // Show UI requesting interact to return to lobby
            GUIManager.Instance.ShowTooltip("Press E to continue.", GUIManager.TOOL_TIP_DURATION.INSTANTANEOUS);
            yield return null;
        }

        this.ReturnToMinigameLobby();
    }

    protected virtual void ReturnToMinigameLobby()
    {
        if (this.InteractForLobbyReturn != null)
        {
            StopCoroutine(this.InteractForLobbyReturn);
            this.InteractForLobbyReturn = null;
        }
        GUIManager.ShowMinigameJoinedUI(this);
        this.DisplayGameInfo();
        // TODO : Enable tab menu
    }

    /// <summary>
    ///  Requests this client's player to be added to <paramref name="team"/>.
    /// </summary>
    /// <param name="team"> The team to add this player to.
    /// </param>
    public void AddPlayerToTeam(MinigameTeamContainer teamContainer)
    {
        // this.photonView.RpcAsMaster("AddPlayerToTeam", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player, team);
        this.photonView.RPC("AddPlayerToTeam", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player, teamContainer.Team);
    }

    /// <summary>
    ///  RPC called by the client wanting to be added to the corresponding team.
    /// </summary>
    /// <param name="player"> The player that's added to <paramref name="team"/>.
    /// </param>
    /// <param name="team"> A clone of the team that <paramref name="player"/> is being added to, received over the network.
    /// </param>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected virtual void AddPlayerToTeam(PhotonPlayer player, MinigameTeam team, PhotonMessageInfo info)
    {
        // A recently disconnected player may still have an AddPlayer RPC buffered for new joining players, in this case, the new joining player will be trying to add a null player
        if (player == null)
            { return; }

        // Find the matching team
        MinigameTeam actualTeam = this.TeamContainers.Find(container => container.Team.Equals(team)).Team;

        // Add the player to the team, if the player is the client, trigger the events associated with joining a team
        bool successfullyAdded = actualTeam.AddPlayer(player);
        if (player.Equals(PhotonNetwork.player))
        {
            if (successfullyAdded)
                { this.LocalPlayerJoin(actualTeam); }
            else
                { this.DisplayJoiningError(); }
        }

        // if (this.LocalPlayerJoined && this.ValidToStart())
        //     { this.InfoMenu.StartButton.interactable = true; }
    }

    /// <summary>
    ///  Displays a UI alert that the team could not be joined.
    /// </summary>
    protected void DisplayJoiningError()
    {
        GUIManager.Instance.ShowTooltip("An error occurred, unable to join.");
    }

    /// <summary>
    ///  What the player should do after successfully being added to a team.
    /// </summary>
    /// <param name="player"> The player that's added to <paramref name="team"/>.
    /// </param>
    /// <param name="team"> The team that <paramref name="player"/> is added to.
    /// </param>
    protected virtual void LocalPlayerJoin(MinigameTeam team)
    {
        PlayerManager.Instance.JoinedTeam = true;
        this.LocalPlayerJoined = true;
        // CameraManager.SetViewLookAngleMax(90f);
        PlayerManager.DisableUserMovement();
        this.ReturnToMinigameLobby();
        this.enabled = true;
    }

    /// <summary>
    ///  Requests all other players remove <paramref name="player"/> from the corresponding team.
    /// </summary>
    private void RemovePlayer(PhotonPlayer player)
    {
        this.photonView.RPC("RemovePlayer", PhotonTargets.AllBufferedViaServer, player);
    }

    /// <summary>
    ///  Requests all other players remove this client's player from the corresponding team.
    /// </summary>
    public void RemovePlayer()
    {
        this.RemovePlayer(PhotonNetwork.player);
    }

    /// <summary>
    ///  RPC called by the client who wants to be removed from the corresponding team.
    /// </summary>
    /// <param name="player"> The player that's added to <paramref name="team"/>.
    /// </param>
    /// <param name="team"> The team that <paramref name="player"/> is added to.
    /// </param>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected virtual void RemovePlayer(PhotonPlayer player, PhotonMessageInfo info)
    {
        // A recently disconnected player may still have a RemovePlayer RPC buffered for new joining players, in this case, the new joining player will be trying to remove a null player
        if (player == null)
            { return; }

        // Find the matching team
        MinigameTeam correspondingTeam = this.TeamContainers.Find(container => container.Team.Contains(player)).Team;

        if (correspondingTeam == null)
        {
            Debug.LogErrorFormat("Player with ID {0} was not found on any team to remove.", player);
            return;
        }

        // Remove the player from the team, if the player is the client, trigger the events associated with leaving the game
        bool successfullyRemoved = correspondingTeam.RemovePlayer(player);

        // Special handling if the player leaving is part of a game the local player is in
        if (this.LocalPlayerJoined && !player.Equals(PhotonNetwork.player) && successfullyRemoved)
            { this.HandleRemotePlayerLeaveDetails(correspondingTeam, player); }

        if (player.Equals(PhotonNetwork.player))
        {
            if (successfullyRemoved)
                { this.LocalPlayerLeave(); }
            else
                { this.DisplayLeavingError(); }
        }

        // The master client checks to see if the game is empty, if so, reset this game
        if (PhotonNetwork.isMasterClient)
        {
            int players = 0;
            foreach (MinigameTeamContainer container in this.TeamContainers)
                { players += container.Team.Size; }

            if (players == 0)
                { this.photonView.RPC("ResetGame", PhotonTargets.AllViaServer); }
        }

        // if (!this.ValidToStart())
        //     { this.InfoMenu.StartButton.interactable = false; }
    }

    protected abstract void HandleRemotePlayerLeaveDetails(MinigameTeam team, PhotonPlayer player);

    /// <summary>
    ///  Displays a UI alert notifying the user that they could not leave the game.
    /// </summary>
    protected void DisplayLeavingError()
    {
        GUIManager.Instance.ShowTooltip("An error occurred, unable to leave.");
    }

    /// <summary>
    ///  What the client should do after leaving the game.
    /// </summary>
    /// <param name="player"> The player leaving.
    /// </param>
    protected virtual void LocalPlayerLeave()
    {
        this.enabled = false;
        PlayerManager.Instance.JoinedTeam = false;
        this.LocalPlayerJoined = false;
        CameraManager.SetViewToPlayer();
        PlayerManager.EnableUserMovement();
        GUIManager.ShowFreeRoamUI();
        // this.EnableZone();
    }

    public void EnableZone()
    {
        this.Zone.EnableZone();
    }

    public void DisableZone()
    {
        this.Zone.DisableZone();
    }

    /// <summary>
    ///  Acquires the team directly after <paramref name="team"/>. If <paramref name="team"/> is the last team, the team returned is the first team.
    /// </summary>
    /// <param name="team"> The team previous to the one we are retrieving.
    /// </param>
    /// <returns> The team directly after <paramref name="team"/>.
    /// </returns>
    protected MinigameTeamContainer GetTeamContainerAfter(MinigameTeamContainer teamContainer)
    {
        int index = this.TeamContainers.IndexOf(teamContainer);
        int nextIndex = (index + 1) % this.TeamContainers.Count;
        return this.TeamContainers[nextIndex];
    }

    /// <summary>
    ///  Adds <paramref name="team"/> to this minigame.
    /// </summary>
    /// <param name="team"> The team to add to this minigame.
    /// </param>
    protected void AddTeamContainer(MinigameTeamContainer teamContainer)
    {
        this.TeamContainers.Add(teamContainer);
    }

    /*
     *  PunBehaviour implements
     */

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        MinigameTeamContainer correspondingTeamContainer = this.TeamContainers.Find(teamContainer => teamContainer.Team.Contains(otherPlayer));

        // If the disconnecting player belonged to a team, remove them from that team
        if (correspondingTeamContainer)
        {
            // MinigameTeam team = correspondingTeamContainer.Team;
            this.RemovePlayer(otherPlayer, null);
        }
    }

    public virtual void Update()
    {
        if (CustomInputManager.GetButtonDown("Minigame Menu", CustomInputManager.InputMode.Gameplay))
            { GUIManager.Instance.OpenMenu(this.InfoMenu); }
    }
}