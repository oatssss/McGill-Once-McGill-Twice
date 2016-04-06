using UnityEngine;
using System.Collections;

public class GameConstants {

    // IO
    public static readonly string PATH_SESSION_SAVES = "Sessions";
    public static readonly string SUFFIX_SESSION_SAVES = "momt";
    public static readonly string USER_SETTINGS_FILE = "user.settings";

    // Static list of tags
    public static readonly string TAG_UNTAGGED = "Untagged";
    public static readonly string TAG_RESPAWN = "Respawn";
    public static readonly string TAG_FINISH = "Finish";
    public static readonly string TAG_EDITORONLY = "EditorOnly";
    public static readonly string TAG_MAINCAMERA = "MainCamera";
    public static readonly string TAG_MAINPLAYER = "MainPlayer";
    public static readonly string TAG_GAMECONTROLLER = "GameController";
    public static readonly string TAG_CAMERARIG = "CamerRig";
    public static readonly string TAG_GROUND = "Ground";
    public static readonly string TAG_OBSTACLE = "Obstacle";

    // Static list of layers
    public static readonly string LAYER_PLAYER = "Player";

    // Static list of assets
    public static readonly string ASSET_MALCOLM = "Prefabs/Characters/Malcolm/Malcolm";
    public static readonly string ASSET_ANDROMEDA = "Prefabs/Characters/Andromeda/Andromeda";

    // Misc

    public static readonly string KEY_ROOMNAME = "n";
    public static readonly string KEY_SEED = "s";

    public static readonly float TRANSITION_SPEED_MULTIPLIER = 7f;
    public static readonly float DELAYED_INPUT_SWITCH_DURATION = 0.2f;
}
