using UnityEngine;

public class PlayerManager : UnitySingleton<PlayerManager> {
    
    [SerializeField] private ThirdPersonCharacterCustom _MainPlayer;    
    public ThirdPersonCharacterCustom MainPlayer
    {
        get
        {
            if (_MainPlayer == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    ThirdPersonCharacterCustom playerScript = player.GetComponent<ThirdPersonCharacterCustom>();
                    if (playerScript != null)
                        { return playerScript; }
                }
    
                Debug.LogErrorFormat("{0} could not obtain the player's script.", this);
                return null;
            }
            else
            {
                return _MainPlayer;
            }
        }
    }
}
