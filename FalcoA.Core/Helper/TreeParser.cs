using System;
using System.Web;
using System.Xml;

namespace FalcoA.Core
{
    public class TreeParser
    {
        public static TreeNode Parse(XmlNode xml)
        {
            TreeNode node = new TreeNode();
            node.Name = xml.Name;

            if (xml.Attributes != null)
            {
                foreach (XmlAttribute attr in xml.Attributes)
                {
                    node.Attributes.Add(attr.Name, HttpUtility.HtmlDecode(attr.Value));
                }
            }

            Int32 elementChild = 0;
            if (xml.ChildNodes != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element)
                    {
                        elementChild++;
                        node.Add(Parse(child));
                    }
                }
            }

            if (elementChild == 0)
            {
                //node.Value = HttpUtility.HtmlDecode(xml.InnerText).Trim();
                node.Value = xml.InnerText.Trim();
            }

            return node;
        }
    }
}
