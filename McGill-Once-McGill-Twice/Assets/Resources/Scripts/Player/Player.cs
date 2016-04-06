using UnityEngine;
using System;
using System.Collections;

public class Player : Photon.PunBehaviour {

    [SerializeField] private ThirdPersonCharacterCustom _ThirdPersonCharacter;
    public ThirdPersonCharacterCustom ThirdPersonCharacter
    {
        get
        {
            if (_ThirdPersonCharacter == null)
            {
                GameObject player = GameObject.FindWithTag(GameConstants.TAG_MAINPLAYER);
                if (player != null)
                {
                    ThirdPersonCharacterCustom playerScript = player.GetComponent<ThirdPersonCharacterCustom>();
                    if (playerScript != null)
                        { return playerScript; }
                }

                Debug.LogErrorFormat("{0} could not obtain the player's third person character script.", this);
                return null;
            }
            else
            {
                return _ThirdPersonCharacter;
            }
        }
    }

    [Range(0f, 100f)] [SerializeField] private float _SleepStatus = 100f;
    [Range(0f, 100f)] [SerializeField] private float _AcademicStatus = 100f;
    [Range(0f, 100f)] [SerializeField] private float _SocialStatus = 100f;
    public float SleepStatus { get { return _SleepStatus; } set { _SleepStatus = Mathf.Clamp(value, 0f, 100f); } }
    public float AcademicStatus { get { return _AcademicStatus; } set { _AcademicStatus = Mathf.Clamp(value, 0f, 100f); } }
    public float SocialStatus { get { return _SocialStatus; } set { _SocialStatus = Mathf.Clamp(value, 0f, 100f); } }

	// Use this for initialization
	void OnEnable () {

	}

	void Update () {
        //  Continually check if the player's dead
        if (PlayerManager.IsPlayerDead(this))
            { Die(); }
	}

    public void Die()
    {
        //  TODO : Trigger death animation and disable user controls
        //  TODO : Display death/respawn UI
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.SleepStatus);
            stream.SendNext(this.AcademicStatus);
            stream.SendNext(this.SocialStatus);
        }
        else
        {
            // Network player, receive data
            this.SleepStatus = (float) stream.ReceiveNext();
            this.AcademicStatus = (float) stream.ReceiveNext();
            this.SocialStatus = (float) stream.ReceiveNext();
        }
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.sender != null)
            { info.sender.TagObject = this.photonView; }
    }
}
