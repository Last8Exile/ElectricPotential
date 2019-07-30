using UnityEngine;
using UnityEngine.UI;

public class Vector3Field : MonoBehaviour
{
    public Vector3Event OnValueChanged;

    private Vector3 mValue;

    [SerializeField]
    private InputField mX = null, mY = null, mZ = null;

    public void SetX(string value) => Set(Axis.x, value);
    public void SetY(string value) => Set(Axis.y, value);
    public void SetZ(string value) => Set(Axis.z, value);

    public void Value(Vector3 value)
    {
        mX.text = value.x.ToString(System.Globalization.CultureInfo.InvariantCulture);
        mY.text = value.y.ToString(System.Globalization.CultureInfo.InvariantCulture);
        mZ.text = value.z.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private void Set(Axis axis, string value)
    {
        mValue = mValue.Change(axis, float.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
        OnValueChanged.Invoke(mValue);
    }
}
