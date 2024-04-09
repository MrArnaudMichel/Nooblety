using GameCreator.Editor.Common;
using GameCreator.Editor.Common.UnityUI;
using GameCreator.Runtime.Stats.UnityUI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Stats.UnityUI
{
    [CustomEditor(typeof(FormulaUI))]
    public class FormulaUIEditor : UnityEditor.Editor
    {
        // MEMBERS: -------------------------------------------------------------------------------

        private VisualElement m_Root;
        private VisualElement m_Head;
        private VisualElement m_Body;
        
        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            this.m_Head = new VisualElement();
            this.m_Body = new VisualElement();
            
            this.m_Root.Add(this.m_Head);
            this.m_Root.Add(this.m_Body);

            SerializedProperty formulaSource = this.serializedObject.FindProperty("m_Source");
            SerializedProperty formulaTarget = this.serializedObject.FindProperty("m_Target");
            SerializedProperty formulaAsset = this.serializedObject.FindProperty("m_Formula");

            SerializedProperty value = this.serializedObject.FindProperty("m_Value");
            SerializedProperty ratioFill = this.serializedObject.FindProperty("m_RatioFill");

            PropertyField fieldSource = new PropertyField(formulaSource);
            PropertyField fieldTarget = new PropertyField(formulaTarget);
            PropertyField fieldFormula = new PropertyField(formulaAsset);
            
            this.m_Head.Add(fieldSource);
            this.m_Head.Add(fieldTarget);
            this.m_Head.Add(fieldFormula);

            this.m_Body.Add(new SpaceSmall());
            this.m_Body.Add(new PropertyField(value));
            this.m_Body.Add(new PropertyField(ratioFill));
            
            this.m_Body.SetEnabled(formulaAsset.objectReferenceValue != null);
            fieldFormula.RegisterValueChangeCallback(changeEvent =>
            {
                bool exists = changeEvent.changedProperty.objectReferenceValue != null;
                this.m_Body.SetEnabled(exists);
            });
            
            return this.m_Root;
        }
        
        // CREATION MENU: -------------------------------------------------------------------------
        
        [MenuItem("GameObject/Game Creator/UI/Stats/Formula UI", false, 0)]
        public static void CreateElement(MenuCommand menuCommand)
        {
            GameObject canvas = UnityUIUtilities.GetCanvas();
            
            DefaultControls.Resources resources = UnityUIUtilities.GetStandardResources();
            GameObject gameObject = DefaultControls.CreateText(resources);
            gameObject.transform.SetParent(canvas.transform, false);
            gameObject.name = "Formula UI";

            Text text = gameObject.GetComponent<Text>();
            text.text = "99";
            
            FormulaUI.CreateFrom(text);

            Undo.RegisterCreatedObjectUndo(gameObject, $"Create {gameObject.name}");
            Selection.activeGameObject = gameObject;
        }
    }
}