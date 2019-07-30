using UnityEngine;
using UnityEngine.UI;

public class FloatField : MonoBehaviour
{
    public FloatEvent OnValueChanged;

    [SerializeField]
    private InputField mX = null;

    public void Value(float value)
    {
        mX.text = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public void Set(string value)
    {
        OnValueChanged.Invoke(float.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
    }
}
