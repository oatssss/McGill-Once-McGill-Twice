using UnityEngine;

public class PhotonManager : PUNSingletonPersistent<PhotonManager> {

    public enum EVENT_CODES { REQUEST_LOAD_FINISHED, RECEIVE_PLAYER_DATA }

    void Start()
    {
        PhotonNetwork.autoCleanUpPlayerObjects = false;
        this.ConnectToPhoton();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), PhotonNetwork.connectionStateDetailed.ToString());
    }

    public void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        GUIManager.Instance.ShowTooltip("Failed to connect online (" + cause + ").");
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        GUIManager.Instance.ShowTooltip("You were disconnected (" + cause + ").");
    }

    /*
    public override void OnJoinedLobby()
    {
        // PhotonNetwork.JoinRandomRoom();
    }


    public void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }
    */

    public override void OnJoinedRoom()
    {
        PhotonNetwork.isMessageQueueRunning = false;
        // PlayerManager.Respawn();
        PhotonNetwork.playerName = "Oats";
    }

    public override void OnCreatedRoom()
    {
        GameManager.Instance.InitializeHostGame();
    }
}
