using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FalcoA.Core
{
    public class PhaseMakeList : PhaseMakeListBase
    {

        public override PhaseResult Run(Context context)
        {
            //PhaseResult last = context.Stack.LastOrDefault();
            List<String> bind = null;
            if (!String.IsNullOrWhiteSpace(Binding))
            {
                if (context.JsonResults.ContainsKey(Binding))
                {
                    bind = context.JsonResults[Binding];
                }
                else
                {
                    bind = new List<string>();
                }
            }
            else
            {
                PhaseResult last = context.Stack.LastOrDefault();
                if (last != null)
                {
                    bind = last.ListResult;
                }
            }

            PhaseResult pr = new PhaseResult(this);
            pr.ListResult = new List<string>();

            if (bind != null)
            {
                foreach (String json in bind)
                {
                    try
                    {
                        IDataProvider provider = ParameterResolver.ParseProvider(json);
                        context.RuntimeProviders.Push(provider);
                        pr.ListResult.AddRange(GenerateOneBatch(context));
                    }
                    finally
                    {
                        context.RuntimeProviders.Pop();
                    }
                }
            }
            else
            {
                pr.ListResult.AddRange(GenerateOneBatch(context));
            }

            if (!String.IsNullOrWhiteSpace(ListID))
            {
                if (context.JsonResults.ContainsKey(ListID))
                {
                    context.JsonResults[ListID] = pr.ListResult;
                }
                else
                {
                    context.JsonResults.Add(ListID, pr.ListResult);
                }
            }

            return pr;
        }

        /// <summary>
        /// context里的RuntimeProvider应该已经设置好
        /// </summary>
        /// <returns></returns>
        private List<String> GenerateOneBatch(Context context)
        {
            Int32 from;
            Int32 to;
            Int32 step;

            List<String> result = new List<string>();

            if (!Int32.TryParse(From, out from))
            {
                throw new ArgumentException("From is corrupted.");
            }

            if (!Int32.TryParse(Step, out step))
            {
                throw new ArgumentException("Step is corrupted.");
            }

            // 如果To不是一个数字，则是一个正则表达式，先使用FirstPage进行请求html
            if (!Int32.TryParse(To, out to))
            {
                if (FirstPage == null)
                {
                    String msg = "When To is a regular expression, FirstPage is expected.";
                    throw new ArgumentException(msg);
                }
                PhaseResult html = FirstPage.Run(context);
                context.PushResult(html);

                Match match = Regex.Match(context.LastRequestContent, To);
                if (!match.Success || !Int32.TryParse(match.Groups[1].Value, out to))
                {
                    to = from;
                }
            }

            //String pattern = context.Resolve(Pattern);
            for (int i = from; i <= to; i += step)
            {
                Dictionary<String, String> dict = new Dictionary<string, string>();
                foreach (Pattern pattern in Patterns)
                {
                    String resolvedPattern = context.Resolve(pattern.RawPattern);
                    if (ParameterResolver.HasDataBinding(resolvedPattern, Constant.RuntimePrefix))
                    {
                        return null;
                    }
                    String gen = resolvedPattern.Replace(Constant.UpdatablePlaceHolder, i.ToString());
                    dict.Add(pattern.Name, gen);
                }
                result.Add(JsonConvert.SerializeObject(dict));
            }

            if (Save)
            {
                context.JsonResult.AddRange(result);
            }

            return result;
        }

        public static PhaseMakeList Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (!parameters.Descends.ContainsKey(Constant.PatternNode))
            {
                throw new ArgumentException("Pattern node is expected for MakeList phase");
            }

            PhaseMakeList list = new PhaseMakeList();

            list.Binding = parameters.Attributes.ContainsKey(Constant.ListBindingAttr) ? parameters.Attributes[Constant.ListBindingAttr] : null;
            list.ListID = parameters.Attributes.ContainsKey(Constant.ListIDAttr) ? parameters.Attributes[Constant.ListIDAttr] : null;
            list.From = parameters.Attributes.ContainsKey(Constant.FromAttr) ? parameters.Attributes[Constant.FromAttr] : "1";
            list.To = parameters.Attributes.ContainsKey(Constant.ToAttr) ? parameters.Attributes[Constant.ToAttr] : "1";
            list.Step = parameters.Attributes.ContainsKey(Constant.StepAttr) ? parameters.Attributes[Constant.StepAttr] : "1";
            list.Hook = parameters.Attributes.ContainsKey(Constant.HookAttr) ? Constant.True(parameters.Attributes[Constant.HookAttr]) : false;
            list.Save = parameters.Attributes.ContainsKey(Constant.SaveAttr) ? Constant.True(parameters.Attributes[Constant.SaveAttr]) : false;

            //list.Pattern = parameters.Descends[Constant.PatternNode].Value;
            // 初始化Pattern
            list.Patterns = new List<Pattern>();
            foreach (var patternNode in parameters.ListDescends.Where(o => o.Name == Constant.PatternNode))
            {
                if (patternNode.Attributes.ContainsKey(Constant.NameAttr) && !String.IsNullOrWhiteSpace(patternNode.Value))
                {
                    list.Patterns.Add(new Pattern
                    {
                        Name = patternNode.Attributes[Constant.NameAttr],
                        RawPattern = patternNode.Value,
                    });
                }
            }
            
            if (parameters.Descends.ContainsKey(Constant.FirstPageNode))
            {
                list.FirstPage = TemplateGenHelper.GeneratePhaseFromTreeNode(parameters.Descends[Constant.FirstPageNode].Descends.FirstOrDefault().Value, useBrowser);
            }

            return list;
        }
    }

    public abstract class PhaseMakeListBase : IPhase
    {
        public String From { get; set; }

        public String To { get; set; }

        public String Step { get; set; }

        public Boolean Hook { get; set; }

        public IPhase FirstPage { get; set; }

        public String ListID { get; set; }

        public String Binding { get; set; }

        public Boolean Save { get; set; }

        protected List<Pattern> Patterns { get; set; }

        public abstract PhaseResult Run(Context context);

        protected class Pattern
        {
            internal String Name { get; set; }

            internal String RawPattern { get; set; }
        }
    }
}
