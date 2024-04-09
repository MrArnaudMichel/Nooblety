using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Dialogue
{
    public class ContentTool : VisualElement
    {
        private const string USS_PATH = EditorPaths.PACKAGES + "Dialogue/Editor/StyleSheets/Content";

        private const string NAME_CONTENT = "GC-Dialogue-Content";
        private const string NAME_PANEL_SETTINGS = "GC-Dialogue-Content-Panel-Settings";
        private const string NAME_PANEL_CONTENT = "GC-Dialogue-Content-Panel-Content";

        private const string KEY_HEIGHT = "gc:dialogue:inspector-height";
        private const string KEY_LASTS_SELECTION = "gc:dialogue:inspector-selection:{0}";
        private const int DEFAULT_HEIGHT = 500;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly VisualElement m_Content;
        [NonSerialized] private readonly VisualElement m_PanelSettings;
        [NonSerialized] private readonly VisualElement m_PanelContent;
        
        [NonSerialized] private readonly TwoPaneSplitView m_SplitView;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] public SerializedProperty Property { get; }

        public Content Content
        {
            get
            {
                this.Property.serializedObject.Update();
                return this.Property.managedReferenceValue as Content;
            }
            set
            {
                this.SerializedObject.Update();
                this.Property.managedReferenceValue = value;

                int random = UnityEngine.Random.Range(1, 99999);
                this.Property.FindPropertyRelative("m_Dirty").intValue = random;
                this.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        
        public SerializedObject SerializedObject => this.Property.serializedObject;

        public int InspectorHeight
        {
            get => EditorPrefs.GetInt(KEY_HEIGHT, DEFAULT_HEIGHT);
            set
            {
                int height = Math.Clamp(value, 200, 600);
                EditorPrefs.SetInt(KEY_HEIGHT, height);
                this.style.height = height;
            }
        }
        
        private int LastSelection
        {
            get => SessionState.GetInt(this.SelectionKey, Content.NODE_INVALID);
            set => SessionState.SetInt(this.SelectionKey, value);
        }

        private string SelectionKey => string.Format(
            KEY_LASTS_SELECTION,
            this.Property.serializedObject.targetObject.GetInstanceID()
        );

        private SerializedProperty PropertyData => 
            this.Property.FindPropertyRelative(Content.NAME_DATA);
        
        private SerializedProperty PropertyDataKeys => 
            this.PropertyData.FindPropertyRelative(Content.NAME_DATA_KEYS);
        
        private SerializedProperty PropertyDataValues => 
            this.PropertyData.FindPropertyRelative(Content.NAME_DATA_VALUES);

        public ContentToolToolbar Toolbar { get; }
        public ContentToolSettings Settings { get; }
        public ContentToolTree Tree { get; }
        public ContentToolInspector Inspector { get; }

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ContentTool(SerializedProperty property)
        {
            this.Property = property;
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);

            this.style.height = this.InspectorHeight;

            this.m_Content = new VisualElement { name = NAME_CONTENT };
            this.m_PanelSettings = new VisualElement { name = NAME_PANEL_SETTINGS };
            this.m_PanelContent = new VisualElement { name = NAME_PANEL_CONTENT };

            this.Toolbar = new ContentToolToolbar(this);
            this.Settings = new ContentToolSettings(this);
            this.Tree = new ContentToolTree(this);
            this.Inspector = new ContentToolInspector(this);
            
            this.Toolbar.Setup();
            this.Settings.Setup();
            this.Tree.Setup();
            this.Inspector.Setup();
            
            int lastSelection = this.LastSelection;
            if (lastSelection != Content.NODE_INVALID)
            {
                this.Tree.Select(lastSelection);
            }

            this.m_SplitView = new TwoPaneSplitView(
                1, this.Inspector.Slider,
                TwoPaneSplitViewOrientation.Horizontal
            );

            this.Settings.EventState += this.RefreshSplitViews;
            this.Inspector.EventState += this.RefreshSplitViews;
            this.Tree.EventSelection += this.RefreshLastSelection;
            
            this.m_Content.Add(this.m_PanelSettings);
            this.m_Content.Add(this.m_PanelContent);
            
            this.m_PanelSettings.Add(this.Settings);
            this.m_PanelContent.Add(this.m_SplitView);
            
            this.m_SplitView.Add(this.Tree);
            this.m_SplitView.Add(this.Inspector);

            this.Add(this.Toolbar);
            this.Add(this.m_Content);

            this.RegisterCallback<GeometryChangedEvent>(_ => this.RefreshSplitViews());
            this.RefreshSplitViews();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public SerializedProperty FindPropertyForId(int id)
        {
            SerializedProperty keys = this.PropertyDataKeys;
            SerializedProperty values = this.PropertyDataValues;

            int size = keys.arraySize;
            for (int i = 0; i < size; ++i)
            {
                if (keys.GetArrayElementAtIndex(i).intValue != id) continue;
                return values.GetArrayElementAtIndex(i);
            }

            return null;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshSplitViews()
        {
            this.m_PanelSettings.style.display = this.Settings.State 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;

            switch (this.Inspector.State)
            {
                case true: this.m_SplitView.UnCollapse(); break;
                case false: this.m_SplitView.CollapseChild(1); break;
            }
            
            this.m_SplitView.fixedPaneInitialDimension = this.Inspector.Slider;
        }
        
        private void RefreshLastSelection(int nodeId)
        {
            this.LastSelection = nodeId;
        }
    }
}