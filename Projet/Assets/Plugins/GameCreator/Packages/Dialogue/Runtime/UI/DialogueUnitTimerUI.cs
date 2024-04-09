using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.UnityUI;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Dialogue.UnityUI
{
    [Icon(EditorPaths.PACKAGES + "Dialogue/Editor/Gizmos/GizmoDialogueUI.png")]
    [AddComponentMenu("Game Creator/UI/Dialogue/Unit Timer UI")]
    
    public class DialogueUnitTimerUI : TDialogueUnitUI
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private GameObject m_Active;
        
        [SerializeField] private Image m_TimerBar;
        [SerializeField] private bool m_InvertProgress = true;
        
        [SerializeField] private TextReference m_RemainingSeconds = new TextReference();
        [SerializeField] private TextReference m_RemainingDecimals = new TextReference();
        
        [SerializeField] private TextReference m_DurationSeconds = new TextReference();
        [SerializeField] private TextReference m_DurationDecimals = new TextReference();

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_IsActive;
        
        [NonSerialized] private Story m_Story;
        [NonSerialized] private int m_NodeId;
        [NonSerialized] private Args m_Args;
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnAwake(DialogueUI dialogueUI)
        {
            base.OnAwake(dialogueUI);
            if (this.m_Active != null) this.m_Active.SetActive(false);
        }

        public override void OnReset(bool isNew)
        { }
        
        public override void OnStartNext(Story story, int nodeId, Args args)
        {
            if (ApplicationManager.IsExiting) return;
            
            this.m_Story = story;
            this.m_NodeId = nodeId;
            this.m_Args = args;
            
            Node node = story.Content.Get(this.m_NodeId);
            if (node == null) return;

            this.m_IsActive = false;
            if (this.m_Active != null) this.m_Active.SetActive(false);

            node.EventStartChoice -= this.OnStartChoice;
            node.EventFinishChoice -= this.OnFinishChoice;
            
            node.EventStartChoice += this.OnStartChoice;
            node.EventFinishChoice += this.OnFinishChoice;
        }
        
        public override void OnFinishNext(Story story, int nodeId, Args args)
        {
            if (ApplicationManager.IsExiting) return;
            
            this.m_IsActive = false;
            
            Node node = story.Content.Get(nodeId);
            if (node == null) return;
            
            if (this.m_Active != null) this.m_Active.SetActive(false);
            
            node.EventStartChoice -= this.OnStartChoice;
            node.EventFinishChoice -= this.OnFinishChoice;
        }
        
        // UPDATE: --------------------------------------------------------------------------------

        private void Update()
        {
            if (!this.m_IsActive) return;
            
            Node node = this.m_Story?.Content.Get(this.m_NodeId);
            
            if (node?.NodeType is not NodeTypeChoice nodeTypeChoice) return;
            if (!nodeTypeChoice.GetTimedChoice(this.m_Story.Content.DialogueSkin)) return;

            this.Refresh(nodeTypeChoice);
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnStartChoice(int nodeId)
        {
            if (ApplicationManager.IsExiting) return;
            Node node = this.m_Story?.Content.Get(nodeId);
            
            if (node?.NodeType is not NodeTypeChoice nodeTypeChoice) return;
            if (!nodeTypeChoice.GetTimedChoice(this.m_Story.Content.DialogueSkin)) return;

            this.m_IsActive = true;
            if (this.m_Active != null) this.m_Active.SetActive(true);

            this.Refresh(nodeTypeChoice);
        }

        private void OnFinishChoice(int nodeId)
        {
            if (ApplicationManager.IsExiting) return;
            
            this.m_IsActive = false;
            if (this.m_Active != null) this.m_Active.SetActive(false);
        }
        
        private void Refresh(NodeTypeChoice nodeChoice)
        {
            if (ApplicationManager.IsExiting) return;
            
            float duration = nodeChoice.CurrentDuration;
            float elapsed = nodeChoice.CurrentElapsed;
            float remaining = duration - elapsed;

            float t = Mathf.Clamp01(elapsed / duration);
            if (this.m_InvertProgress) t = 1f - t;

            if (this.m_TimerBar != null) this.m_TimerBar.fillAmount = t;
            
            this.m_RemainingSeconds.Text = GetSeconds(remaining);
            this.m_RemainingDecimals.Text = GetDecimals(remaining);
            
            this.m_DurationSeconds.Text = GetSeconds(duration);
            this.m_DurationDecimals.Text = GetDecimals(duration);
        }
        
        private static string GetSeconds(float time)
        {
            return Mathf.FloorToInt(time).ToString("0");
        }

        private static string GetDecimals(float time)
        {
            float decimals = time - Mathf.Floor(time);
            return (decimals * 100f).ToString("0");
        }
    }
}