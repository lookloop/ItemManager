using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lookloop.ItemManager.Editor
{
    [CustomPropertyDrawer(typeof(ContainerSpec))]
    public class ContainerSpecDrawer : PropertyDrawer
    {
        static readonly Dictionary<string, bool> Foldouts = new();

        // Lazily‑populated cache of all concrete SetItemBase subclasses
        static Type[] filterTypes;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Prefab and detail panel — always visible
            EditorGUILayout.PropertyField(property.FindPropertyRelative("prefabRect"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("detailRect"));

            // Item filter — manually draw the [SerializeReference] type selector
            DrawFilterField(property.FindPropertyRelative("itemFilter"));

            // Collapsible foldout triangle
            string key = property.propertyPath;
            if (!Foldouts.ContainsKey(key)) Foldouts[key] = false;
            Foldouts[key] = EditorGUILayout.Foldout(Foldouts[key], "参数");

            // Auto‑fill defaults for a newly added spec element
            if (property.FindPropertyRelative("totalItems").intValue == 0)
            {
                property.FindPropertyRelative("totalItems").intValue = 80;
                property.FindPropertyRelative("everyPageCells").intValue = 40;
                property.FindPropertyRelative("row").intValue = 5;
                property.FindPropertyRelative("maskHeight").floatValue = 40f;
                property.FindPropertyRelative("containerFillHorizontal").floatValue = 2f;
                property.FindPropertyRelative("containerFillUp").floatValue = 8f;
                property.FindPropertyRelative("containerFillDown").floatValue = 4f;
                property.FindPropertyRelative("pageTextWidth").floatValue = 24f;
                property.FindPropertyRelative("pageTextHeight").floatValue = 4f;
            }

            if (Foldouts[key])
            {
                Prop(property, "totalItems",            "Total Items (default 80)");
                Prop(property, "everyPageCells",        "Cells Per Page (default 40)");
                Prop(property, "row",                   "Row Count (default 5)");
                Prop(property, "maskHeight",            "Mask Height (default 40)");
                Prop(property, "containerFillHorizontal","Horizontal Padding (default 2)");
                Prop(property, "containerFillUp",       "Top Padding (default 8)");
                Prop(property, "containerFillDown",     "Bottom Padding (default 4)");
                Prop(property, "pageTextWidth",         "Page Input Width (default 24)");
                Prop(property, "pageTextHeight",        "Page Input Height (default 4)");
                Prop(property, "containerSprite");
                Prop(property, "maskSprite");
                Prop(property, "cellSprite");
            }
        }

        // ── Custom [SerializeReference] type picker ──
        void DrawFilterField(SerializedProperty prop)
        {
            CacheFilterTypes();

            object current = prop.managedReferenceValue;
            int currentIndex = -1;
            string[] names;

            if (filterTypes.Length == 0)
            {
                // No subclasses found → show a placeholder message
                names = new[] { "(No types available)" };
            }
            else
            {
                names = new string[filterTypes.Length + 1];
                names[0] = "(None)";
                for (int i = 0; i < filterTypes.Length; i++)
                {
                    names[i + 1] = filterTypes[i].Name;
                    if (current != null && current.GetType() == filterTypes[i])
                        currentIndex = i + 1;
                }
            }

            EditorGUI.BeginChangeCheck();
            int selected = EditorGUILayout.Popup(
                "Item Filter",
                currentIndex == -1 ? 0 : currentIndex,
                names);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(prop.serializedObject.targetObject, "Change Item Filter");
                if (selected <= 0 || filterTypes.Length == 0)
                    prop.managedReferenceValue = null;
                else
                    prop.managedReferenceValue = Activator.CreateInstance(filterTypes[selected - 1]);
            }

            // When a type is selected, expand its child fields inline
            if (current != null)
            {
                EditorGUI.indentLevel++;
                SerializedProperty child = prop.Copy();
                SerializedProperty end = child.GetEndProperty();
                child.NextVisible(true);
                while (!SerializedProperty.EqualContents(child, end))
                {
                    EditorGUILayout.PropertyField(child, true);
                    child.NextVisible(false);
                }
                EditorGUI.indentLevel--;
            }
        }

        static void CacheFilterTypes()
        {
            if (filterTypes != null) return;

            var list = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t.IsSubclassOf(typeof(SetItemBase)) && !t.IsAbstract)
                        list.Add(t);
                }
            }
            filterTypes = list.ToArray();
        }

        static void Prop(SerializedProperty parent, string name, string label = null)
        {
            var prop = parent.FindPropertyRelative(name);
            if (label != null)
                EditorGUILayout.PropertyField(prop, new GUIContent(label), true);
            else
                EditorGUILayout.PropertyField(prop, true);
        }
    }
}
