using UnityEngine;
using UnityEngine.UI;

public class IntField : MonoBehaviour {

    public IntEvent OnValueChanged;

    [SerializeField]
    private InputField mX = null;

    public virtual void Value(int value)
    {
        mX.text = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public void Set(string value)
    {
        OnValueChanged.Invoke(int.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
    }
}
