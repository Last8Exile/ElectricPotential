using UnityEngine;
using UnityEngine.UI;

public class DropDownField : IntField {

    [SerializeField]
    private Dropdown mDropdown = null;

    public override void Value(int value)
    {
        mDropdown.value = value;
    }

    public void Set(int value)
    {
        OnValueChanged?.Invoke(value);
    }
}
