using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCreator.Runtime.Dialogue.UnityUI
{
    [Icon(EditorPaths.PACKAGES + "Dialogue/Editor/Gizmos/GizmoDialogueUI.png")]
    [AddComponentMenu("Game Creator/UI/Dialogue/Choice UI")]
    
    public class DialogueChoiceUI : MonoBehaviour
    {
        #if UNITY_EDITOR

        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            EventChoice = null;
        }
        
        #endif
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private TextReference m_Text = new TextReference();
        [SerializeField] private TextReference m_Index = new TextReference();
        [SerializeField] private Button m_Button;
        
        [SerializeField] private RectTransform m_Actor;
        [SerializeField] private TextReference m_ActorName = new TextReference();
        [SerializeField] private TextReference m_ActorDescription = new TextReference();

        [SerializeField] private GameObject m_ActiveSelected;
        [SerializeField] private GameObject m_ActiveCondition;
        
        [SerializeField] private Graphic m_Graphic;
        [SerializeField] private Color m_GraphicNormal = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color m_GraphicSelected = new Color(1f, 1f, 1f, 1f);
        
        [SerializeField] private Selectable m_Color;
        [SerializeField] private Color m_ColorNormal = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color m_ColorVisited = new Color(1f, 1f, 1f, 0.5f);

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private DialogueUnitChoicesUI m_ChoicesUI;

        [NonSerialized] private int m_NodeIndex;
        [NonSerialized] private int m_NodeId;
        [NonSerialized] private Story m_Story;
        [NonSerialized] private Args m_Args;

        [NonSerialized] private bool m_WasSelected;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public Button Button => this.m_Button;
        
        // EVENTS: --------------------------------------------------------------------------------

        public static event Action<int> EventChoice;

        // UPDATE METHODS: ------------------------------------------------------------------------

        private void Update()
        {
            if (this.m_Button == null) return;

            if (EventSystem.current == null)
            {
                Debug.LogError("No 'EventSystem' found. Please create one");
                return;
            }
 
            GameObject selection = EventSystem.current.currentSelectedGameObject;
            bool isSelected = this.m_Button.interactable && selection == this.m_Button.gameObject;

            if (isSelected && !this.m_WasSelected)
            {
                this.m_ChoicesUI.Select();
            }
            
            this.m_WasSelected = isSelected;

            if (this.m_ActiveSelected != null)
            {
                this.m_ActiveSelected.SetActive(isSelected);
            }
            
            if (this.m_Graphic != null)
            {
                this.m_Graphic.color = isSelected
                    ? this.m_GraphicSelected
                    : this.m_GraphicNormal;
            }
        }

        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        public void Setup(int index, int nodeId, Node node, Story story, Args args, DialogueUnitChoicesUI choices)
        {
            this.m_NodeIndex = index + 1;
            this.m_NodeId = nodeId;
            this.m_Story = story;
            this.m_Args = args;

            this.m_WasSelected = false;
            this.m_ChoicesUI = choices;

            if (this.m_Actor != null)
            {
                this.m_Actor.gameObject.SetActive(node.Actor != null);
            
                if (node.Actor != null)
                {
                    this.m_ActorName.Text = node.Actor.GetName(this.m_Args);
                    this.m_ActorDescription.Text = node.Actor.GetDescription(this.m_Args);   
                }
            }

            if (this.m_Button != null)
            {
                this.m_Button.interactable = node.CanRun(args);
                this.m_Button.onClick.AddListener(this.OnClick);
            }

            if (this.m_ActiveCondition != null)
            {
                bool canRun = node.CanRun(args);
                this.m_ActiveCondition.SetActive(!canRun);
            }

            if (this.m_Color != null)
            {
                ColorBlock block = this.m_Color.colors;
                block.normalColor = this.m_Story.Visits.Nodes.Contains(this.m_NodeId)
                    ? this.m_ColorVisited
                    : this.m_ColorNormal;

                this.m_Color.colors = block;
            }
            
            this.m_Text.Text = node.GetText(args);
            this.m_Index.Text = this.m_NodeIndex.ToString();
            
            this.Update();
            
            EventChoice -= this.OnSelect;
            EventChoice += this.OnSelect;
        }

        private void OnDisable()
        {
            EventChoice -= this.OnSelect;
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnClick()
        {
            if (this.m_ChoicesUI == null) return;
            this.m_ChoicesUI.Choose(this.m_NodeId);
        }

        private void OnSelect(int index)
        {
            if (this.m_ChoicesUI == null) return;
            if (this.m_NodeIndex != index) return;
            
            this.m_ChoicesUI.Choose(this.m_NodeId);
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static void SelectIndex(int index)
        {
            EventChoice?.Invoke(index);
        }
    }
}