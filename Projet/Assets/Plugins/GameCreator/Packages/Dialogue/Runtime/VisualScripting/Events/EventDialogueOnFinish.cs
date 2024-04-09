using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Title("On Finish Dialogue")]
    [Category("Dialogue/On Finish Dialogue")]
    [Description("Executed when a specific Dialogue component finishes playing")]

    [Image(typeof(IconNodeText), ColorTheme.Type.Blue, typeof(OverlayCross))]
    
    [Keywords("Node", "Conversation", "Speech", "Text")]
    [Keywords("End", "Complete")]

    [Serializable]
    public class EventDialogueOnFinish : VisualScripting.Event
    {
        [SerializeField] private PropertyGetGameObject m_Dialogue = GetGameObjectDialogue.Create();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        private Dialogue Dialogue { get; set; }
        private Args Args { get; set; }
        
        // INITIALIZERS: --------------------------------------------------------------------------
        
        protected override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            this.Dialogue = this.m_Dialogue.Get<Dialogue>(trigger);
            if (this.Dialogue == null) return;

            this.Args = new Args(this.Self, this.Dialogue.gameObject);
            
            this.Dialogue.EventFinish -= this.OnDialogueFinish;
            this.Dialogue.EventFinish += this.OnDialogueFinish;
        }

        protected override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            
            if (this.Dialogue == null) return;
            this.Dialogue.EventFinish -= this.OnDialogueFinish;
        }

        private void OnDialogueFinish()
        {
            _ = this.m_Trigger.Execute(this.Args);
        }
    }
}