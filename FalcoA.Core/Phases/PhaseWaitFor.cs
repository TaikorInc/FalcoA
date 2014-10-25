using Gecko;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace FalcoA.Core
{
    public class PhaseWaitFor : PhaseWaitForBase
    {
        public override PhaseResult Run(Context context)
        {
            PhaseResult pr = new PhaseResult(this);

            GeckoWebBrowser browser = (GeckoWebBrowser)context.GetService(typeof(GeckoWebBrowser));
            Debug.Assert(browser != null, "browser is null");

            if (WaitMilliseconds > 0)
            {
                // 等待一定的毫秒数
                Thread.Sleep(WaitMilliseconds);
            }
            else
            {
                String urlRegex = context.Resolve(UrlRegex);
                String contentRegex = context.Resolve(ContentRegex);
                
                // TODO::加上Timeout!!!返回结果只有为Timeout的时候才算失败
                // 先等待Url的Pattern
                if (!String.IsNullOrWhiteSpace(urlRegex))
                {
                    while (browser.Document.Uri == null || !Regex.IsMatch(browser.Document.Uri, urlRegex))
                    {
                        Thread.Sleep(200);
                    }
                }

                // 再等待Content的Pattern
                if (!String.IsNullOrWhiteSpace(contentRegex))
                {
                    while (!Regex.IsMatch(RequestHelper.GetGeckoContent(browser), contentRegex))
                    {
                        Thread.Sleep(200);
                    }
                }
            }

            string content = RequestHelper.GetGeckoContent(browser);
            context.LastRequestContent = content ?? String.Empty;
            pr.SetString(Constant.RVHttpRequestResult, content);
            pr.Succeed = true;
            return pr;
        }

        public static PhaseWaitFor Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends == null)
            {
                return null;
            }

            PhaseWaitFor waitFor = new PhaseWaitFor();
            if (parameters.Descends.ContainsKey(Constant.WaitMillisecondsNode))
            {
                Int32 waitMS;
                Int32.TryParse(parameters.Descends[Constant.WaitMillisecondsNode].Value, out waitMS);
                waitFor.WaitMilliseconds = waitMS;
            }

            if (parameters.Descends.ContainsKey(Constant.UrlRegexNode))
            {
                waitFor.UrlRegex = parameters.Descends[Constant.UrlRegexNode].Value;
            }

            if (parameters.Descends.ContainsKey(Constant.ContentRegexNode))
            {
                waitFor.ContentRegex = parameters.Descends[Constant.ContentRegexNode].Value;
            }

            return waitFor;
        }
    }

    //TODO::添加禁止条件，UrlForbiddenRegex和ContentForbiddenRegex
    public abstract class PhaseWaitForBase : IPhase
    {
        public Int32 WaitMilliseconds { get; set; }

        public String UrlRegex { get; set; }

        public String ContentRegex { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
