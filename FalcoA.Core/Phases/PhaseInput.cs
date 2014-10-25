using Gecko;
using System;
using System.Diagnostics;

namespace FalcoA.Core
{
    public class PhaseInput : PhaseInputBase
    {
        public override PhaseResult Run(Context context)
        {
            GeckoWebBrowser browser = (GeckoWebBrowser)context.GetService(typeof(GeckoWebBrowser));
            Debug.Assert(browser != null, "browser is null");

            Locator.Locator = context.Resolve(Locator.Locator);

            Boolean succ = RequestHelper.OperateBrowserInput(browser, Locator, context.Resolve(InputValue));

            PhaseResult pr = new PhaseResult(this);
            pr.Succeed = succ;
            context.LastRequestContent = RequestHelper.GetGeckoContent(browser) ?? String.Empty;

            return pr;
        }

        public static PhaseInput Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (!parameters.Descends.ContainsKey(Constant.TargetNode) ||
                !parameters.Descends.ContainsKey(Constant.DataNode))
            {
                return null;
            }

            PhaseInput input = new PhaseInput();

            String target = parameters.Descends[Constant.TargetNode].Value;
            String data = parameters.Descends[Constant.DataNode].Value;
            input.Locator = TemplateGenHelper.GenerateLocator(target);
            input.InputValue = data;

            return input;
        }
    }

    public abstract class PhaseInputBase : IPhase
    {
        public DomElementLocator Locator { get; set; }

        public String InputValue { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
