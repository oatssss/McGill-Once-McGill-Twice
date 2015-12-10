using UnityEngine;

public class CustomInputManager
{

    public static bool GetButton(string buttonName)
        { return Input.GetButton(buttonName); }

    public static bool GetButtonDown(string buttonName)
        { return Input.GetButtonDown(buttonName); }

    public static bool GetButtonUp(string buttonName)
        { return Input.GetButtonUp(buttonName); }
}
