using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Serializable]
    public class Story
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeReference] private Content m_Content;
        [SerializeField] private Visits m_Visits;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_IsCanceled;
        [NonSerialized] private int m_CurrentId;
        
        [NonSerialized] private Actor m_LastActor;
        [NonSerialized] private int m_LastExpression;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Content Content => this.m_Content;

        public Visits Visits => this.m_Visits;

        public TimeMode Time => this.m_Content.Time;

        public bool IsCanceled
        {
            get => AsyncManager.ExitRequest || this.m_IsCanceled;
            set => this.m_IsCanceled = value;
        }
        
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action<int> EventStartNext;
        public event Action<int> EventFinishNext;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public Story()
        {
            this.m_Content = new Content();
            this.m_Visits = new Visits();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public async Task Play(Args args)
        {
            int[] rootIds = this.m_Content.RootIds;
            if (rootIds.Length == 0) return;

            Stack<int> stack = new Stack<int>();
            for (int i = rootIds.Length - 1; i >= 0; --i)
            {
                stack.Push(rootIds[i]);
            }

            this.m_IsCanceled = false;
            this.m_LastActor = null;
            this.m_LastExpression = -1;
            
            while (stack.Count > 0 && !this.IsCanceled)
            {
                this.m_CurrentId = stack.Pop();
                Node node = this.m_Content.Get(this.m_CurrentId);

                if (node == null) continue;
                if (!node.CanRun(args)) continue;

                if (node.Actor != this.m_LastActor || node.Expression != this.m_LastExpression)
                {
                    if (this.m_LastActor != null)
                    {
                        Expression value = this.m_LastActor
                            .GetExpressionFromIndex(this.m_LastExpression);
                        if (value != null) await value.OnEnd(args);
                    }

                    if (node.Actor != null)
                    {
                        Expression value = node.Actor.GetExpressionFromIndex(node.Expression);
                        if (value != null) await value.OnStart(args);
                    }
                }

                this.m_LastActor = node.Actor;
                this.m_LastExpression = node.Expression;

                this.EventStartNext?.Invoke(this.m_CurrentId);
                NodeJump result = await node.Run(this.m_CurrentId, this, args);
                this.EventFinishNext?.Invoke(this.m_CurrentId);

                switch (result.Jump)
                {
                    case JumpType.Continue:
                        List<int> children = node.GetNext(this.m_CurrentId, this, args);
                        for (int i = children.Count - 1; i >= 0; --i)
                        {
                            stack.Push(children[i]);
                        }
                        break;
                    
                    case JumpType.Exit:
                        stack.Clear();
                        await this.Finish(args);
                        return;

                    case JumpType.Jump:
                        int nextId = this.m_Content.FindByTag(result.JumpTo);
                        stack.Clear(); 
                        this.StackFromJump(stack, nextId);
                        break;
                    
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            
            await this.Finish(args);
        }

        public void Continue()
        {
            this.m_Content.Get(this.m_CurrentId)?.Continue();
        }
        
        // INTERNAL METHODS: ----------------------------------------------------------------------

        internal void StopTypewriter()
        {
            this.m_Content.Get(this.m_CurrentId)?.StopTypewriter();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private async Task Finish(Args args)
        {
            if (this.m_LastActor != null)
            {
                Actor actor = this.m_LastActor;
                int index = this.m_LastExpression;
                
                Expression expression = actor.GetExpressionFromIndex(index);
                if (expression != null) await expression.OnEnd(args);
            }

            this.m_Visits.IsVisited = true;
        }
        
        private void StackFromJump(Stack<int> stack, int nodeId)
        {
            int parentId = this.Content.Parent(nodeId);

            if (parentId != Content.NODE_INVALID)
            {
                this.StackFromJump(stack, parentId);
                Node parent = this.Content.Get(parentId);

                if (parent.NodeType.IsBranch)
                {
                    stack.Push(nodeId);
                    return;
                }
            }

            List<int> siblings = this.m_Content.Siblings(nodeId);
                
            for (int i = siblings.Count - 1; i >= 0; --i)
            {
                int siblingId = siblings[i];
                    
                stack.Push(siblingId);
                if (siblingId == nodeId) break;
            }
        }
    }
}