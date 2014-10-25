using System;
using System.Collections.Generic;
using System.Linq;

namespace FalcoA.Core
{
    public class PhaseSimpleScan : IPhase
    {
        public IPhase Request { get; set; }

        public IPhase Parse { get; set; }

        public String ListID { get; set; }

        public String Binding { get; set; }

        public PhaseResult Run(Context context)
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
                    //IDataProvider provider = new BasicDataProvider();
                    // TODO:使用动态绑定来获取url
                    // provider.SetString("url", json);
                    try
                    {
                        IDataProvider provider = BasicDataProvider.CreateFromJson(json);
                        context.RuntimeProviders.Push(provider);

                        PhaseResult result = Request.Run(context);
                        context.PushResult(result);
                        result = Parse.Run(context);
                        context.PushResult(result);

                        if (result.ListResult != null)
                        {
                            pr.ListResult.AddRange(result.ListResult);
                        }
                    }
                    finally
                    {
                        context.RuntimeProviders.Pop();
                    }
                }
            }
            else
            {

                PhaseResult result = Request.Run(context);
                context.PushResult(result);
                result = Parse.Run(context);
                context.PushResult(result);

                if (result.ListResult != null)
                {
                    pr.ListResult.AddRange(result.ListResult);
                }
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

        public static PhaseSimpleScan Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (!parameters.Descends.ContainsKey(Constant.RequestNode) ||
                !parameters.Descends.ContainsKey(Constant.ParseNode))
            {
                throw new ArgumentException("Request and Parse nodes are both expected for Scan phase.");
            }

            PhaseSimpleScan scan = new PhaseSimpleScan();

            scan.Binding = parameters.Attributes.ContainsKey(Constant.ListBindingAttr) ? parameters.Attributes[Constant.ListBindingAttr] : null;
            scan.ListID = parameters.Attributes.ContainsKey(Constant.ListIDAttr) ? parameters.Attributes[Constant.ListIDAttr] : null;
            scan.Request = TemplateGenHelper.GeneratePhaseFromTreeNode(parameters.Descends[Constant.RequestNode].Descends.FirstOrDefault().Value);
            scan.Parse = TemplateGenHelper.GeneratePhaseFromTreeNode(parameters.Descends[Constant.ParseNode].Descends.FirstOrDefault().Value);

            return scan;
        }
    }
}
