using System;
using UnityEngine;

public class FieldSettings : MonoBehaviour {

    [SerializeField]
    private string mSettingName;

    [SerializeField]
    private FieldType mType;

    void Start()
    {
        var settings = MonoSingleton<SettingsManager>.Instance.Settings;
        var property = settings.GetType().GetProperty(mSettingName);
        switch (mType)
        {
            case FieldType.Int:
                var intField = GetComponent<IntField>();
                intField.Value((int)property.GetMethod.Invoke(settings, null));
                var setMethodInt = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), settings, property.SetMethod);
                intField.OnValueChanged.AddListener(x => setMethodInt(x));
                break;
            case FieldType.Float:
                var floatField = GetComponent<FloatField>();
                floatField.Value((float) property.GetMethod.Invoke(settings, null));
                var setMethodFloat = (Action<float>) Delegate.CreateDelegate(typeof(Action<float>), settings, property.SetMethod);
                floatField.OnValueChanged.AddListener(x => setMethodFloat(x));
                break;
            case FieldType.Vector3:
                var vector3Field = GetComponent<Vector3Field>();
                vector3Field.Value((Vector3)property.GetMethod.Invoke(settings, null));
                var setMethodVector3 = (Action<Vector3>)Delegate.CreateDelegate(typeof(Action<Vector3>), settings, property.SetMethod);
                vector3Field.OnValueChanged.AddListener(x => setMethodVector3(x));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public enum FieldType
    {
        Int = 0,
        Float = 1,
        Vector2 = 2,
        Vector3 = 3,
        Vector4 = 4
    }

}
