using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SettingsView : LiveMenuView
{
    [SerializeField] private InputField Username;
    [SerializeField] private Dropdown Sex;

    public override void Activate()
    {
        this.Username.text = GameManager.Instance.Settings.Name;
        this.Sex.value = (int)GameManager.Instance.Settings.Sex;

        base.Activate();
    }

    protected override void Update()
    {
        // Do nothing
    }
}
