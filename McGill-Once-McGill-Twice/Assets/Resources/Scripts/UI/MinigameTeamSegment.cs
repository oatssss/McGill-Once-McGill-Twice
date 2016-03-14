using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinigameTeamSegment : LiveMenuSegment
{
    public MinigameTeam Team { get; set; }
    [SerializeField] private RectTransform Content;
    [SerializeField] private Button JoinButton;
    [SerializeField] private List<PlayerListItem> ListItems = new List<PlayerListItem>();

    public override void UpdateSegment()
    {
        // Ensure the list view has enough slots to show a full size team
        while (this.Team != null && this.Team.MaxSize > this.ListItems.Count)
        {
            PlayerListItem newListItem = Instantiate<PlayerListItem>(GUIManager.Instance.PlayerListItemPrefab);
            newListItem.SetUnoccupied();
            newListItem.gameObject.transform.SetParent(this.Content, false);
            this.ListItems.Add(newListItem);
        }

        // Ensure the list view doesn't have any extra slots on top of a full team
        while (this.Team != null && this.Team.MaxSize < this.ListItems.Count)
        {
            this.ListItems.RemoveAt(0);
        }

        // Occupy a corresponding slot for each player on the team
        List<PlayerListItem>.Enumerator listItems = this.ListItems.GetEnumerator();
        listItems.MoveNext();
        foreach (PhotonPlayer player in this.Team)
        {
            PlayerListItem current = listItems.Current;

            if (!player.Equals(current.Player))
                { current.SetOccupied(player); }

            listItems.MoveNext();
        }

        // Any remaining slots should be set to unoccupied
        while (listItems.MoveNext())
        {
            if (listItems.Current.Occupied)
                { listItems.Current.SetUnoccupied(); }
        }

        // Update the status of the join button
        if (this.Team.Size == this.Team.MaxSize)
            { this.JoinButton.interactable = false; }
        else
            { this.JoinButton.interactable = true; }
    }
}
