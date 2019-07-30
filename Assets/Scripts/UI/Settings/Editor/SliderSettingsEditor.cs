using System;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SliderSettings))]
public class SliderSettingsEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var property = serializedObject.FindProperty("mSettingName");
        var typeInd = serializedObject.FindProperty("mType").enumValueIndex;
        Type type;
        switch (typeInd)
        {
            case 0:
                type = typeof(float);
                break;
            case 1:
                type = typeof(int);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var names = typeof(Settings).GetProperties().Where(x => x.PropertyType == type).Select(x => x.Name).ToArray();

        var index = EditorGUILayout.Popup(names.IndexOf(property.stringValue), names);
        if (index >= 0)
        {
            property.stringValue = names[index];
            serializedObject.ApplyModifiedProperties();
        }
    }
}
