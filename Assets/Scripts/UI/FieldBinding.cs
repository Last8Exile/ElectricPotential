using System;
using UnityEngine;

public class FieldBinding : MonoBehaviour {

    [SerializeField]
    private string mSettingName;

    [SerializeField]
    private MonoBehaviour mTarget;

    [SerializeField]
    private FieldSettings.FieldType mType;

    void Start()
    {
        var property = mTarget.GetType().GetProperty(mSettingName);
        switch (mType)
        {
            case FieldSettings.FieldType.Int:
                var intField = GetComponent<IntField>();
                intField.Value((int)property.GetMethod.Invoke(mTarget, null));
                var setMethodInt = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), mTarget, property.SetMethod);
                intField.OnValueChanged.AddListener(x => setMethodInt(x));
                break;
            case FieldSettings.FieldType.Float:
                var floatField = GetComponent<FloatField>();
                floatField.Value((float)property.GetMethod.Invoke(mTarget, null));
                var setMethodFloat = (Action<float>)Delegate.CreateDelegate(typeof(Action<float>), mTarget, property.SetMethod);
                floatField.OnValueChanged.AddListener(x => setMethodFloat(x));
                break;
            case FieldSettings.FieldType.Vector3:
                var vector3Field = GetComponent<Vector3Field>();
                vector3Field.Value((Vector3)property.GetMethod.Invoke(mTarget, null));
                var setMethodVector3 = (Action<Vector3>)Delegate.CreateDelegate(typeof(Action<Vector3>), mTarget, property.SetMethod);
                vector3Field.OnValueChanged.AddListener(x => setMethodVector3(x));
                break;
            case FieldSettings.FieldType.Vector4:
                var vector4Field = GetComponent<Vector4Field>();
                vector4Field.Value((Vector4)property.GetMethod.Invoke(mTarget, null));
                var setMethodVector4 = (Action<Vector4>)Delegate.CreateDelegate(typeof(Action<Vector4>), mTarget, property.SetMethod);
                vector4Field.OnValueChanged.AddListener(x => setMethodVector4(x));
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
