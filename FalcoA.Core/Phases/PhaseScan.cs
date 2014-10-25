using Gecko;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FalcoA.Core
{
    //TODO:保持Phase接口的一致性
    public class PhaseScan : PhaseScanBase
    {
        private IParameterUpdatable<Int32> _updatableRequest;

        private Int32 _from;

        private Int32 _to;

        private Int32 _step;

        private Boolean _toPageInitialized = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Succeed表示是否成功，ListResult表示抓到的Json数据</returns>
        public override PhaseResult Run(Context context)
        {
            return MetaList == null ? RunAsIteration(context) : RunAsNestedList(context);
        }

        public PhaseResult RunAsNestedList(Context context)
        {
            PhaseResult list = MetaList.Run(context);
            PhaseResult pr = new PhaseResult(this);
            List<String> jsonResult = new List<string>();

            if (list.ListResult != null)
            {
                GeckoWebBrowser browser = (GeckoWebBrowser)context.GetService(typeof(GeckoWebBrowser));
                foreach (String item in list.ListResult)
                {
                    Dictionary<String, String> itemDict = JsonConvert.DeserializeObject<Dictionary<String, String>>(item);
                    if (itemDict.ContainsKey(MetaListUrlKey))
                    {
                        String url = itemDict[MetaListUrlKey];
                        if (!url.StartsWith("http"))
                        {
                            url = String.Format("{0}/{1}", "http://club.autohome.com.cn", url.TrimStart('/'));
                        }
                        String content = String.Empty;
                        if (browser != null)
                        {
                            content = RequestHelper.BrowserGet(browser, url);
                        }
                        else
                        {
                            content = RequestHelper.Get(url);
                        }
                        context.LastRequestContent = content;
                        PhaseResult parseResult = Parse.Run(context);
                        context.PushResult(parseResult);
                        jsonResult.AddRange(parseResult.ListResult ?? new List<String>());
                    }
                }
            }

            pr.ListResult = jsonResult;
            pr.Succeed = true;
            return pr;
        }

        public PhaseResult RunAsIteration(Context context)
        {
            Initialize(context);
            Boundary bound = _step > 0 ? new Boundary(LessOrEqual) : new Boundary(GreaterOrEqual);
            List<String> jsonResult = new List<string>();

            for (int i = _from; bound(i, _to); i += _step)
            {
                _updatableRequest.Update(context, i);
                PhaseResult pr = Request.Run(context);
                context.PushResult(pr);

                // 检查是否可以更新_to
                if (!_toPageInitialized)
                {
                    Match match = Regex.Match(context.LastRequestContent, To);
                    String pageStr = match.Groups[1].Value;
                    Int32 page;
                    if (Int32.TryParse(pageStr, out page))
                    {
                        _to = page;
                        _toPageInitialized = true;
                    }
                }

                pr = Parse.Run(context);
                context.PushResult(pr);

                if (pr.ListResult != null)
                {
                    jsonResult.AddRange(pr.ListResult);
                }
            }

            PhaseResult result = new PhaseResult(this);
            result.Succeed = true;
            result.ListResult = jsonResult;

            context.PushResult(result);

            return result;
        }

        private delegate Boolean Boundary(Int32 a, Int32 b);

        private Boolean LessOrEqual(Int32 a, Int32 b)
        {
            return a <= b;
        }

        private Boolean GreaterOrEqual(Int32 a, Int32 b)
        {
            return a >= b;
        }

        private void Initialize(Context context)
        {
            Int32 to;
            if (Int32.TryParse(To, out to))
            {
                _to = to;
                _toPageInitialized = true;
            }
            else
            {
                // To是一个正则表达式，这种情况下则需要在第一次请求的时候进行解析
                // 如果解析失败，则程序运行一次后自动会停下并返回(_step != 0)
                _to = _from;
            }
        }

        //TODO:把所有参数的解析都延迟到运行时，比如这里的From和To。
        public static PhaseScan Create(TreeNode parameters, Boolean useBrowser = false)
        {
            // RequestNode和ListNode至少要有一个
            if (!(parameters.Descends.ContainsKey(Constant.RequestNode) ||
                parameters.Descends.ContainsKey(Constant.ListNode)) ||
                !parameters.Descends.ContainsKey(Constant.ParseNode))
            {
                return null;
            }

            PhaseScan scan = new PhaseScan();

            // 只有To是可以留到Phase创建之后才解析的
            scan.From = parameters.Attributes.ContainsKey(Constant.FromAttr) ? parameters.Attributes[Constant.FromAttr] : "1";
            scan.To = parameters.Attributes.ContainsKey(Constant.ToAttr) ? parameters.Attributes[Constant.ToAttr] : "1";
            scan.Step = parameters.Attributes.ContainsKey(Constant.StepAttr) ? parameters.Attributes[Constant.StepAttr] : "1";

            Int32 integer;
            
            if (!Int32.TryParse(scan.From, out integer))
            {
                return null;
            }
            scan._from = integer;

            if (!Int32.TryParse(scan.Step, out integer))
            {
                return null;
            }
            scan._step = integer;

            IPhase request = null;
            if (parameters.Descends.ContainsKey(Constant.RequestNode))
            {
                request = TemplateGenHelper.GeneratePhaseFromTreeNode(parameters.Descends[Constant.RequestNode].Descends.FirstOrDefault().Value, useBrowser);
            }

            IPhase list = null;
            if (parameters.Descends.ContainsKey(Constant.ListNode))
            {
                TreeNode listNode = parameters.Descends[Constant.ListNode];
                list = TemplateGenHelper.GeneratePhaseFromTreeNode(listNode.Descends.FirstOrDefault().Value, useBrowser);
                scan.MetaListUrlKey = "Url";
                if (listNode.Attributes.ContainsKey(Constant.UrlAttr))
                {
                    scan.MetaListUrlKey = listNode.Attributes[Constant.UrlAttr];
                }
            }

            IPhase parse = TemplateGenHelper.GeneratePhaseFromTreeNode(parameters.Descends[Constant.ParseNode].Descends.FirstOrDefault().Value, useBrowser);

            if (request as IParameterUpdatable<Int32> == null && request as PhaseClick == null && list == null)
            {
                return null;
            }

            scan._updatableRequest = request as IParameterUpdatable<Int32>;

            scan.Request = request;
            scan.Parse = parse;
            scan.MetaList = list;

            return scan;
        }
    }

    public abstract class PhaseScanBase : IPhase
    {
        //TODO::通过Gecko点击下一页按钮来翻页
        /// <summary>
        /// 这个Request必须实现IParameterUpdatable的整数接口
        /// </summary>
        public IPhase Request { get; set; }

        public IPhase Parse { get; set; }

        public IPhase MetaList { get; set; }

        public String MetaListUrlKey { get; set; }

        public String From { get; set; }

        /// <summary>
        /// To可以是一个正则表达式
        /// </summary>
        public String To { get; set; }

        public String Step { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
