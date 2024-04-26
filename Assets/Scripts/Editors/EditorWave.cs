using UnityEngine;
using System;
using UnityEditor;

[CustomEditor(typeof(EnnemySpawnerGestionary))]
public class EditorWave : Editor
{
    #region SerializedProperty
    private SerializedProperty radiusCircle;

    private SerializedProperty nbSpawnPoint;
    private SerializedProperty nbMonsterSpawn;

    private SerializedProperty currentWave;
    private SerializedProperty waveSettings;

    private SerializedProperty ennemyList;

    private SerializedProperty debug;
    #endregion

    protected virtual void OnEnable()
    {
        radiusCircle = serializedObject.FindProperty("radiusCircle");
        nbSpawnPoint = serializedObject.FindProperty("nbSpawnPoint");
        nbMonsterSpawn = serializedObject.FindProperty("nbMonsterSpawn");

        debug = serializedObject.FindProperty("modDebug");

        currentWave = serializedObject.FindProperty("currentWave");
        waveSettings = serializedObject.FindProperty("waveSettings");
        ennemyList = serializedObject.FindProperty("ennemyList");
    }

    public override void OnInspectorGUI()
    {
        var ennemyManager = (EnnemySpawnerGestionary)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(debug);

        EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(waveSettings);
            EditorGUILayout.PropertyField(currentWave);
        EditorGUILayout.EndHorizontal();

        if (debug.boolValue)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.Slider(radiusCircle, 0, 9999);
            EditorGUILayout.Space(2);
            EditorGUILayout.IntSlider(nbMonsterSpawn, 0, 200);
            EditorGUILayout.Space(2);
            EditorGUILayout.IntSlider(nbSpawnPoint, 0, 20);

            EditorGUILayout.Space(15);
            EditorGUILayout.PropertyField(ennemyList);
            EditorGUILayout.Space(15);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Exemple"))
            {
                ennemyManager.ShowExemple();
            }
            if (GUILayout.Button("Remove"))
            {
                ennemyManager.ResetExemple();
            }
            GUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
