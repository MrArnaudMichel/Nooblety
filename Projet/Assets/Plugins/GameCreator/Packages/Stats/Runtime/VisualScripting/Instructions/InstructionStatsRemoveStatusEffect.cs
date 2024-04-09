using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Stats
{
    [Version(0, 1, 1)]
    
    [Title("Remove Status Effect")]
    [Category("Stats/Remove Status Effect")]
    
    [Image(typeof(IconStatusEffect), ColorTheme.Type.Green, typeof(OverlayMinus))]
    [Description("Removes a Status Effect from the selected game object's Traits component")]

    [Parameter("Target", "The targeted game object with a Traits component")]
    [Parameter("Amount", "Indicates how many Status Effects are removed at most")]
    [Parameter("Status Effect", "The type of Status Effect that is removed")]
    
    [Keywords("Buff", "Debuff", "Enhance", "Ailment")]
    [Keywords(
        "Blind", "Dark", "Burn", "Confuse", "Dizzy", "Stagger", "Fear", "Freeze", "Paralyze", 
        "Shock", "Silence", "Sleep", "Silence", "Slow", "Toad", "Weak", "Strong", "Poison"
    )]
    [Keywords(
        "Haste", "Protect", "Reflect", "Regenerate", "Shell", "Armor", "Shield", "Berserk",
        "Focus", "Raise"
    )]

    [Serializable]
    public class InstructionStatsRemoveStatusEffect : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();

        [SerializeField] private int m_Amount = 1;
        [SerializeField] private StatusEffect m_StatusEffect;

        public override string Title => string.Format(
            "Remove {0} {1} from {2}",
            this.m_Amount,
            this.m_StatusEffect != null
                ? this.m_StatusEffect.ID.String 
                : "(none)",
            this.m_Target
        );
        
        protected override Task Run(Args args)
        {
            GameObject target = this.m_Target.Get(args);
            if (target == null) return DefaultResult;

            Traits traits = target.Get<Traits>();
            if (traits == null) return DefaultResult;
            
            if (this.m_StatusEffect == null) return DefaultResult;
            
            traits.RuntimeStatusEffects.Remove(this.m_StatusEffect, this.m_Amount);
            return DefaultResult;
        }
    }
}