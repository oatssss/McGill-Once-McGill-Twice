using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour {
    
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
    
    [Range(0f, 100f)] public float SleepStatus;
    [Range(0f, 100f)] public float GradesStatus;
    [Range(0f, 100f)] public float SocialStatus;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public void Die()
    {
        throw new NotImplementedException();
    }
}
