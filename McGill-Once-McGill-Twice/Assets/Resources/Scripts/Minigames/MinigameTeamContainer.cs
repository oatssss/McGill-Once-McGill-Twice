using UnityEngine;

public class MinigameTeamContainer : MonoBehaviour {

    public MinigameTeam Team;
    [SerializeField] private int MaxSize;

	void Awake()
    {
        this.Team = new MinigameTeam(this.MaxSize);
    }

    public void SetTeamSize(int size)
    {
        MinigameTeam newTeam = new MinigameTeam(size);

        foreach (PhotonPlayer player in this.Team)
        {
            newTeam.AddPlayer(player);
        }

        this.Team = newTeam;
    }
}
