using UnityEngine;
using System.Collections.Generic;

public class RunningGamesView : LiveMenuView {

    [SerializeField] private GameObject Content;
    [ReadOnly] [SerializeField] private bool GreyedFlag;
    [ReadOnly] [SerializeField] private bool RefreshFlag;
    [ReadOnly] [SerializeField] private List<RoomListItem> Rooms = new List<RoomListItem>();
    private RoomInfo[] OnlineRooms;

    protected override void Update()
    {
        if (PhotonNetwork.connecting || !PhotonNetwork.connectedAndReady)
        {
            this.GreyOut(true);
            return;
        }

        if (this.RefreshFlag || this.OnlineRooms == null)
            { this.Refresh(); }

        // The game list is not up to date
        if (this.OnlineRooms.Length != this.Rooms.Count)
        {
            // Extend the list to match the online rooms
            while (OnlineRooms.Length > this.Rooms.Count)
            {
                RoomListItem roomListItem = Instantiate<RoomListItem>(GUIManager.Instance.RoomListItemPrefab);
                roomListItem.transform.SetParent(this.Content.transform, false);
                this.Rooms.Add(roomListItem);
            }

            // Delete from the list to match online rooms
            while (OnlineRooms.Length < this.Rooms.Count)
            {
                RoomListItem del = this.Rooms[0];
                this.Rooms.RemoveAt(0);
                Destroy(del.gameObject);
            }

            // Synchronize the list
            int i = 0;
            foreach (RoomListItem roomListItem in this.Rooms)
            {
                roomListItem.Name.text = this.OnlineRooms[i].name;
                int numPlayers = this.OnlineRooms[i].playerCount;
                int maxPlayers = this.OnlineRooms[i].maxPlayers;
                roomListItem.Players.text = numPlayers + "/" + maxPlayers;
                i++;
            }
        }
    }

    public override void Activate()
    {
        base.Activate();

        if (!PhotonNetwork.connectedAndReady)
            { PhotonManager.Instance.ConnectToPhoton(); }

        this.RefreshFlag = true;
    }

    private void Refresh()
    {
        this.RefreshFlag = false;
        this.GreyOut(false);

        this.OnlineRooms = PhotonNetwork.GetRoomList();
    }

    private void GreyOut(bool grey)
    {
        if ((this.GreyedFlag && grey) || (!this.GreyedFlag && !grey))
            { return; }

        this.GreyedFlag = grey;

        if (grey)
        {
            this.OnlineRooms = null;
            GUIManager.Instance.ShowTooltip("Could not connect to servers.");
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }
}
