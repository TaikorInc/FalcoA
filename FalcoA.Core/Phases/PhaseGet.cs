using Gecko;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FalcoA.Core
{
    /// <summary>
    /// Get using Gecko
    /// </summary>
    public class PhaseGet : PhaseGetBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Succeed表示http请求返回是否为Succ，Constant.RVHttpRequestResult表示返回的html字符串</returns>
        public override PhaseResult Run(Context context)
        {
            GeckoWebBrowser browser = (GeckoWebBrowser)context.GetService(typeof(GeckoWebBrowser));
            Debug.Assert(browser != null, "browser is null");

            String url = context.Resolve(Url);
            Debug.Assert(!String.IsNullOrWhiteSpace(url), "Url is null or empty");

            String result = RequestHelper.BrowserGet(browser, url);
            PhaseResult pr = new PhaseResult(this);
            pr.SetString(Constant.RVHttpRequestResult, result);
            pr.Succeed = true;

            context.LastRequestContent = result ?? String.Empty;

            return pr;
        }

        public static PhaseGet Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends == null)
            {
                return null;
            }

            try
            {
                PhaseGet get = new PhaseGet();
                get.Url = parameters.Descends[Constant.UrlNode].Value;
                return get;
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Get using plain http request
    /// </summary>
    public class PhasePlainGet : PhaseGetBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Succeed表示http请求返回是否为Succ，Constant.RVHttpRequestResult表示返回的html字符串</returns>
        public override PhaseResult Run(Context context)
        {
            String url = context.Resolve(Url);
            Debug.Assert(!String.IsNullOrWhiteSpace(url), "Url is null or empty");

            Boolean succ = false;
            String result = null;
            Int32 retry = 3;

            while (!succ && retry-->0)
            {
                result = RequestHelper.Get(url);
                succ = !String.IsNullOrWhiteSpace(result);
            }

            PhaseResult pr = new PhaseResult(this);
            pr.SetString(Constant.RVHttpRequestResult, result);
            pr.Succeed = succ;

            context.LastRequestContent = result ?? String.Empty;

            return pr;
        }

        public static PhasePlainGet Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends == null)
            {
                return null;
            }

            try
            {
                PhasePlainGet get = new PhasePlainGet();
                get.Url = parameters.Descends[Constant.UrlNode].Value;
                return get;
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }
    }

    public abstract class PhaseGetBase : IPhase, IParameterUpdatable<Int32>
    {
        public String Url { get; set; }

        // Used to update the current url;
        private String _rawUrl;

        public abstract PhaseResult Run(Context context);

        //TODO::像慧科这种翻页的时候依靠url里带记录条数的需要额外定制一个Updater
        //TODO::提供模板里对val进行计算的表达式比如$(50@2)表示50*val + 2;
        public void Update(Context context, int val)
        {
            // Calling Update for the first time;
            if (String.IsNullOrWhiteSpace(_rawUrl))
            {
                _rawUrl = context.Resolve(Url);
            }

            Url = _rawUrl.Replace(Constant.UpdatablePlaceHolder, val.ToString());
        }
    }
}
