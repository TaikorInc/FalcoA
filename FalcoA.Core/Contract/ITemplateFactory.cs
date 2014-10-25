using System;
using System.Xml;

namespace FalcoA.Core
{
    public interface ITemplateFactory
    {
        ITemplate GetCrawlTemplate(String addr, params object[] parameters);

        ITemplate GetCrawlTemplate(XmlDocument addr, params object[] parameters);
    }
}
