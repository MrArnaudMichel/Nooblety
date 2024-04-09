using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Stats
{
    [Title("Traits")]
    [Category("Stats/Traits")]
    
    [Image(typeof(IconTraits), ColorTheme.Type.Pink)]
    [Description("Remembers every Stat, Attribute and Status Effect of the game object")]

    [Serializable]
    public class MemoryTraits : Memory
    {
        public override string Title => "Traits";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Token GetToken(GameObject target)
        {
            Traits traits = target.Get<Traits>();
            return traits != null ? new TokenTraits(traits) : null;
        }

        public override void OnRemember(GameObject target, Token token)
        {
            Traits traits = target.Get<Traits>();
            if (traits == null) return;
            
            TokenTraits.OnRemember(traits, token);
        }
    }
}