using UnityEngine;
using System.Collections.Generic;
using System;

public class ConnectFourMenu : Menu
{
    [SerializeField] private ConnectFourMinigame Minigame;
    [SerializeField] private MinigameTeamSegment SegmentTeamA;
    [SerializeField] private MinigameTeamSegment SegmentTeamB;

    protected override void Activate()
    {
        this.SegmentTeamA.Team = this.Minigame.TeamA;
        this.SegmentTeamB.Team = this.Minigame.TeamB;
        base.Activate();
    }
}
