using Gecko;
using System;
using System.Diagnostics;

namespace FalcoA.Core.Phases
{
    public class PhaseFocus : PhaseFocusBase
    {
        public override PhaseResult Run(Context context)
        {
            GeckoWebBrowser browser = (GeckoWebBrowser)context.GetService(typeof(GeckoWebBrowser));
            Debug.Assert(browser != null, "browser is null");

            Locator.Locator = context.Resolve(Locator.Locator);
            Boolean succ = RequestHelper.OperateBrowserSetFocus(browser, Locator);

            PhaseResult pr = new PhaseResult(this);
            pr.Succeed = succ;
            context.LastRequestContent = RequestHelper.GetGeckoContent(browser) ?? String.Empty;

            return pr;
        }

        public static PhaseFocus Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (!parameters.Descends.ContainsKey(Constant.TargetNode))
            {
                return null;
            }

            PhaseFocus focus = new PhaseFocus();

            String target = parameters.Descends[Constant.TargetNode].Value;
            focus.Locator = TemplateGenHelper.GenerateLocator(target);

            return focus;
        }
    }

    public abstract class PhaseFocusBase : IPhase
    {
        public DomElementLocator Locator { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
