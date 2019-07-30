using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSettings : MonoBehaviour {

    [SerializeField]
    private string mSettingName;
    
    void Start ()
    {
        var settings = MonoSingleton<SettingsManager>.Instance.Settings;
        var property = settings.GetType().GetProperty(mSettingName);
        var setMethod = (Action<bool>)Delegate.CreateDelegate(typeof(Action<bool>), settings, property.SetMethod);

        var toggle = GetComponent<Toggle>();
        toggle.isOn = (bool)property.GetMethod.Invoke(settings, null);
        toggle.onValueChanged.AddListener(x => setMethod(x));
    }
}
