using UnityEngine;
using UnityEditor;
using Framework.EditorTools;

namespace LevelGen
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LightProfile))]
    public class GeneratedLightEditor : Editor
    {
        SerializedProperty m_lightMode;
        SerializedProperty m_colors;
        SerializedProperty m_maxIntensity;
        SerializedProperty m_minIntensity;
        SerializedProperty m_intensityChangeRate;
        SerializedProperty m_flickerChance;
        SerializedProperty m_flickerMinDuration;
        SerializedProperty m_flickerMaxDuration;

        private void OnEnable()
        {
            m_lightMode = serializedObject.FindProperty("lightMode");
            m_colors = serializedObject.FindProperty("colors");
            m_maxIntensity = serializedObject.FindProperty("maxIntensity");
            m_minIntensity = serializedObject.FindProperty("minIntensity");
            m_intensityChangeRate = serializedObject.FindProperty("intensityChangeRate");
            m_flickerChance = serializedObject.FindProperty("flickerChance");
            m_flickerMaxDuration = serializedObject.FindProperty("flickerMaxDuration");
            m_flickerMinDuration = serializedObject.FindProperty("flickerMinDuration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_lightMode);
            EditorExtentions.DrawList(m_colors);
            EditorGUILayout.PropertyField(m_maxIntensity);

            switch (m_lightMode.intValue)
            {
                case (int)GeneratedLight.LightMode.Flicker:
                    EditorGUILayout.PropertyField(m_minIntensity);
                    EditorGUILayout.PropertyField(m_flickerChance);
                    EditorGUILayout.PropertyField(m_flickerMaxDuration);
                    EditorGUILayout.PropertyField(m_flickerMinDuration);
                    break;
                case (int)GeneratedLight.LightMode.Sine:
                case (int)GeneratedLight.LightMode.RandomFluctuation:
                    EditorGUILayout.PropertyField(m_minIntensity);
                    EditorGUILayout.PropertyField(m_intensityChangeRate);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
