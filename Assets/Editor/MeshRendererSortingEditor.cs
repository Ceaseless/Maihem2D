using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Maihem.Editor
{
    /// This just exposes the Sorting Layer / Order in MeshRenderer since it's there
    /// but not displayed in the inspector. Getting MeshRenderer to render in front
    /// of a SpriteRenderer is pretty hard without this.
    /// Adapted from https://gist.github.com/sinbad/bd0c49bc462289fa1a018ffd70d806e3
    /// With changes from https://forum.unity.com/threads/extending-mesh-renderer-component-with-a-custom-editor.949176/
    /// to preserve the Unity MeshRenderer GUI. 
    [CustomEditor(typeof(MeshRenderer))]
    [CanEditMultipleObjects]
    public class MeshRendererSortingEditor : UnityEditor.Editor
    {
        private UnityEditor.Editor _defaultEditor;
        private MeshRenderer _meshRenderer;
        private static bool _showSorting = true;
        private const string Header = "2D Sorting";

        private SerializedProperty _sortingLayerIdProperty;
        private SerializedProperty _sortingOrderProperty;

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            _showSorting = EditorPrefs.GetBool("MeshRendererSortingEditor.showSorting");
        }

        private void OnEnable()
        {
            _defaultEditor = CreateEditor(targets, Type.GetType("UnityEditor.MeshRendererEditor, UnityEditor"));
            _meshRenderer = target as MeshRenderer;

            _sortingLayerIdProperty = serializedObject.FindProperty("m_SortingLayerID");
            _sortingOrderProperty = serializedObject.FindProperty("m_SortingOrder");
        }

        private void OnDisable()
        {
            //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
            //Also, make sure to call any required methods like OnDisable
            var disableMethod = _defaultEditor.GetType().GetMethod("OnDisable",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (disableMethod != null)
                disableMethod.Invoke(_defaultEditor, null);
            DestroyImmediate(_defaultEditor);
        }

        public override void OnInspectorGUI()
        {
            _defaultEditor.OnInspectorGUI();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            _showSorting = EditorGUILayout.BeginFoldoutHeaderGroup(_showSorting, Header);
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool("MeshRendererSortingEditor.showSorting", _showSorting);

            if (_showSorting)
            {
                EditorGUI.indentLevel++;

                var rect = EditorGUILayout.GetControlRect();
                EditorGUI.BeginProperty(rect, new GUIContent("Sorting Layer"), _sortingLayerIdProperty);
                EditorGUI.BeginChangeCheck();
                var newId = DrawSortingLayersPopup(rect, _meshRenderer.sortingLayerID);
                if (EditorGUI.EndChangeCheck())
                    _sortingLayerIdProperty.intValue = newId;
                EditorGUI.EndProperty();

                EditorGUILayout.PropertyField(_sortingOrderProperty);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private static int DrawSortingLayersPopup(Rect rect, int layerID)
        {
            var layers = SortingLayer.layers;
            var names = layers.Select(l => l.name).ToArray();

            if (!SortingLayer.IsValid(layerID))
                layerID = SortingLayer.NameToID("Default");

            var index = 0;
            for (int i = 0; i < layers.Length; i++) //No IndexOf in LINQ unfortunately
                if (layers[i].id == layerID)
                    index = i;

            index = EditorGUI.Popup(rect, "Sorting Layer", index, names);

            return layers[index].id;
        }
    }

// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org/>
}