using UnityEngine;

public class PlayerManager : UnitySingleton<PlayerManager> {
    
    [SerializeField] private static ThirdPersonCharacterCustom _MainPlayer;    
    public static ThirdPersonCharacterCustom MainPlayer
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
    
                Debug.LogErrorFormat("{0} could not obtain the player's script.", Instance);
                return null;
            }
            else
            {
                return _MainPlayer;
            }
        }
    }
}
