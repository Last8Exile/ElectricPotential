using UnityEngine;
using UnityEngine.UI;

public class Vector4Field : MonoBehaviour
{
    public Vector4Event OnValueChanged;

    private Vector4 mValue;

    [SerializeField]
    private InputField mX = null, mY = null, mZ = null, mW = null;

    public void SetX(string value) => Set(Axis.x, value);
    public void SetY(string value) => Set(Axis.y, value);
    public void SetZ(string value) => Set(Axis.z, value);
    public void SetW(string value) => Set(Axis.w, value);

    public void Value(Vector4 value)
    {
        mX.text = value.x.ToString(System.Globalization.CultureInfo.InvariantCulture);
        mY.text = value.y.ToString(System.Globalization.CultureInfo.InvariantCulture);
        mZ.text = value.z.ToString(System.Globalization.CultureInfo.InvariantCulture);
        mW.text = value.w.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private void Set(Axis axis, string value)
    {
        mValue = mValue.Change(axis, float.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
        OnValueChanged.Invoke(mValue);
    }
}
