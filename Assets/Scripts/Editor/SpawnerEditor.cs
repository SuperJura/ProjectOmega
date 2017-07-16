using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor {

    Spawner spawner;

    private void OnEnable()
    {
        spawner = (Spawner)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        int totalExperience = 0;
        int totalLevel = 0;
        if(spawner.enemiesToSpawn != null)
        {
            for (int i = 0; i < spawner.enemiesToSpawn.Length; i++)
            {
                totalExperience += spawner.enemiesToSpawn[i].expDrop;
                totalLevel += spawner.enemiesToSpawn[i].types.Length;
            }
        }
        EditorGUILayout.Separator();
        GUILayout.Label("Total Experience: " + totalExperience);
        GUILayout.Label("Total magic types: " + totalLevel);
    }
}
