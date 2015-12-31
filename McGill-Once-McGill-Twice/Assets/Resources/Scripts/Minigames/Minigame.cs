using UnityEngine;
using System.Collections.Generic;
using System;
using ExtensionMethods;

public abstract class Minigame : Photon.PunBehaviour
{
    private List<MinigameTeam> Teams;
    [SerializeField] private Transform InstructionPrefab;
    [SerializeField] private Transform GameInfoPrefab;
    
    protected virtual void Awake()
    {
        this.Teams = new List<MinigameTeam>();
    }

    /// <summary>
    ///  Displays the minigame's instruction UI to the player.
    /// </summary>
    public void DisplayInstructions()
    {
        Transform prefab = Instantiate(InstructionPrefab);
        prefab.SetParent(GUIManager.Instance.Canvas.transform, false);
        
        // TODO : Set focus, then Use animator to transition UI
        throw new NotImplementedException();
    }

    /// <summary>
    ///  Displays the minigame's game-info UI to the player.
    /// </summary>
    public void DisplayGameInfo()
    {
        Transform prefab = Instantiate(GameInfoPrefab);
        prefab.SetParent(GUIManager.Instance.Canvas.transform, false);
        
        // TODO : Set focus, Use animator to transition and move focus
        throw new NotImplementedException();
    }
    
    /// <summary>
    ///  Starts the game for all players on the teams.
    /// </summary>
    public void StartGame()
    {
        // this.photonView.RpcAsMaster("StartGame", PhotonTargets.AllBufferedViaServer);
        this.photonView.RPC("StartGame", PhotonTargets.AllBufferedViaServer);
    }

    /// <summary>
    ///  RPC called by a client in the same game triggering game start.
    /// </summary>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected abstract void StartGame(PhotonMessageInfo info);

    /// <summary>
    ///  RPC called by a client in the same game trigerring teams to reset and allowing control over player again.
    /// </summary>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected void EndGame(PhotonMessageInfo info)
    {
        foreach (MinigameTeam team in this.Teams)
        {
            team.ResetTeam();
        }
        
        // Give free-roam control back to the player
        PlayerManager.GetMainPlayer(true).ThirdPersonCharacter.EnableUserControls();
        
        // Update UI for free-roam
        throw new NotImplementedException();
        
        // Perform any supplemental events such as awarding achievements
        // this.EndGame();
    }

    /// <summary>
    ///  Requests this client's player to be added to <paramref name="team"/>.
    /// </summary>
    /// <param name="team"> The team to add this player to.
    /// </param>
    public void AddPlayerToTeam(MinigameTeam team)
    {
        // this.photonView.RpcAsMaster("AddPlayerToTeam", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player, team);
        this.photonView.RPC("AddPlayerToTeam", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player, team);
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
    protected void AddPlayerToTeam(PhotonPlayer player, MinigameTeam team, PhotonMessageInfo info)
    {
        // Find the matching team
        MinigameTeam actualTeam = this.Teams.Find(listItem => listItem.Equals(team));
        
        // Add the player to the team, if the player is the client, trigger the events associated with joining a team
        bool successfullyAdded = actualTeam.AddPlayer(player);
        if (player.Equals(PhotonNetwork.player))
        {
            if (successfullyAdded)
                { this.PlayerJoin(player, actualTeam); }
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
    protected abstract void PlayerJoin(PhotonPlayer player, MinigameTeam team);
    
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
        MinigameTeam correspondingTeam = this.Teams.Find(team => team.Contains(player));
        
        if (correspondingTeam == null)
        {
            Debug.LogErrorFormat("Player with ID {0} was not found on any team to remove.", player);
            return;
        }
        
        // Remove the player from the team, if the player is the client, trigger the events associated with leaving the game
        bool successfullyRemoved = correspondingTeam.RemovePlayer(player);
        if (player.Equals(PhotonNetwork.player))
        {
            if (successfullyRemoved)
                { this.PlayerLeave(); }
            else
                { this.DisplayLeavingError(); }
        }
    }
    
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
    protected abstract void PlayerLeave();

    /// <summary>
    ///  Acquires the team directly after <paramref name="team"/>. If <paramref name="team"/> is the last team, the team returned is the first team.
    /// </summary>
    /// <param name="team"> The team previous to the one we are retrieving.
    /// </param>
    /// <returns> The team directly after <paramref name="team"/>.
    /// </returns>
    protected MinigameTeam GetTeamAfter(MinigameTeam team)
    {
        int index = this.Teams.IndexOf(team);
        int nextIndex = (index + 1) % this.Teams.Count;
        return this.Teams[nextIndex];
    }
    
    /// <summary>
    ///  Adds <paramref name="team"/> to this minigame.
    /// </summary>
    /// <param name="team"> The team to add to this minigame.
    /// </param>
    protected void AddTeam(MinigameTeam team)
    {
        this.Teams.Add(team);
    }
    
    /*
     *  PunBehaviour implements
     */
    
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        // Only allow the master client to add the RPC removing the player to the buffer
        if (PhotonNetwork.isMasterClient)
            { this.RemovePlayer(otherPlayer); }
    }
}