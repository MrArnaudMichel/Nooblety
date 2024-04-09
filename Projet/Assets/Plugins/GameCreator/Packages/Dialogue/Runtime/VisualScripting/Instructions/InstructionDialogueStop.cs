using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Version(0, 1, 1)]
    
    [Title("Stop Dialogue")]
    [Category("Dialogue/Stop Dialogue")]
    
    [Image(typeof(IconNodeText), ColorTheme.Type.Red, typeof(OverlayCross))]
    [Description("Stop playing a dialogue")]

    [Parameter("Dialogue", "The Dialogue component to stop playing")]

    [Keywords("Dialogue", "Narration", "Speech", "Next", "Skip")]

    [Serializable]
    public class InstructionDialogueStop : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Dialogue = GetGameObjectDialogue.Create();

        public override string Title => $"Stop {this.m_Dialogue}";
        
        protected override Task Run(Args args)
        {
            Dialogue dialogue = this.m_Dialogue.Get<Dialogue>(args);
            if (dialogue != null) dialogue.Stop();

            return DefaultResult;
        }
    }
}