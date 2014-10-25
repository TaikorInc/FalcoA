using Gecko;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FalcoA.Core
{
    /// <summary>
    /// 使用Gecko浏览器里的当前Url作为解析对象，应用RegularExpression
    /// </summary>
    public class PhaseParseUrl : PhaseParseUrlBase
    {
        public override PhaseResult Run(Context context)
        {
            GeckoWebBrowser browser = (GeckoWebBrowser)context.GetService(typeof(GeckoWebBrowser));
            Debug.Assert(browser != null, "browser is null");

            String regex = context.Resolve(RegularExpression);
            Debug.Assert(String.IsNullOrWhiteSpace(regex), "regular expression is null!");

            String url = browser.Document.Uri;
            Match match = Regex.Match(url, regex);

            PhaseResult pr = new PhaseResult(this);

            if (match.Success)
            {
                // 表达式中所有的Group的id名
                List<String> ids = RegexHelper.ParseGroupIndexNames(regex);

                // 按照Group的名字写入到ParameterProvider
                foreach (String id in ids)
                {
                    context.ParameterProvider.SetString(id, match.Groups[id].Value);
                }

                pr.Succeed = true;
                pr.SetInt(Constant.RVCount, ids.Count);
            }
            else
            {
                pr.Succeed = false;
            }

            return pr;
        }

        public static PhaseParseUrl Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends == null)
            {
                return null;
            }

            try
            {
                PhaseParseUrl parseUrl = new PhaseParseUrl();
                parseUrl.RegularExpression = parameters.Descends[Constant.RegexNode].Value;
                return parseUrl;
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }
    }

    public abstract class PhaseParseUrlBase : IPhase
    {
        /// <summary>
        /// 所有Capture住的字符串的Group必须使用(?<[id]>..)的形式
        /// </summary>
        public String RegularExpression { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
