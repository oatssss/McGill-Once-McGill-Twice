using UnityEngine;
using System.Collections.Generic;
using System;
using ExtensionMethods;

public abstract class Minigame : Photon.MonoBehaviour
{
    [SerializeField] private List<MinigameTeam> Teams = new List<MinigameTeam>();
    [SerializeField] private Transform InstructionPrefab;
    [SerializeField] private Transform GameInfoPrefab;

    /// <summary>
    ///  Displays the minigame's instruction UI to the player.
    /// </summary>
    public void DisplayInstructions()
    {
        Transform prefab = Instantiate(InstructionPrefab);
        prefab.SetParent(GUIManager.Instance.Canvas.transform, false);
        
        // TODO : Set focus, then Use animator to transition
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
    ///  Requests the MasterClient to start the game.
    /// </summary>
    public void StartGame()
    {
        // this.photonView.RpcAsMaster("StartGame", PhotonTargets.AllBufferedViaServer);
        this.photonView.RPC("StartGame", PhotonTargets.AllBufferedViaServer);
    }

    /// <summary>
    ///  RPC called by the MasterClient to start the game.
    /// </summary>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    protected abstract void StartGame(PhotonMessageInfo info);

    /// <summary>
    ///  RPC called by the MasterClient to end the game and reset the minigame state.
    /// </summary>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    private void EndGame(PhotonMessageInfo info)
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
    
    /*
    /// <summary>
    ///  RPC called by the MasterClient to end the game and reset the minigame state.
    /// </summary>
    protected virtual void EndGame()
    {
        // Do nothing as an adapter
    }
    */

    /// <summary>
    ///  Requests the MasterClient to add this player to the corresponding team.
    /// </summary>
    /// <param name="team"> The team to add this player to.
    /// </param>
    public void AddPlayerToTeam(MinigameTeam team)
    {
        // this.photonView.RpcAsMaster("AddPlayerToTeam", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player, team);
        this.photonView.RPC("AddPlayerToTeam", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player, team);
    }
    
    /// <summary>
    ///  RPC called by the MasterClient to add the corresponding player to the corresponding team.
    /// </summary>
    /// <param name="player"> The player that's added to <paramref name="team"/>.
    /// </param>
    /// <param name="team"> A clone of the team that <paramref name="player"/> is being added to, received over the network.
    /// </param>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    private void AddPlayerToTeam(PhotonPlayer player, MinigameTeam team, PhotonMessageInfo info)
    {
        // Find the matching team
        MinigameTeam actualTeam = this.Teams.Find(item => item.Equals(team));
        
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
    ///  Requests the MasterClient to remove this player from the corresponding team.
    /// </summary>
    /// <param name="team"> The team to add this player to.
    /// </param>
    public void RemovePlayerFromTeam(MinigameTeam team)
    {
        // this.photonView.RpcAsMaster("RemovePlayerFromTeam", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player, team);
        this.photonView.RPC("RemovePlayerFromTeam", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player, team);
    }

    /// <summary>
    ///  RPC called by the MasterClient to remove the corresponding player from the corresponding team.
    /// </summary>
    /// <param name="player"> The player that's added to <paramref name="team"/>.
    /// </param>
    /// <param name="team"> The team that <paramref name="player"/> is added to.
    /// </param>
    /// <param name="info"> The info provided by the RPC sender (sender should always be the MasterClient).
    /// </param>
    [PunRPC]
    private void RemovePlayerFromTeam(PhotonPlayer player, MinigameTeam team, PhotonMessageInfo info)
    {
        // Find the matching team
        MinigameTeam actualTeam = this.Teams.Find(item => item.TeamID == team.TeamID);
        
        // Remove the player from the team, if the player is the client, trigger the events associated with leaving the game
        bool successfullyRemoved = actualTeam.RemovePlayer(player);
        if (player.Equals(PhotonNetwork.player))
        {
            if (successfullyRemoved)
                { this.PlayerLeave(player); }
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
    ///  What the player should do after leaving the game.
    /// </summary>
    /// <param name="player"> The player leaving.
    /// </param>
    protected abstract void PlayerLeave(PhotonPlayer player);

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
}