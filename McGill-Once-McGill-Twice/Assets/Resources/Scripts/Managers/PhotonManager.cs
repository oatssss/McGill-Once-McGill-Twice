using UnityEngine;

public class PhotonManager : PUNSingletonPersistent<PhotonManager> {

    public enum EVENT_CODES { REQUEST_LOAD_FINISHED, RECEIVE_PLAYER_DATA }

    void Start()
    {
        PhotonNetwork.autoCleanUpPlayerObjects = false;
        this.ConnectToPhoton();
    }

    // void OnGUI()
    // {
    //     GUI.Label(new Rect(10, 10, 100, 20), PhotonNetwork.connectionStateDetailed.ToString());
    // }

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

    public override void OnJoinedLobby()
    {
        if (GameManager.Instance.DebugMode)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }


    public override void OnJoinedRoom()
    {
        if (!GameManager.Instance.DebugMode)
        {
            PhotonNetwork.isMessageQueueRunning = false;
        }

        GUIManager.Instance.ChatUI.CanvasGroup.alpha = 1f;
        GUIManager.Instance.ChatUI.CanvasGroup.interactable = true;
        GUIManager.Instance.ChatUI.CanvasGroup.blocksRaycasts = true;
    }

    public override void OnLeftRoom()
    {
        GUIManager.Instance.ChatUI.CanvasGroup.alpha = 0f;
        GUIManager.Instance.ChatUI.CanvasGroup.interactable = false;
        GUIManager.Instance.ChatUI.CanvasGroup.blocksRaycasts = false;
    }

    public override void OnCreatedRoom()
    {
        if (!GameManager.Instance.DebugMode)
        {
            GameManager.Instance.InitializeHostGame();
        }
    }

    public static void SendChatMessage(string message)
    {
        Instance.photonView.RPC("ReceiveChatMessage", PhotonTargets.AllViaServer, message);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonView playerView = (PhotonView)otherPlayer.TagObject;
            int viewID = playerView.viewID;
            this.photonView.RPC("CleanupPlayerAvatar", PhotonTargets.AllBufferedViaServer, viewID);
        }
    }

    [PunRPC]
    private void CleanupPlayerAvatar(int avatarViewID)
    {
        PhotonView photonView = PhotonView.Find(avatarViewID);
        Destroy(photonView.gameObject);
    }

    [PunRPC]
    private void ReceiveChatMessage(string message, PhotonMessageInfo info)
    {
        GUIManager.Instance.ChatUI.AddChatMessageText(info.sender.name, message);
    }
}
