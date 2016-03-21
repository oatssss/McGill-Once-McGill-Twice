using UnityEngine;
using UnityEngine.UI;

public class SavedGameItem : MonoBehaviour {

    public GameManager.GameState State = new GameManager.GameState();

    public void SetRoomName(Text inputRoomName)
    {
        this.State.RoomName = inputRoomName.text;
    }

    public void SetRoomName(string inputRoomName)
    {
        this.State.RoomName = inputRoomName;
    }

    public void SetLevelSeed(Text inputLevelSeed)
    {
        long.TryParse(inputLevelSeed.text, out this.State.LevelSeed);
    }

    public void SetLevelSeed(string inputLevelSeed)
    {
        long.TryParse(inputLevelSeed, out this.State.LevelSeed);
    }

    public void SetAutosaveInterval(Text inputInterval)
    {
        int.TryParse(inputInterval.text, out this.State.AutosaveInterval);
    }

    public void SetAutosaveInterval(string inputInterval)
    {
        int.TryParse(inputInterval, out this.State.AutosaveInterval);
    }

    public void SetAutosaveUnit(Dropdown inputUnit)
    {
        this.State.AutosaveUnit = (GameManager.AUTOSAVE_UNIT)inputUnit.value;
    }
}
