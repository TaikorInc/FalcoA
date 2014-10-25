using System;
using System.IO;
using System.Xml;

namespace FalcoA.Core
{
    public class GeneralTemplateFactory : ITemplateFactory
    {
        /// <summary>
        /// 根据输入来定位到一个Template模板位置
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ITemplate GetCrawlTemplate(string addr, params object[] parameters)
        {
            //CrawlEntity crawl = CrawlBusiness.GetByCrawlID(addr);
            XmlDocument xml = LoadTemplateDocument(addr);
            GeneralTemplate template = CreateTemplate(xml);

            return template;
        }

        /// <summary>
        /// 根据输入的模板字符串生成的xml来生成模板
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ITemplate GetCrawlTemplate(XmlDocument xml, params object[] parameters)
        {
            GeneralTemplate template = CreateTemplate(xml);

            return template;
        }

        /// <summary>
        /// 创建只包含默认Phase的模板
        /// </summary>
        /// <param name="xml">Xml格式的模板文件</param>
        /// <returns></returns>
        private GeneralTemplate CreateTemplate(XmlDocument xml)
        {
            XmlNodeList list = xml.SelectNodes(String.Format("/{0}/*", Constant.TemplateNode));
            GeneralTemplate template = new GeneralTemplate();

            Boolean useBrowser = IsBrowserRequired(xml);
            template.UseBrowser = useBrowser;

            foreach (XmlNode node in list)
            {
                TreeNode vals = TreeParser.Parse(node);
                IPhase phase = TemplateGenHelper.GeneratePhaseFromTreeNode(vals, useBrowser);
                if (vals.Name == Constant.LoginNode)
                {
                    template._needLogin = true;
                    template._login = phase;
                }
                else if (vals.Name == Constant.LogoutNode)
                {
                    template._logout = phase;
                }
                else
                {
                    template._phases.Add(phase);
                }
            }

            return template;
        }

        private XmlDocument LoadTemplateDocument(String addr)
        {
            using (FileStream fs = new FileStream(addr, FileMode.Open))
            using (XmlReader xr = XmlReader.Create(fs, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null }))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xr);
                return doc;
            }
        }

        private Boolean IsBrowserRequired(XmlDocument xml)
        {
            try
            {
                var root = xml.SelectSingleNode(String.Format("/{0}", Constant.TemplateNode));
                XmlAttribute attr = root.Attributes[Constant.UseBrowserAttr];
                if (attr.Value == "1" || attr.Value == "Yes")
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
