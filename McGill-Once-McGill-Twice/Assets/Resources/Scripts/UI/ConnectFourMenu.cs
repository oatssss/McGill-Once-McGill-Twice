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
        this.SegmentTeamBlue.TeamContainer = this.Minigame.TeamContainerA;
        this.SegmentTeamRed.TeamContainer = this.Minigame.TeamContainerB;
        base.Activate();
    }
}
