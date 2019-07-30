using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ToggleField : IntField
{
    [SerializeField]
    private ToggleGroup mToggleGroup = null;

    //public IntEvent OnValueChanged;

    public override void Value(int value)
    {
        GetComponentsInChildren<Toggle>().First(x => x.gameObject.name == value.ToString()).isOn = true;
    }

    public void SomeChanged(bool value)
    {
        if (value)
            SetUpate();
    }

    public void SetUpate()
    {
        OnValueChanged.Invoke(int.Parse(mToggleGroup.ActiveToggles().First().gameObject.name));
    }

}
