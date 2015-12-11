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
        int emptyBars = 0;
        
        if (this.SleepStatus <= 0)
            { emptyBars++; }
        if (this.AcademicStatus <= 0)
            { emptyBars++; }
        if (this.SocialStatus <= 0)
            { emptyBars++; }
            
        if (emptyBars >= 2)
            { Die(); }
	}
    
    public void Die()
    {
        //  TODO : Trigger death animation and disable user controls
        //  TODO : Display death/respawn UI
        Destroy(gameObject);
    }
}
