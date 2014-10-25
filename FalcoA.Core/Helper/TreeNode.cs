using System;
using System.Collections.Generic;

namespace FalcoA.Core
{
    /// <summary>
    /// Xml格式的模板解析完之后存放在一个树结构中
    /// </summary>
    public class TreeNode
    {
        public Dictionary<String, String> Attributes = new Dictionary<string, string>();

        public String Name { get; set; }

        public String Value { get; set; }

        public Dictionary<String, TreeNode> Descends { get; set; }

        public List<TreeNode> ListDescends { get; set; }

        public void Add(TreeNode node)
        {
            if (Descends == null)
            {
                Descends = new Dictionary<string, TreeNode>();
                ListDescends = new List<TreeNode>();
            }

            // Descends存放第一次出现，一般用来构建Phase的时候直接取用
            // ListDescends存放所有有序和重复出现的对象
            if (!Descends.ContainsKey(node.Name))
            {
                Descends.Add(node.Name, node);
            }

            ListDescends.Add(node);
        }
    }
}
