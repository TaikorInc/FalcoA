using Gecko;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FalcoA.Core
{
    /// <summary>
    /// 使用Gecko浏览器来做Post请求
    /// </summary>
    public class PhasePost : PhasePostBase
    {
        public override PhaseResult Run(Context context)
        {
            GeckoWebBrowser browser = (GeckoWebBrowser)context.GetService(typeof(GeckoWebBrowser));
            Debug.Assert(browser != null, "browser is null");

            String url = context.Resolve(Url);
            String data = context.Resolve(Data);
            Debug.Assert(!String.IsNullOrWhiteSpace(url), "Url is null or empty");

            String result = RequestHelper.BrowserPost(browser, url, data);
            PhaseResult pr = new PhaseResult(this);
            pr.SetString(Constant.RVHttpRequestResult, result);
            pr.Succeed = true;

            context.LastRequestContent = result ?? String.Empty;

            return pr;
        }

        public static PhasePost Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends == null)
            {
                return null;
            }

            try
            {
                PhasePost post = new PhasePost();
                post.Url = parameters.Descends[Constant.UrlNode].Value;
                post.Data = parameters.Descends[Constant.DataNode].Value;
                return post;
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }
    }

    public class PhasePlainPost : PhasePostBase
    {
        public override PhaseResult Run(Context context)
        {
            String url = context.Resolve(Url);
            String data = context.Resolve(Data);
            Debug.Assert(!String.IsNullOrWhiteSpace(url), "Url is null or empty");

            String result = RequestHelper.Post(url, data);
            PhaseResult pr = new PhaseResult(this);
            pr.SetString(Constant.RVHttpRequestResult, result);
            pr.Succeed = !String.IsNullOrWhiteSpace(result);

            context.LastRequestContent = result ?? String.Empty;

            return pr;
        }

        public static PhasePlainPost Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends == null)
            {
                return null;
            }

            try
            {
                PhasePlainPost post = new PhasePlainPost();
                post.Url = parameters.Descends[Constant.UrlNode].Value;
                post.Data = parameters.Descends[Constant.DataNode].Value;
                return post;
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }
    }

    public abstract class PhasePostBase : IPhase, IParameterUpdatable<Int32>
    {
        public String Url { get; set; }

        public String Data { get; set; }

        // Used to update the current url and post data
        private String _rawUrl;

        private String _rawData;

        public abstract PhaseResult Run(Context context);

        public void Update(Context context, int val)
        {
            // Calling update for the first time
            if (String.IsNullOrWhiteSpace(_rawUrl) && String.IsNullOrWhiteSpace(_rawData))
            {
                _rawData = context.Resolve(Data);
                _rawUrl = context.Resolve(Url);
            }

            Url = _rawUrl.Replace(Constant.UpdatablePlaceHolder, val.ToString());
            Data = _rawData.Replace(Constant.UpdatablePlaceHolder, val.ToString());
        }
    }
}
