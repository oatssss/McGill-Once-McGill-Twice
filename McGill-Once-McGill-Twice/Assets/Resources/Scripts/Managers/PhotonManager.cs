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
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
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
        // GameObject playerPrefab = Resources.Load("Prefabs/Characters/Malcolm/Malcolm") as GameObject;
        /*GameObject player = PhotonNetwork.Instantiate("Prefabs/Characters/Malcolm/Malcolm", Vector3.zero, Quaternion.identity, 0);*/
        // PhotonView playerView = player.GetComponent<PhotonView>();
        // playerView.view
        // GameObject monster = PhotonNetwork.Instantiate("monsterprefab", Vector3.zero, Quaternion.identity, 0);
        // CharacterControl controller = monster.GetComponent<CharacterControl>();
        // controller.enabled = true;
        // CharacterCamera camera = monster.GetComponent<CharacterCamera>();
        // camera.enabled = true;
    }
}
