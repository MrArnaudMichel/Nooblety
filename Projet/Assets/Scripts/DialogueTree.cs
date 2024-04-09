using GameCreator.Runtime.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace GameCreator.Runtime
{
    /// <summary>
    /// Data structure for a node in a dialogue tree
    /// </summary>
    public class DialogueNode
    {
        public int id = 0;
        public string text;
        public string speaker;
        public VisualScripting.Instruction [] instruction = Array.Empty<VisualScripting.Instruction>();
        public List<DialogueNode> children { get; }

        /// <summary>
        /// Constructor for a node with children
        /// </summary>
        /// <param name="text"></param>
        /// <param name="skeaker"></param>
        public DialogueNode(string text, string skeaker = null)
        {
            this.children = new List<DialogueNode>();
            this.speaker = skeaker;
            this.text = text;
        }
        /// <summary>
        /// Constructor for a node without children
        /// </summary>
        /// <param name="text"></param>
        /// <param name="instruction"></param>
        public DialogueNode(string text,params VisualScripting.Instruction[] instruction)
        {
            this.children = new List<DialogueNode>();
            this.text = text;
            this.instruction = instruction;
        }

        
        /// <summary>
        /// Add a child to the node
        /// </summary>
        /// <param name="node"></param>
        public void addChild(DialogueNode node)
        {
            children.Add(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TNodeType getType()
        {
            foreach (DialogueNode node in children)
            {
                if (node.speaker != null || speaker != null)
                {
                    return new NodeTypeChoice();
                }
            }
            return new NodeTypeText();
        }
    }


    /// <summary>
    /// Data structure for a dialogue tree
    /// </summary>
    public class DialogueTree
    {
        /// <summary>
        /// Root node of the tree
        /// </summary>
        public DialogueNode root { get; }
        int size;

        /// <summary>
        /// Constructor for a tree
        /// </summary>
        /// <param name="root"></param>
        public DialogueTree(DialogueNode root)
        {
            this.root = root;
            size = 1;
        }

        /// <summary>
        /// Constructor for a tree
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public DialogueNode find(int id, DialogueNode node = null)
        {
            if (node == null)
            {
                node = root;
            }

            if (node.id == id)
            {
                return node;
            }

            foreach (DialogueNode n in node.children)
            {
                DialogueNode result = find(id, n);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a child to a node
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <returns>int</returns>
        public int addchild(int id, DialogueNode node)
        {
            DialogueNode parent;
            if ((parent = find(id, root)) != null)
            {
                parent.addChild(node);

                node.id = size;
                size++;
                return node.id;
            }

            Debug.LogWarning("pas de noeud avec cette id");
            return -1;


        }

        /// <summary>
        /// Add children to a node
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nodes"></param>
        /// <returns>List[int]</returns>
        public List<int> addchild(int id, params DialogueNode[] nodes)
        {
            List<int> listId = new List<int>();
            DialogueNode parent;
            if ((parent = find(id, root)) != null)
            {
                foreach (DialogueNode node in nodes)
                {
                    parent.addChild(node);
                    node.id = size;
                    listId.Add(node.id);
                    size++;
                }
                return listId;
            }

            Debug.LogWarning("pas de noeud avec cette id");
            return null;


        }
    }
}
