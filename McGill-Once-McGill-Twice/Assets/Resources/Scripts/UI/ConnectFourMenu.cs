using UnityEngine;

public class ConnectFourMenu : MinigameInfoMenu
{
    private ConnectFourMinigame Minigame
    {
        get { return (ConnectFourMinigame) this.GenericMinigame; }
    }
    [SerializeField] private MinigameTeamView TeamBlueView;
    [SerializeField] private MinigameTeamView TeamRedView;

    protected override void Activate()
    {
        this.TeamBlueView.TeamContainer = this.Minigame.TeamContainerA;
        this.TeamRedView.TeamContainer = this.Minigame.TeamContainerB;
        base.Activate();
    }
}
