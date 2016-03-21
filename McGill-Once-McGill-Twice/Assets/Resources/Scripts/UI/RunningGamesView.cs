using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RunningGamesView : LiveMenuView {

    [SerializeField] private GameObject Content;
    [SerializeField] private Button JoinGameButton;
    [SerializeField] private RoomListItem RoomListItemPrefab;
    [ReadOnly] [SerializeField] private bool GreyedFlag;
    [ReadOnly] [SerializeField] private bool RefreshFlag;
    [ReadOnly] [SerializeField] private List<RoomListItem> Rooms = new List<RoomListItem>();
    [ReadOnly] [SerializeField] private RoomListItem SelectedRoom;
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
                RoomListItem roomListItem = Instantiate<RoomListItem>(this.RoomListItemPrefab);
                roomListItem.gameObject.SetActive(true);
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
        }

        // Synchronize the list
        int i = 0;
        foreach (RoomListItem roomListItem in this.Rooms)
        {
            if (roomListItem.Room != this.OnlineRooms[i])
                { roomListItem.Room = this.OnlineRooms[i]; }
            i++;
        }
    }

    public override void Activate()
    {
        base.Activate();

        if (!PhotonNetwork.connectedAndReady)
            { PhotonManager.Instance.ConnectToPhoton(); }

        this.RefreshFlag = true;
        this.JoinGameButton.interactable = false;
    }

    public override void Deactivate()
    {
        this.Deselect(this.SelectedRoom);
        this.JoinGameButton.interactable = false;
        base.Deactivate();
    }

    private void Refresh()
    {
        this.RefreshFlag = false;
        this.GreyOut(false);

        this.OnlineRooms = PhotonNetwork.GetRoomList();
        Debug.Log(this.OnlineRooms);
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

    public void Select(RoomListItem room)
    {
        this.SelectedRoom = room;
    }

    public void Deselect(RoomListItem caller)
    {
        if (this.SelectedRoom == caller)
            { this.SelectedRoom = null; }
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom(this.SelectedRoom.Room.name);

        long levelSeed;
        long.TryParse(this.SelectedRoom.Room.customProperties[GameConstants.KEY_SEED].ToString(), out levelSeed);
        GUIManager.MajorFadeToBlack( () => GameManager.Instance.GenerateLevel(levelSeed) );

        GUIManager.Instance.ResumeGame();
    }
}
