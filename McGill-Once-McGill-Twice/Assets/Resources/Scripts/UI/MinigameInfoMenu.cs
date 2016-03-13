using UnityEngine;
using System.Collections;

public class MinigameInfoMenu : Menu {

    [SerializeField] protected Minigame GenericMinigame;
	public override void Close()
    {
        base.Close();
        if (!this.GenericMinigame.LocalPlayerJoined)
        {
            this.GenericMinigame.EnableZone();
        }
    }
}
