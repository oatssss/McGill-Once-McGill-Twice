using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour {

    public PhotonPlayer Player = null;
    [ReadOnly] public bool Occupied = false;
    [SerializeField] private Text Name;
    [SerializeField] private GameObject OccupiedView;
    [SerializeField] private GameObject UnoccupiedView;

    public void SetOccupied(PhotonPlayer player)
    {
        this.Player = player;
        this.Name.text = player.name;
        this.UnoccupiedView.SetActive(false);
        this.OccupiedView.SetActive(true);
        this.Occupied = true;
    }

    public void SetUnoccupied()
    {
        this.Player = null;
        this.Name.text = "Unoccupied";
        this.OccupiedView.SetActive(false);
        this.UnoccupiedView.SetActive(true);
        this.Occupied = false;
    }
}
