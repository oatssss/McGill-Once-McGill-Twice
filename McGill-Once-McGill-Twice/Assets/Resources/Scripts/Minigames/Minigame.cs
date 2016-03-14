using UnityEngine;
using System.Collections.Generic;
using System;
using ExtensionMethods;

public abstract class Minigame : Photon.PunBehaviour
{
    [SerializeField] private List<MinigameTeamContainer> TeamContainers = new List<MinigameTeamContainer>();
    [SerializeField] private Menu InstructionMenu;
    [SerializeField] private Menu InfoMenu;
    [SerializeField] private MinigameZone Zone;
    [ReadOnly] public bool LocalPlayerJoined;

    protected virtual void Awake()
    {

    }

    /// <summary>
    ///  Displays the minigame's instruction UI to the player.
    /// </summary>
    public void DisplayInstructions()
    {
        // this.InstructionMenu.Open();
        GUIManager.Instance.OpenMenu(this.InstructionMenu);
    }

    /// <summary>
    ///  Displays the minigame's game-info UI to the player.
    /// </summary>
    public void DisplayGameInfo()
    {
        // this.InfoMenu.Open();
        GUIManager.Instance.OpenMenu(this.InfoMenu);
    }

    /// <summary>
    ///  Starts the game for all players on the teams.
    /// </summary>
    public void StartGame()
    {
        // TODO : Check if start is valid for number of members
        this.photonView.RPC("StartGame", PhotonTargets.AllBufferedViaServer);
    }

    /// <summary>
    ///  RPC called by a client in the same game triggering game start.
    /// </summary>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected virtual void StartGame(PhotonMessageInfo info)
    {
        GUIManager.ShowMinigameUI(this);
    }

    /// <summary>
    ///  RPC called when this game becomes empty.
    /// </summary>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected void ResetGame(PhotonMessageInfo info)
    {
        foreach (MinigameTeamContainer team in this.TeamContainers)
        {
            team.Team.ResetTeam();
        }

        // Remove buffered calls
        this.photonView.ClearRpcBufferAsMasterClient();

        // Perform any supplemental events such as awarding achievements
    }

    protected virtual void ReturnToMinigameLobby()
    {
        GUIManager.ShowMinigameJoinedUI(this);
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
    }

    /// <summary>
    ///  Displays a UI alert that the team could not be joined.
    /// </summary>
    protected void DisplayJoiningError()
    {
        throw new NotImplementedException();
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
        this.LocalPlayerJoined = true;
        CameraManager.SetViewLookAngleMax(90f);
        PlayerManager.DisableUserControls();
        this.ReturnToMinigameLobby();
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
    protected void RemovePlayer(PhotonPlayer player, PhotonMessageInfo info)
    {
        // Find the matching team
        MinigameTeam correspondingTeam = this.TeamContainers.Find(container => container.Team.Contains(player)).Team;

        if (correspondingTeam == null)
        {
            Debug.LogErrorFormat("Player with ID {0} was not found on any team to remove.", player);
            return;
        }

        // Remove the player from the team, if the player is the client, trigger the events associated with leaving the game
        bool successfullyRemoved = correspondingTeam.RemovePlayer(player);
        if (!player.Equals(PhotonNetwork.player) && successfullyRemoved)
            { this.HandleRemotePlayerLeaveDetails(correspondingTeam, player); }

        if (player.Equals(PhotonNetwork.player))
        {
            if (successfullyRemoved)
                { this.LocalPlayerLeave(); }
            else
                { this.DisplayLeavingError(); }
        }
    }

    protected abstract void HandleRemotePlayerLeaveDetails(MinigameTeam team, PhotonPlayer player);

    /// <summary>
    ///  Displays a UI alert notifying the user that they could not leave the game.
    /// </summary>
    protected void DisplayLeavingError()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///  What the client should do after leaving the game.
    /// </summary>
    /// <param name="player"> The player leaving.
    /// </param>
    protected virtual void LocalPlayerLeave()
    {
        this.LocalPlayerJoined = false;
        CameraManager.SetViewToPlayer();
        PlayerManager.EnableUserControls();
        GUIManager.ShowFreeRoamUI();
        this.EnableZone();
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
        MinigameTeam correspondingTeam = this.TeamContainers.Find(teamContainer => teamContainer.Team.Contains(otherPlayer)).Team;

        // If the disconnecting player belonged to a team, remove them from that team
        if (correspondingTeam != null)
            { this.RemovePlayer(otherPlayer, null); }
    }
}