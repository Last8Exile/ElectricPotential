﻿using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(InspectorButton))]
public class InspectorButtonEditor : Editor {

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
		if (Application.isPlaying && GUILayout.Button ("Execute")) 
		{
			(target as InspectorButton).OnClick.Invoke ();
		}
	}

}
