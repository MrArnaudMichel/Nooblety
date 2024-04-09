using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue.UnityUI
{
    [Icon(EditorPaths.PACKAGES + "Dialogue/Editor/Gizmos/GizmoDialogueUI.png")]
    [AddComponentMenu("Game Creator/UI/Dialogue/Unit Logs UI")]
    
    public class DialogueUnitLogsUI : TDialogueUnitUI
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private RectTransform m_Content;
        [SerializeField] private GameObject m_Prefab;

        [SerializeField] private bool m_KeepAmount = false;
        [SerializeField] private int m_MaxAmount = 50;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Story m_Story;
        [NonSerialized] private Args m_Args;
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnReset(bool newSequence)
        {
            if (newSequence) this.Clear();
        }
        
        public override void OnStartNext(Story story, int nodeId, Args args)
        {
            if (ApplicationManager.IsExiting) return;
            
            this.m_Story = story;
            this.m_Args = args;
            
            Node node = story.Content.Get(nodeId);
            if (node == null) return;
            
            node.EventFinishType -= this.OnFinishText;
            node.EventFinishType += this.OnFinishText;
        }

        public override void OnFinishNext(Story story, int nodeId, Args args)
        {
            if (ApplicationManager.IsExiting) return;
            
            Node node = story.Content.Get(nodeId);
            if (node == null) return;
            
            node.EventFinishType -= this.OnFinishText;
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnFinishText(int nodeId)
        {
            if (ApplicationManager.IsExiting) return;
            
            Node node = this.m_Story?.Content.Get(nodeId);
            if (node == null) return;

            GameObject prefab = this.m_DialogueUI.SpeechSkin.OverrideLog != null 
                ? this.m_DialogueUI.SpeechSkin.OverrideLog
                : this.m_Prefab;

            if (this.m_Content == null) return;
            if (prefab == null) return;
            
            GameObject instance = UIUtils.Instantiate(prefab, this.m_Content);
            
            DialogueLogUI logUI = instance.Get<DialogueLogUI>();
            if (logUI != null) logUI.Setup(node, this.m_Args);
            
            if (!this.m_KeepAmount || this.m_Content.childCount <= this.m_MaxAmount) return;

            Transform child = this.m_Content.GetChild(0);
            Destroy(child.gameObject);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public void Clear()
        {
            if (this.m_Content == null) return;
            for (int i = this.m_Content.childCount - 1; i >= 0; --i)
            {
                Transform child = this.m_Content.GetChild(i);
                Destroy(child.gameObject);
            }
        }
    }
}