using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Dialogue
{
    public class ContentToolTree : VisualElement
    {
        public const int DEFAULT_HEIGHT = 30;
        public const int DEFAULT_MAX_ROWS = 20;
        
        private const string KEY_MAX_ROWS = "gc:dialogue:max-rows";

        private const string TITLE_DELETE = "Are you sure you want to delete this element?";
        private const string MSG_DELETE = "This action cannot be undone";

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly TreeView m_TreeView;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public ContentTool ContentTool { get; }
        
        public int Rows
        {
            get => EditorPrefs.GetInt(KEY_MAX_ROWS, DEFAULT_MAX_ROWS);
            set
            {
                int max = Math.Max(value, 10);
                EditorPrefs.SetInt(KEY_MAX_ROWS, max);
                this.RefreshMaxHeight();
            }
        }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<int> EventSelection;
        public event Action EventChange;
        public event Action EventChangeTag;

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ContentToolTree(ContentTool contentTool)
        {
            this.ContentTool = contentTool;

            Content content = this.ContentTool.Content;
            int[] roots = content.RootIds;
            
            List<TreeViewItemData<Node>> entries = new List<TreeViewItemData<Node>>();
            foreach (int rootId in roots) entries.AddRange(this.GetTree(content, rootId));

            this.m_TreeView = new TreeView
            {
                fixedItemHeight = DEFAULT_HEIGHT,
                horizontalScrollingEnabled = true,
                makeItem = this.MakeItem,
                bindItem = this.OnBindItem,
                unbindItem = this.OnUnbindItem,
            };
            
            this.m_TreeView.SetRootItems(entries);
            
            this.m_TreeView.reorderable = true;
            this.m_TreeView.selectionType = SelectionType.Single;
            this.m_TreeView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;

            this.m_TreeView.itemIndexChanged += this.ReorderItems;
            
            // TODO: [21/03/2023] Remove once Unity 2022.3 LTS is released
            
            #if UNITY_2022_2_OR_NEWER
            this.m_TreeView.selectionChanged += this.SelectionChange;
            #else
            this.m_TreeView.onSelectionChange += this.SelectionChange;
            #endif
            
            this.m_TreeView.RegisterCallback<KeyDownEvent>(keyEvent =>
            {
                if (this.m_TreeView.selectedIndex < 0) return;
                if (keyEvent.keyCode != KeyCode.Delete && keyEvent.keyCode != KeyCode.Backspace)
                {
                    return;
                }
                
                this.RemoveSelection();
            });
            
            this.Add(this.m_TreeView);
            this.RefreshMaxHeight();
        }
        
        public void Setup()
        { }

        private void SelectionChange(IEnumerable<object> selection)
        {
            int id = this.m_TreeView.selectedIndex >= 0
                ? this.m_TreeView.GetIdForIndex(this.m_TreeView.selectedIndex)
                : Content.NODE_INVALID;
            
            this.EventSelection?.Invoke(id);
        }

        // CALLBACKS: -----------------------------------------------------------------------------
        
        private VisualElement MakeItem() => new ContentToolTreeNode(this.ContentTool);

        private void OnBindItem(VisualElement element, int index)
        {
            int id = this.m_TreeView.GetIdForIndex(index);
            SerializedProperty propertyData = this.ContentTool
                .FindPropertyForId(id)
                .FindPropertyRelative(TTreeDataItem<Node>.NAME_VALUE);

            if (element is ContentToolTreeNode nodeTool)
            {
                nodeTool.BindItem(propertyData, id);
                
                nodeTool.EventChangeTag -= this.OnChangeTag;
                nodeTool.EventChangeTag += this.OnChangeTag;
            }
        }

        private void OnUnbindItem(VisualElement element, int index)
        {
            if (element is ContentToolTreeNode nodeTool)
            {
                nodeTool.UnbindItem();
                nodeTool.EventChangeTag -= this.OnChangeTag;
            }
        }
        
        private void ReorderItems(int indexSource, int indexTarget)
        {
            this.m_TreeView.viewController.RebuildTree();
            this.m_TreeView.RefreshItems();
            
            // int idSource = this.m_TreeView.GetIdForIndex(indexSource);
            // int idTarget = this.m_TreeView.GetIdForIndex(indexSource);
            //
            // this.m_TreeView.ExpandItem(idSource, true);
            // this.m_TreeView.ExpandItem(idTarget, true);
            this.SynchronizeTree();

            this.EventChange?.Invoke();
        }
        
        private void OnChangeTag()
        {
            this.EventChangeTag?.Invoke();
        }
        
        // SETTER METHODS: ------------------------------------------------------------------------

        public void CreateAsSelectionChild(object value)
        {
            if (value is not Node valueNode) return;
            
            if (this.m_TreeView.selectedIndex == -1)
            {
                this.CreateAsSelectionSibling(value);
                return;
            }

            int parentId = this.m_TreeView.GetIdForIndex(this.m_TreeView.selectedIndex);
            
            Content content = this.ContentTool.Content;
            int newId = content.AddChild(valueNode, parentId);
            
            this.ContentTool.Content = content;

            TreeViewItemData<Node> itemData = new TreeViewItemData<Node>(newId, valueNode);
            this.m_TreeView.AddItem(itemData, parentId);
            this.m_TreeView.SetSelectionById(newId);
            
            this.EventChange?.Invoke();
        }
        
        public void CreateAsSelectionSibling(object value)
        {
            if (value is not Node valueNode) return;

            Content content = this.ContentTool.Content;
            int[] rootIds = this.m_TreeView.GetRootIds()?.ToArray() ?? Array.Empty<int>();

            int selectedId = this.m_TreeView.selectedIndex switch
            {
                -1 => rootIds.Length > 0 ? rootIds[^1] : Content.NODE_INVALID,
                _ => this.m_TreeView.GetIdForIndex(this.m_TreeView.selectedIndex)
            };
            
            int newId = selectedId != Content.NODE_INVALID
                ? content.AddAfterSibling(valueNode, selectedId)
                : content.AddToRoot(valueNode);

            this.ContentTool.Content = content;

            int parentId = this.m_TreeView.GetParentIdForIndex(this.m_TreeView.selectedIndex);
            int selectedIndex = this.m_TreeView.viewController.GetChildIndexForId(selectedId);

            TreeViewItemData<Node> itemData = new TreeViewItemData<Node>(newId, valueNode);
            this.m_TreeView.AddItem(itemData, parentId, selectedIndex + 1);
            
            this.m_TreeView.SetSelectionById(newId);
            
            this.EventChange?.Invoke();
        }

        public void RemoveSelection(bool confirmationDialog = true)
        {
            if (confirmationDialog)
            {
                bool delete = EditorUtility.DisplayDialog(
                    TITLE_DELETE, MSG_DELETE, 
                    "Yes", "Cancel"
                );
                
                if (!delete) return;
            }

            if (this.m_TreeView.selectedIndex == -1) return;
            int selectedIndex = this.m_TreeView.selectedIndex;

            int selectedId = this.m_TreeView.GetIdForIndex(selectedIndex);
            this.m_TreeView.ExpandItem(selectedId, true);
            
            Content content = this.ContentTool.Content;
            
            bool success = content.Remove(selectedId);
            this.ContentTool.Content = content;
            
            if (!success) return;
            
            this.m_TreeView.TryRemoveItem(selectedId);
            
            this.EventChange?.Invoke();
            this.m_TreeView.SetSelection(Content.NODE_INVALID);
        }

        public bool Select(int id)
        {
            int index = this.m_TreeView.viewController.GetIndexForId(id);
            if (index == -1) return false;
            
            this.m_TreeView.SetSelection(index);
            return true;
        }

        // SYNCHRONIZE METHODS: -------------------------------------------------------------------

        private void SynchronizeTree()
        {
            this.m_TreeView.ExpandAll();
            
            Content content = this.ContentTool.Content;

            this.SynchronizeRoots(content);
            this.SynchronizeNodes(content);
            
            this.ContentTool.Content = content;
        }
        
        private void SynchronizeRoots(Content content)
        {
            content.RootIds = this.m_TreeView.GetRootIds()?.ToArray() ?? Array.Empty<int>();
        }

        private void SynchronizeNodes(Content content)
        {
            int[] rootIds = this.m_TreeView.GetRootIds()?.ToArray() ?? Array.Empty<int>();

            foreach (int rootId in rootIds)
            {
                SynchronizeNode(content, rootId, Content.NODE_INVALID);
            }
        }

        private void SynchronizeNode(Content content, int node, int ancestor)
        {
            int index = this.m_TreeView.viewController.GetIndexForId(node);
            int[] childrenIds = this.m_TreeView.GetChildrenIdsForIndex(index)?.ToArray() ?? Array.Empty<int>();

            TreeNode value = content.Nodes[node]; 
            value.Children = new List<int>(childrenIds);
            value.Parent = ancestor;
            
            foreach (int childId in childrenIds)
            {
                SynchronizeNode(content, childId, node);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private IEnumerable<TreeViewItemData<Node>> GetTree(Content content, int nodeId)
        {
            Node node = content.Get(nodeId);
            if (node == null) return new List<TreeViewItemData<Node>>();
        
            List<int> children = content.Children(nodeId);
            List<TreeViewItemData<Node>> subTree = new List<TreeViewItemData<Node>>();
        
            foreach (int childId in children)
            {
                IEnumerable<TreeViewItemData<Node>> childEntries = this.GetTree(content, childId);
                subTree.AddRange(childEntries);
            }
        
            return new List<TreeViewItemData<Node>>
            {
                new TreeViewItemData<Node>(nodeId, node, subTree)
            };
        }
        
        private void RefreshMaxHeight()
        {
            if (this.m_TreeView == null) return;

            float height = this.m_TreeView.fixedItemHeight;
            Length length = new Length(height * this.Rows, LengthUnit.Pixel);
            
            this.m_TreeView.style.maxHeight = new StyleLength(length);
        }
        
        // DEBUG METHODS: -------------------------------------------------------------------------
        
        private void DebugPrintTree()
        {
            int[] rootsIds =  this.m_TreeView.GetRootIds().ToArray();
            StringBuilder stringBuilder = new StringBuilder();
            
            this.ToStringTree(stringBuilder, rootsIds, 0);
            Debug.Log(stringBuilder.ToString());
        }
        
        private void ToStringTree(StringBuilder sb, IEnumerable<int> children, int depth)
        {
            if (children == null) return;
            
            foreach (int child in children)
            {
                string text = child.ToString();
                int padding = depth * 4;
                sb.AppendLine(text.PadLeft(text.Length + padding));

                int i = this.m_TreeView.viewController.GetIndexForId(child);
                ToStringTree(sb, this.m_TreeView.GetChildrenIdsForIndex(i)?.ToArray(), depth + 1);
            }
        }
    }
}