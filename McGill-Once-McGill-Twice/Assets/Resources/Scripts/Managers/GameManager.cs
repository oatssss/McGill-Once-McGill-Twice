using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GameManager : UnitySingletonPersistent<GameManager> {

    public enum AUTOSAVE_UNIT { SECONDS, MINUTES, HOURS }

    public class GameState {
        public int SaveGameIdentifier;
        public string RoomName;
        public int AutosaveInterval;
        public GameManager.AUTOSAVE_UNIT AutosaveUnit;
        public long LevelSeed;
        public int MaxPlayers;
        public Dictionary<string,PlayerState> Players = new Dictionary<string,PlayerState>();

        public GameState()
        {
            this.SaveGameIdentifier = -1;   // A value of -1 means this GameState is newly created, not loaded
            this.RoomName = "";
            this.AutosaveInterval = 15;
            this.AutosaveUnit = GameManager.AUTOSAVE_UNIT.SECONDS;
            this.MaxPlayers = 5;
        }

        public PlayerState GetPlayerData(PhotonPlayer player)
        {
            if (this.Players.ContainsKey(player.name))
                { return this.Players[player.name]; }
            else
                { return null; }
        }

        public void SetPlayerData(PhotonPlayer player, PlayerState data)
        {
            this.Players[player.name] = data;
        }
    }

    public class PlayerState {
        public float SleepStatus;
        public float AcademicStatus;
        public float SocialStatus;
        public Vector3 Location;

        public PlayerState(float sleepStatus, float academicStatus, float socialStatus, Vector3 location)
        {
            this.SleepStatus = sleepStatus;
            this.AcademicStatus = academicStatus;
            this.SocialStatus = socialStatus;
            this.Location = location;
        }
    }

    public class UserSettings {
        public string Name;

        public UserSettings()
        {
            this.Name = "skidrow";
        }
        public UserSettings(string name)
        {
            this.Name = name;
        }
    }

    public GameState SessionState = new GameState();
    private UserSettings Settings;

    void Start()
    {
        PhotonNetwork.OnEventCall += this.OnPhotonEvent;

        if (IOManager.FileExists(GameConstants.USER_SETTINGS_FILE))
            { this.Settings = IOManager.ReadFromFile<UserSettings>(GameConstants.USER_SETTINGS_FILE); }
        else
            { this.Settings = new UserSettings(); }
    }

    // Handle raised photon events
    private void OnPhotonEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == (byte)PhotonManager.EVENT_CODES.REQUEST_LOAD_FINISHED)
        {
            PhotonPlayer sender = PhotonPlayer.Find(senderid);
            PlayerState state = this.SessionState.GetPlayerData(sender);
            byte playerDataCode = (byte)PhotonManager.EVENT_CODES.RECEIVE_PLAYER_DATA;
            RaiseEventOptions options = new RaiseEventOptions();
            options.TargetActors = new int[] { senderid };
            PhotonNetwork.RaiseEvent(playerDataCode, state, true, options);
        }

        else if (eventcode == (byte)PhotonManager.EVENT_CODES.RECEIVE_PLAYER_DATA)
        {
            PlayerState state = (PlayerState)content;

            if (state != null)
                { PlayerManager.Respawn(state.SleepStatus, state.AcademicStatus, state.SocialStatus, state.Location); }
            else
                { PlayerManager.Respawn(); }

            GUIManager.MajorFadeToClear(null);

            if (PhotonNetwork.isMasterClient)
            {
                // Begin the autosaving
                this.SetAutosaveInterval(this.SessionState.AutosaveInterval, this.SessionState.AutosaveUnit);
            }
        }
    }

    public void SetAutosaveInterval(int interval, AUTOSAVE_UNIT unit)
    {
        CancelInvoke("SaveSessionState");   // Stop the repeating invoke for the previous interval

        int intervalSeconds = 60;          // 60s = 1m
        switch (unit)
        {
            case AUTOSAVE_UNIT.SECONDS: intervalSeconds = interval;         break;
            case AUTOSAVE_UNIT.MINUTES: intervalSeconds = interval*60;      break;
            case AUTOSAVE_UNIT.HOURS:   intervalSeconds = interval*60*60;   break;
        }

        this.SessionState.AutosaveInterval = interval;
        this.SessionState.AutosaveUnit = unit;
        InvokeRepeating("SaveSessionState", 0, intervalSeconds);
    }

    public void SetAutosaveInterval(Text inputInterval, Dropdown inputUnit)
    {
        int interval;
        int.TryParse(inputInterval.text, out interval);
        this.SetAutosaveInterval(interval, (AUTOSAVE_UNIT)inputUnit.value);
    }

    public void SetMaxPlayers(int max)
    {
        this.SessionState.MaxPlayers = max;
        PhotonNetwork.room.maxPlayers = max;
    }

    public void SetMaxPlayers(Text inputMax)
    {
        int maxPlayers;
        int.TryParse(inputMax.text, out maxPlayers);
        this.SetMaxPlayers(maxPlayers);
    }

    public void SetName(string name)
    {
        this.Settings.Name = name;
        this.SaveUserSettings();
    }

    public void SetName(InputField nameInput)
    {
        this.SetName(nameInput.text);
    }

    public static int NextAvailableSaveGameIdentifier()
    {
        SortedList<DateTime,FileInfo> saveFiles = IOManager.GetSavedSessionFiles();
        SortedList<int,int> usedIdentifiers = new SortedList<int,int>();
        foreach (FileInfo f in saveFiles.Values)
        {
            GameState savedState = IOManager.ReadFromFile<GameState>(f.Directory.Name + "/" + f.Name);
            usedIdentifiers.Add(savedState.SaveGameIdentifier,savedState.SaveGameIdentifier);
        }

        int unusedIdentifier = 0;
        foreach (int usedIdentifier in usedIdentifiers.Keys)
        {
            if (unusedIdentifier != usedIdentifier)
                { break; }
            unusedIdentifier++;
        }

        return unusedIdentifier;
    }

    public void SaveSessionState()
    {
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
        {
            Player player = (Player) photonPlayer.TagObject;
            PlayerState playerState;

            if (player == null)
                { playerState = new PlayerState(0, 0, 0, Vector3.zero); }
            else
                { playerState = new PlayerState(player.SleepStatus, player.AcademicStatus, player.SocialStatus, player.transform.position); }

            this.SessionState.SetPlayerData(photonPlayer, playerState);
        }

        IOManager.WriteToFile(GameConstants.PATH_SESSION_SAVES + "/Session" + this.SessionState.SaveGameIdentifier + "." + GameConstants.SUFFIX_SESSION_SAVES, this.SessionState);
    }

    public void SaveUserSettings()
    {
        IOManager.WriteToFile(GameConstants.USER_SETTINGS_FILE, this.Settings);
    }

    void OnLevelWasLoaded(int scene)
    {
        if (scene == 1)
        {
            this.GenerateLevelFinished();
        }
    }

    private void GenerateLevelFinished()
    {
        PhotonNetwork.isMessageQueueRunning = true;
        byte loadFinishedCode = (byte)PhotonManager.EVENT_CODES.REQUEST_LOAD_FINISHED;
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ExitGames.Client.Photon.ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent(loadFinishedCode, null, true, options);
    }

    public void GenerateLevel(long seed)
    {
        // TODO : Generate the level asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("World", LoadSceneMode.Additive);

        StartCoroutine(CallbackOnSceneLoad(asyncLoad, SceneManager.GetSceneByName("World")));

        // Afterwards, request this player's data from the master client to use in spawning
        /*Action loadFinished = () => {
            PhotonNetwork.isMessageQueueRunning = true;
            byte loadFinishedCode = (byte)PhotonManager.EVENT_CODES.REQUEST_LOAD_FINISHED;
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ExitGames.Client.Photon.ReceiverGroup.MasterClient;
            PhotonNetwork.RaiseEvent(loadFinishedCode, null, true, options);
        };*/

        // throw new System.NotImplementedException();
    }

    private IEnumerator CallbackOnSceneLoad(AsyncOperation asyncLoad, Scene scene)
    {
        while(!asyncLoad.isDone)
            { yield return null; }

        this.OnLevelWasLoaded(scene.buildIndex);
    }

    public void InitializeHostGame()
    {
        // If hosting a new game, save the new session
        bool newGame = this.SessionState.SaveGameIdentifier == -1;
        if (newGame)
        {
            this.SessionState.SaveGameIdentifier = GameManager.NextAvailableSaveGameIdentifier();
        }

        this.GenerateLevel(this.SessionState.LevelSeed);
    }

    public void HostGame(SavedGameItem savedGameItem, bool useCustomSeed)
    {
        System.Guid guid = System.Guid.NewGuid();
        string uniqueRoomName = guid.ToString();

        if (string.IsNullOrEmpty(savedGameItem.State.RoomName) || savedGameItem.State.RoomName.Trim().Length == 0)
        {
            savedGameItem.State.RoomName = guid.ToString();
        }
        this.SessionState = savedGameItem.State;

        if (!useCustomSeed)
        {
            // Use two floats to get 64 bits in total, combine them, and use their long representation as a seed
            byte[] lo32 = BitConverter.GetBytes(UnityEngine.Random.value);
            byte[] hi32 = BitConverter.GetBytes(UnityEngine.Random.value);
            byte[] all64 = new byte[lo32.Length + hi32.Length];
            lo32.CopyTo(all64, 0);
            hi32.CopyTo(all64, lo32.Length);
            long randomSeed = BitConverter.ToInt64(all64, 0);
            this.SessionState.LevelSeed = randomSeed;
        }

        // Setup room settings
        RoomOptions options = new RoomOptions();
        options.cleanupCacheOnLeave = false;
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add(GameConstants.KEY_ROOMNAME,this.SessionState.RoomName);
        properties.Add(GameConstants.KEY_SEED,this.SessionState.LevelSeed);
        options.customRoomProperties = (ExitGames.Client.Photon.Hashtable) properties;
        options.customRoomPropertiesForLobby = new string[] { GameConstants.KEY_ROOMNAME, GameConstants.KEY_SEED };

        // Fade to black and create a room as callback
        GUIManager.MajorFadeToBlack( () => PhotonNetwork.CreateRoom(uniqueRoomName, options ,null) );
    }

    public void HostGame(SavedGameItem savedGameItem, Toggle customSeedToggle)
    {
        this.HostGame(savedGameItem, customSeedToggle.isOn);
    }

    public void HostGame(SavedGameItem savedGameItem)
    {
        this.HostGame(savedGameItem, false);
    }
}
