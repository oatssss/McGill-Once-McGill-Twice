using UnityEngine;

public class PlayerManager : UnitySingleton<PlayerManager> {
    
    [SerializeField] private static Player _MainPlayer;
    public static Player MainPlayer
    {
        get
        {
            if (_MainPlayer == null)
            {
                GameObject player = GameObject.FindWithTag(GameConstants.TAG_MAINPLAYER);
                if (player != null)
                {
                    Player playerScript = player.GetComponent<Player>();
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
