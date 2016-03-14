using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class MinigameTeam
{
    private static int NumOccupiedIDs;
    public readonly int TeamID;
    private List<PhotonPlayer> Players;

    public int Score;

    public readonly int MaxSize;

    /// <summary>
    ///  The current number of players on the team.
    /// </summary>
    public int Size { get { return this.Players.Count; } }

    public MinigameTeam(int teamID, int maxSize)
    {
        this.TeamID = teamID;
        this.Players = new List<PhotonPlayer>();
        this.MaxSize = maxSize;
    }

    public MinigameTeam(int maxSize) : this(FreeID(), maxSize)
    {
        // Already calls other constructor with FreeID() as teamID
    }

    public static int FreeID()
    {
        return NumOccupiedIDs++;
    }

    /// <summary>
    ///  An operation that does...
    ///
    ///  @param firstParam a description of this parameter
    /// </summary>
    /// <param name="player"> The player to be added.
    /// </param>
    /// <returns> true if the player was successfully added, otherwise false.
    /// </returns>
    public bool AddPlayer(PhotonPlayer player)
    {
        if (this.Size >= this.MaxSize)
        {
            Debug.LogErrorFormat("Can't add {0} to team {1} since it's full.", player, this);
            return false;
        }
        if (this.Players.Contains(player))
        {
            Debug.LogErrorFormat("Can't add {0} to team {1}, player is already in the team.", player, this);
            return false;
        }

        this.Players.Add(player);
        return true;
    }

    /// <summary>
    ///  An operation that does...
    ///
    ///  @param firstParam a description of this parameter
    /// </summary>
    /// <param name="player">
    /// </param>
    /// <returns>
    /// </returns>
    public bool RemovePlayer(PhotonPlayer player)
    {
        if (!this.Players.Remove(player))
        {
            Debug.LogErrorFormat("Can't remove {0} from team {1}, player is not in team.", player, this);
            return false;
        }

        return true;
    }

    public bool Contains(PhotonPlayer player)
    {
        return this.Players.Contains(player);
    }

    /// <summary>
    ///  Removes all the players and resets the score.
    /// </summary>
    public void ResetTeam()
    {
        this.Players.Clear();
        this.Score = 0;
    }

    public IEnumerator<PhotonPlayer> GetEnumerator()
    {
        return this.Players.GetEnumerator();
    }

    /// <summary>
    /// Makes MinigameTeam comparable
    /// </summary>
    public override bool Equals(object team)
    {
        MinigameTeam otherTeam = team as MinigameTeam;
        return (otherTeam != null && this.GetHashCode() == otherTeam.GetHashCode());
    }

    public override int GetHashCode()
    {
        return this.TeamID;
    }
}