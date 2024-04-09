using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Title("Dialogue")]
    [Category("Dialogue/Dialogue")]
    
    [Image(typeof(IconNodeText), ColorTheme.Type.Green)]
    [Description("Remembers the visited nodes of a Story")]

    [Serializable]
    public class MemoryDialogue : Memory
    {
        public override string Title => "Dialogue";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Token GetToken(GameObject target)
        {
            Dialogue dialogue = target.Get<Dialogue>();
            return dialogue != null ? new TokenDialogue(dialogue) : null;
        }

        public override void OnRemember(GameObject target, Token token)
        {
            Dialogue dialogue = target.Get<Dialogue>();
            if (dialogue == null) return;
            
            TokenDialogue.OnRemember(dialogue, token);
        }
    }
}