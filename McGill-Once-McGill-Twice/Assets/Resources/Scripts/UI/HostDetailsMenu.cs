using UnityEngine;
using UnityEngine.UI;

public class HostDetailsMenu : Menu {

	[SerializeField] private InputField RoomName;
    [SerializeField] private InputField AutosaveInterval;
    [SerializeField] private Dropdown AutosaveUnit;
    [SerializeField] private InputField MaxPlayers;
    [SerializeField] private RectTransform SeedOptions;
    [SerializeField] private Toggle CustomSeed;
    [SerializeField] private InputField Seed;
    [SerializeField] private SavedGameItem SavedGameItem;

    public void ShowNewGame()
    {
        GameManager.Instance.SessionState = new GameManager.GameState();
    }

    protected override void Activate()
    {
        base.Activate();
        this.SavedGameItem.State = GameManager.Instance.SessionState;
        this.RoomName.text = this.SavedGameItem.State.RoomName;
        this.AutosaveInterval.text = this.SavedGameItem.State.AutosaveInterval.ToString();
        this.AutosaveUnit.value = (int) this.SavedGameItem.State.AutosaveUnit;
        this.MaxPlayers.text = this.SavedGameItem.State.MaxPlayers.ToString();

        bool detailsNotFromSaved = this.SavedGameItem.State.SaveGameIdentifier == -1;

        this.CustomSeed.isOn = detailsNotFromSaved ? false : true;
        this.Seed.text = this.SavedGameItem.State.LevelSeed.ToString();

        this.SeedOptions.gameObject.SetActive(detailsNotFromSaved);
    }
}
