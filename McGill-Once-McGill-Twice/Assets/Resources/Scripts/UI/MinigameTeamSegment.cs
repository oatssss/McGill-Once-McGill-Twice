using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinigameTeamSegment : LiveMenuSegment
{
    public MinigameTeamContainer TeamContainer;
    [SerializeField] private Minigame Minigame;
    [SerializeField] private RectTransform Content;
    [SerializeField] private Button Button;
    [SerializeField] private List<PlayerListItem> ListItems = new List<PlayerListItem>();

    public override void UpdateSegment()
    {
        // Ensure the list view has enough slots to show a full size team
        while (this.TeamContainer.Team.MaxSize > this.ListItems.Count)
        {
            PlayerListItem newListItem = Instantiate<PlayerListItem>(GUIManager.Instance.PlayerListItemPrefab);
            newListItem.SetUnoccupied();
            newListItem.gameObject.transform.SetParent(this.Content, false);
            this.ListItems.Add(newListItem);
        }

        // Ensure the list view doesn't have any extra slots on top of a full team
        while (this.TeamContainer.Team.MaxSize < this.ListItems.Count)
        {
            this.ListItems.RemoveAt(0);
        }

        // Occupy a corresponding slot for each player on the team
        List<PlayerListItem>.Enumerator listItems = this.ListItems.GetEnumerator();
        foreach (PhotonPlayer player in this.TeamContainer.Team)
        {
            listItems.MoveNext();
            PlayerListItem current = listItems.Current;

            if (!player.Equals(current.Player))
                { current.SetOccupied(player); }
        }

        // Any remaining slots should be set to unoccupied
        while (listItems.MoveNext())
        {
            if (listItems.Current.Occupied)
                { listItems.Current.SetUnoccupied(); }
        }

        // Update the status of the join button
        if (this.TeamContainer.Team.Contains(PhotonNetwork.player))
        {
            this.ShowLeaveTeamButton();
        }
        else if (this.TeamContainer.Team.Size == this.TeamContainer.Team.MaxSize)
        {
            this.ShowFullTeamButton();
        }
        else
        {
            this.ShowJoinTeamButton();
        }
    }

    private void ShowLeaveTeamButton()
    {
        this.Button.onClick.RemoveAllListeners();
        this.Button.onClick.AddListener(() => this.Minigame.RemovePlayer());
        this.Button.GetComponentInChildren<Text>().text = "Leave team";
        this.Button.interactable = true;
    }

    private void ShowJoinTeamButton()
    {
        this.Button.onClick.RemoveAllListeners();
        this.Button.onClick.AddListener(() => this.Minigame.AddPlayerToTeam(this.TeamContainer));
        this.Button.GetComponentInChildren<Text>().text = "Join team";

        if (PlayerManager.Instance.JoinedTeam)
            { this.Button.interactable = false; }
        else
            { this.Button.interactable = true; }
    }

    private void ShowFullTeamButton()
    {
        this.Button.onClick.RemoveAllListeners();
        this.Button.GetComponentInChildren<Text>().text = "Full team";
        this.Button.interactable = false;
    }
}
