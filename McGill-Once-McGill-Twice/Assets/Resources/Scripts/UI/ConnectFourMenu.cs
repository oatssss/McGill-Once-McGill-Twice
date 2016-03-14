using UnityEngine;

public class ConnectFourMenu : MinigameInfoMenu
{
    private ConnectFourMinigame Minigame
    {
        get { return (ConnectFourMinigame) this.GenericMinigame; }
    }
    [SerializeField] private MinigameTeamSegment SegmentTeamBlue;
    [SerializeField] private MinigameTeamSegment SegmentTeamRed;

    protected override void Activate()
    {
        this.SegmentTeamBlue.Team = this.Minigame.TeamContainerA.Team;
        this.SegmentTeamRed.Team = this.Minigame.TeamContainerB.Team;
        base.Activate();
    }
}
