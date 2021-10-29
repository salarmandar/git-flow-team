using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Environment
{
    public class TreeNode

    {
        public Guid? Guid { get; set; }
        public int Index { get; set; }

        public int Parent { get; set; }

        public TreeNode TreeNodes { get; set; }

        public string Name { get; set; }
        public bool Checked { get; set; }
        public bool expanded { get; set; }
        public bool enabled { get; set; }
        public int? NodeOrderingNo { get; set; }
        public string AppKey { get; set; }

        public List<TreeNode> ChildList { get; set; } =  new List<TreeNode>();
    }

    public class Tree
    {
        private TreeNode rootNode;
        public TreeNode RootNode
        {
            get { return rootNode; }
            set
            {
                if (RootNode != null)
                    Nodes.Remove(RootNode.Index);

                Nodes.Add(value.Index, value);
                rootNode = value;
            }
        }

        public Dictionary<int, TreeNode> Nodes { get; set; }

        public Tree()
        {
        }

        public void BuildTree()
        {
            TreeNode parent;
            foreach (var node in Nodes.Values)
            {
                if (Nodes.TryGetValue(node.Parent, out parent) &&
                    node.Index != node.Parent)
                {
                    node.TreeNodes = parent;
                    parent.ChildList.Add(node);
                }
            }
        }
    }
}


