using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RoomListItem : MonoBehaviour, ISelectHandler, IDeselectHandler {

    private RoomInfo room;
    public RoomInfo Room
    {
        get { return this.room; }
        set
        {
            string roomName;
            if (value.customProperties.ContainsKey(GameConstants.KEY_ROOMNAME))
                { roomName = (string) value.customProperties[GameConstants.KEY_ROOMNAME]; }
            else
                { roomName = value.name; }
            this.Name.text = roomName;
            this.Players.text = value.playerCount + "/" + value.maxPlayers;
        }
    }
    [SerializeField] private Text Name;
    [SerializeField] private Text Players;

    [SerializeField] private UnityEvent onSelect;
    [SerializeField] private UnityEvent onDeselect;

    public void OnDeselect(BaseEventData eventData)
    {
        if (this.onDeselect != null)
            { this.onDeselect.Invoke(); }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (this.onSelect != null)
            { this.onSelect.Invoke(); }
    }
}
