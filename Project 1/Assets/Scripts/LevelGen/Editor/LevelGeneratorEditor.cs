using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelGen
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelGenerator generator = (LevelGenerator)target;
            if (GUILayout.Button("Generate Level"))
            {
                generator.GenerateLevel();
            }
        }
    }
}