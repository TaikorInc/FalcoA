using Gecko;
using System;
using System.Linq;

namespace FalcoA.Core
{
    public enum LocatorMethod
    {
        /// <summary>
        /// 用元素的ID来定位
        /// </summary>
        ID = 0,

        /// <summary>
        /// 用元素的Name来定位
        /// </summary>
        Name,

        /// <summary>
        /// 用XPath来定位一个元素
        /// </summary>
        XPath,
    }

    public class DomElementLocator
    {
        private LocatorMethod _method;

        public String Locator { get; set; }

        public String Name { get; set; }

        public DomElementLocator(LocatorMethod method, String locator)
        {
            _method = method;

            Locator = locator;
        }

        public DomElementLocator(LocatorMethod method, String locator, String name)
            : this(method, locator)
        {
            Name = name;
        }

        public GeckoHtmlElement LocateElement(GeckoWebBrowser browser)
        {
            switch (_method)
            {
                case LocatorMethod.ID:
                    return browser.Document.GetHtmlElementById(Locator);
                case LocatorMethod.Name:
                    return browser.Document.GetElementsByName(Locator).FirstOrDefault();
                case LocatorMethod.XPath:
                    return browser.Document.SelectSingle(Locator) as GeckoHtmlElement;
                default:
                    throw new NotSupportedException("目前DomElementLocator只支持ID, Name, XPath三种定位方式");
            }
        }
    }
}
