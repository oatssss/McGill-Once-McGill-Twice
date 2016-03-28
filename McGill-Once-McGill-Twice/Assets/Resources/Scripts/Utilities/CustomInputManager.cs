using UnityEngine;
using System.Collections;

public class CustomInputManager
{
    public enum InputMode { Menu, Gameplay }
    private static InputMode mode;
    public static InputMode Mode {
        get { return mode; }
        set {
            if (DelayedSwitch != null)
                { GameManager.Instance.StopCoroutine(DelayedSwitch); }

            if (value == InputMode.Gameplay && mode == InputMode.Menu)    // Have a little delay when transitioning from menu to gameplay
                { DelayedSwitch = GameManager.Instance.StartCoroutine(DelayInputModeSwitch(value)); }
            else
                { mode = value; }
        }
    }

    private static Coroutine DelayedSwitch;

    public static IEnumerator DelayInputModeSwitch(InputMode switchTo)
    {
        float elapsedTime = 0;
        while (elapsedTime <= GameConstants.DELAYED_INPUT_SWITCH_DURATION)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        mode = switchTo;
        DelayedSwitch = null;
    }

    public static bool GetButton(string buttonName)
        { return Input.GetButton(buttonName); }

    public static bool GetButtonDown(string buttonName)
        { return Input.GetButtonDown(buttonName); }

    public static bool GetButtonUp(string buttonName)
        { return Input.GetButtonUp(buttonName); }

    public static bool GetButtonDown(string buttonName, InputMode inputMode)
        { return Mode == inputMode && Input.GetButtonDown(buttonName); }

    public static bool GetButtonUp(string buttonName, InputMode inputMode)
        { return Mode == inputMode && Input.GetButtonUp(buttonName); }

    public static bool GetButton(string buttonName, InputMode inputMode)
        { return Mode == inputMode && Input.GetButton(buttonName); }
}
