using System;

public  abstract class TurnBasedMinigame : Minigame
{
    public MinigameTeam CurrentTurn;

    /// <summary>
    ///  An operation that does...
    /// </summary>
    /// <returns>
    /// </returns>
    [PunRPC]
    protected void NextTurn()
    {
        throw new NotImplementedException();
    }
}