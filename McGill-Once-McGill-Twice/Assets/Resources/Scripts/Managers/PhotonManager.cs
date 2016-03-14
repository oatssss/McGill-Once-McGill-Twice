using UnityEngine;
using System.Collections;

public class PhotonManager : Photon.PunBehaviour {

	// Use this for initialization
    void Start()
    {
        PhotonNetwork.autoCleanUpPlayerObjects = false;
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), PhotonNetwork.connectionStateDetailed.ToString());
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
        PlayerManager.Respawn();
        PhotonNetwork.playerName = "Oats";
    }
}
