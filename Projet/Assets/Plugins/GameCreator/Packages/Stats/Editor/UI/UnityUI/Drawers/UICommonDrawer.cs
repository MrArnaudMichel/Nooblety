using GameCreator.Editor.Common;
using GameCreator.Runtime.Stats.UnityUI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Stats.UnityUI
{
    [CustomPropertyDrawer(typeof(UICommon))]
    public class UICommonDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            root.Add(new LabelTitle("Graphics:"));
            root.Add(new PropertyField(property.FindPropertyRelative("m_Icon")));
            root.Add(new PropertyField(property.FindPropertyRelative("m_Color")));
            
            root.Add(new SpaceSmall());
            root.Add(new LabelTitle("Texts:"));
            root.Add(new PropertyField(property.FindPropertyRelative("m_Name")));
            root.Add(new PropertyField(property.FindPropertyRelative("m_Acronym")));
            root.Add(new PropertyField(property.FindPropertyRelative("m_Description")));
            
            return root;
        }
    }
}