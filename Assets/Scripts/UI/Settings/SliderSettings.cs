using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderSettings : MonoBehaviour
{
    [SerializeField]
    private string mSettingName;

    [SerializeField]
    private SliderType mType;

    void Start()
    {
        var settings = MonoSingleton<SettingsManager>.Instance.Settings;
        var property = settings.GetType().GetProperty(mSettingName);
        var slider = GetComponent<Slider>();
        switch (mType)
        {
            case SliderType.Float:
                slider.value = (float)property.GetMethod.Invoke(settings, null);
                var setMethodFloat = (Action<float>) Delegate.CreateDelegate(typeof(Action<float>), settings, property.SetMethod);
                slider.onValueChanged.AddListener(x => setMethodFloat(x));
                break;
            case SliderType.Int:
                slider.value = (int)property.GetMethod.Invoke(settings, null);
                var setMethodInt = (Action<int>) Delegate.CreateDelegate(typeof(Action<int>), settings, property.SetMethod);
                slider.onValueChanged.AddListener(x => setMethodInt((int) x));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public enum SliderType
    {
        Float = 0,
        Int = 1
    }

}
