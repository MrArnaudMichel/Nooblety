using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Stats
{
    [CreateAssetMenu(
        fileName = "Status Effect", 
        menuName = "Game Creator/Stats/Status Effect",
        order    = 50
    )]
    
    [Icon(EditorPaths.PACKAGES + "Stats/Editor/Gizmos/GizmoStatusEffects.png")]
    
    public class StatusEffect : ScriptableObject
    {
        #if UNITY_EDITOR

        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            LastAdded = null;
            LastRemoved = null;
        }
        
        #endif
        
        public static StatusEffect LastAdded { get; internal set; }
        public static StatusEffect LastRemoved { get; internal set; }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private IdString m_ID = new IdString("status-effect-id");
        [SerializeField] private StatusEffectData m_Data = new StatusEffectData();
        [SerializeField] private StatusEffectInfo m_Info = new StatusEffectInfo();

        [SerializeField] private StatusEffectInstruction m_OnStart = new StatusEffectInstruction(
            new InstructionCommonDebugComment("Executed right after this Status Effect is applied")
        );
        
        [SerializeField] private StatusEffectInstruction m_OnEnd = new StatusEffectInstruction(
            new InstructionCommonDebugComment("Executed right after this Status Effect finishes")
        );
        
        [SerializeField] private StatusEffectInstruction m_WhileActive = new StatusEffectInstruction(
            new InstructionCommonDebugComment("Executed over and over again while this Status Effect lasts")
        );

        // PROPERTIES: ----------------------------------------------------------------------------

        public IdString ID => m_ID;
        public Color Color => this.m_Info.Color;

        public StatusEffectType Type => this.m_Data.Type;
        public bool Save => this.m_Data.Save;
        public bool HasDuration => this.m_Data.HasDuration;

        public StatusEffectInstruction OnStart => this.m_OnStart;
        public StatusEffectInstruction OnEnd => this.m_OnEnd;
        public StatusEffectInstruction OnWhileActive => this.m_WhileActive;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public string GetAcronym(Args args) => this.m_Info.m_Acronym.Get(args);
        public string GetName(Args args) => this.m_Info.m_Name.Get(args);
        public string GetDescription(Args args) => this.m_Info.m_Description.Get(args);
        public Sprite GetIcon(Args args) => this.m_Info.GetIcon(args);

        public float GetDuration(Args args) => (float) this.m_Data.GetDuration(args);
        public int GetMaxStack(Args args) => this.m_Data.GetMaxStack(args);
    }
}