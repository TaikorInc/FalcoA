using Gecko;
using System;
using System.Diagnostics;

namespace FalcoA.Core
{
    public class PhaseClick : PhaseClickBase
    {
        public override PhaseResult Run(Context context)
        {
            GeckoWebBrowser browser = (GeckoWebBrowser)context.GetService(typeof(GeckoWebBrowser));
            Debug.Assert(browser != null, "browser is null");

            Locator.Locator = context.Resolve(Locator.Locator);
            Boolean succ = RequestHelper.OperateBrowserClick(browser, Locator);

            PhaseResult pr = new PhaseResult(this);
            pr.Succeed = succ;
            context.LastRequestContent = RequestHelper.GetGeckoContent(browser) ?? String.Empty;

            return pr;
        }

        public static PhaseClick Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (!parameters.Descends.ContainsKey(Constant.TargetNode))
            {
                return null;
            }

            PhaseClick click = new PhaseClick();

            String target = parameters.Descends[Constant.TargetNode].Value;
            click.Locator = TemplateGenHelper.GenerateLocator(target);

            return click;
        }
    }

    public abstract class PhaseClickBase : IPhase
    {
        public DomElementLocator Locator { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
