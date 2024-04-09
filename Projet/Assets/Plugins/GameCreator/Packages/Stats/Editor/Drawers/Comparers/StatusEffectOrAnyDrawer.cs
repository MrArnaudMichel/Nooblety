using GameCreator.Editor.Common;
using GameCreator.Runtime.Stats;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Stats
{
    [CustomPropertyDrawer(typeof(StatusEffectOrAny))]
    public class StatusEffectOrAnyDrawer : PropertyDrawer
    {
        private const string EMPTY_LABEL = " ";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();

            SerializedProperty option = property.FindPropertyRelative("m_Option");
            SerializedProperty statusEffect = property.FindPropertyRelative("m_StatusEffect");
            
            PropertyField fieldOption = new PropertyField(option, property.displayName);
            PropertyField fieldStatusEffect = new PropertyField(statusEffect, EMPTY_LABEL);
            
            head.Add(fieldOption);
            
            fieldOption.RegisterValueChangeCallback(changeEvent =>
            {
                body.Clear();
                if (changeEvent.changedProperty.intValue != 1) return;
                
                body.Add(fieldStatusEffect);
                body.Bind(changeEvent.changedProperty.serializedObject);
            });

            if (option.intValue == 1)
            {
                body.Add(fieldStatusEffect);
            }

            root.Add(head);
            root.Add(body);
            
            return root;
        }
    }
}
