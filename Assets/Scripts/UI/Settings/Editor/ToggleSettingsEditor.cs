using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ToggleSettings))]
public class ToggleSettingsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var property = serializedObject.FindProperty("mSettingName");
        var names = typeof(Settings).GetProperties().Where(x => x.PropertyType == typeof(bool)).Select(x => x.Name).ToArray();

        var index = EditorGUILayout.Popup(names.IndexOf(property.stringValue), names);
        if (index >= 0)
        {
            property.stringValue = names[index];
            serializedObject.ApplyModifiedProperties();
        }
    }
}
