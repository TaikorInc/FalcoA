using Gecko;
using System;
using System.Threading;
using System.Windows.Forms;

namespace FalcoA.Core
{
    /// <summary>
    /// 跟Gecko浏览器相关的一些操作，可以在除了Gecko线程以外的线程里调用
    /// </summary>
    public sealed class RequestHelper
    {
        /// <summary>
        /// 使用browser访问url链接，将结果存在GeckoWebBrowser的Response里
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="url"></param>
        public static String BrowserGet(GeckoWebBrowser browser, String url)
        {
            //var response = GeckoRequestProcessor.DoRequest(CrawlRequest.BuildRequest(url), null, null, null, specifiedBrowser: browser);
            //browser.Response = response;
            //return response.Content;
            return (String)browser.Invoke(new GeckoOperationOne<String>(GeckoGet), browser, url);
        }

        /// <summary>
        /// 使用browser向地址url发送数据data，将返回结果放在GeckoWebBrowser的Response里
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        public static String BrowserPost(GeckoWebBrowser browser, String url, String data)
        {
            //CrawlRequest request = CrawlRequest.BuildRequest(url);
            //request.PostData = data;
            //var response = GeckoRequestProcessor.DoRequest(request, null, null, null, specifiedBrowser: browser);
            //browser.Response = response;
            //return response.Content;
            return (String)browser.Invoke(new GeckoOperationTwo<String>(GeckoPost), browser, url, data);
        }

        public static String Get(String url)
        {
            return HttpHelper.GetHttpContent(url);
        }

        public static String Post(String url, String data)
        {
            return HttpHelper.GetHttpContent(url, data);
        }

        /// <summary>
        /// 对browser当前document里的locator所指向的元素进行单击操作
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="locator"></param>
        /// <returns>是否找到并点击了这个元素</returns>
        public static Boolean OperateBrowserClick(GeckoWebBrowser browser, DomElementLocator locator)
        {
            return (Boolean)browser.Invoke(new GeckoOperationOne<Boolean>(GeckoClick), browser, locator);
        }

        /// <summary>
        /// 对browser当前document里locator所指向的元素填入数据inputValue
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="locator"></param>
        /// <param name="inputValue"></param>
        public static Boolean OperateBrowserInput(GeckoWebBrowser browser, DomElementLocator locator, String inputValue)
        {
            return (Boolean)browser.Invoke(new GeckoOperationTwo<Boolean>(GeckoInput), browser, locator, inputValue);
        }

        /// <summary>
        /// 把browser当前document里locator所指向的元素设置成focus状态
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="locator"></param>
        public static Boolean OperateBrowserSetFocus(GeckoWebBrowser browser, DomElementLocator locator)
        {
            return (Boolean)browser.Invoke(new GeckoOperationOne<Boolean>(GeckoFocus), browser, locator);
        }

        /// <summary>
        /// 把browser当前document里locator所指向的元素触发MouseOver的事件
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="locator"></param>
        public static void OperateBrowserMouseOver(GeckoWebBrowser browser, DomElementLocator locator)
        {

        }

        /// <summary>
        /// 把GeckoWebBrowser当前的Document里的数据取出放入到GeckoWebBrowser的Response字段里
        /// </summary>
        /// <param name="browser"></param>
        //public static void OperateBrowserGetContent(GeckoWebBrowser browser)
        //{
        //    browser.Invoke(new GeckoOperation(GeckoContentUpdate), browser);
        //}

        #region Base operations

        public delegate void GeckoOperation(GeckoWebBrowser browser);

        private delegate T GeckoOperationOne<T>(GeckoWebBrowser browser, Object obj);

        private delegate T GeckoOperationTwo<T>(GeckoWebBrowser browser, Object obj1, Object obj2);

        public static string GetGeckoContent(GeckoWebBrowser browser)
        {
            string content = string.Empty;
            try
            {
                content = browser.Document.Body.Parent.OuterHtml; //第一种方式
            }
            catch (Exception)
            {
                try
                {
                    content = browser.Document.Body.OuterHtml; //第二种方式
                }
                catch { }//取Dom内容都失败，则Gecko坏，返回空
            }
            return content;
        }

        public static void GeckoStop(GeckoWebBrowser browser)
        {
            browser.Stop();
        }

        public static void GeckoWaitForComplete(GeckoWebBrowser browser)
        {
            //TODO:使得Gecko的Timeout可以被配置，在TaskSettings里配置。
            //browser.Navigating.WaitOne(60000);
            while (browser.Document.ReadyState != "complete" && browser.Document.ReadyState != "interactive")
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }

            //GeckoContentUpdate(browser);
        }

        public static void GeckoDocumentCompleted(object sender, EventArgs e)
        {
            GeckoWebBrowser browser = sender as GeckoWebBrowser;
            browser.DocumentCompleted -= GeckoDocumentCompleted;
        }

        private static Boolean GeckoClick(GeckoWebBrowser browser, Object locator)
        {
            DomElementLocator loc = locator as DomElementLocator;

            if (loc == null)
            {
                return false;
            }

            GeckoHtmlElement element = loc.LocateElement(browser);
            if (element == null)
            {
                return false;
            }
            else
            {
                element.Click();
                GeckoWaitForComplete(browser);
                return true;
            }
        }

        private static Boolean GeckoFocus(GeckoWebBrowser browser, Object locator)
        {
            DomElementLocator loc = locator as DomElementLocator;

            if (loc == null)
            {
                return false;
            }

            GeckoHtmlElement element = loc.LocateElement(browser);
            if (element == null)
            {
                return false;
            }
            else
            {
                element.Focus();
                GeckoWaitForComplete(browser);
                return true;
            }
        }

        private static String GeckoGet(GeckoWebBrowser browser, Object url)
        {
            String u = url as String;
            if (u == null)
                return null;

            browser.DocumentCompleted += GeckoDocumentCompleted;
            browser.Stop();
            browser.Navigate(u);

            GeckoWaitForComplete(browser);

            return GetGeckoContent(browser);
        }

        private static String GeckoPost(GeckoWebBrowser browser, Object url, Object data)
        {
            String u = url as String;
            String d = data as String;

            if (u == null || d == null)
            {
                return null;
            }

            Gecko.IO.MimeInputStream post = Gecko.IO.MimeInputStream.Create();
            //GeckoMIMEInputStream post = new GeckoMIMEInputStream();
            post.SetData(d);
            post.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            post.AddContentLength = true;
            String referer = browser.ReferrerUrl.ToString();

            browser.DocumentCompleted += GeckoDocumentCompleted;
            browser.Stop();
            browser.Navigate(u, GeckoLoadFlags.None, referer, post);

            GeckoWaitForComplete(browser);

            return GetGeckoContent(browser);
        }

        private static Boolean GeckoInput(GeckoWebBrowser browser, Object locator, Object data)
        {
            DomElementLocator loc = locator as DomElementLocator;
            String d = data as String;
            if (loc == null || d == null)
                return false;

            GeckoHtmlElement element = loc.LocateElement(browser);
            if (element == null)
                return false;

            element.SetAttribute("type", "button");
            element.SetAttribute("value", d);
            element.SetAttribute("type", "text");

            return true;
        }

        #endregion
    }
}
