using System;
using System.Linq;

namespace FalcoA.Core.Phases
{
    public class PhaseClickScan : PhaseClickScanBase
    {
        private Int32 _count;

        public override PhaseResult Run(Context context)
        {
            Int32 max = _count < 0 ? Int32.MaxValue : _count;
            Int32 count = 0;

            for (; count < max; count++)
            {
                String previous = context.LastRequestContent;

                PhaseResult pr = Parse.Run(context);
                context.PushResult(pr);

                pr = Click.Run(context);
                context.PushResult(pr);

                //TODO:如果JsonResult里有重复的项的时候就停止
                // 如果前后的html一样
                if (previous == context.LastRequestContent)
                {
                    break;
                }
            }

            PhaseResult clickScanResult = new PhaseResult(this);
            clickScanResult.SetInt(Constant.RVCount, count + 1);
            clickScanResult.Succeed = true;

            return clickScanResult;
        }

        public static PhaseClickScan Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (!parameters.Descends.ContainsKey(Constant.ClickNode) ||
                !parameters.Descends.ContainsKey(Constant.ParseNode))
            {
                return null;
            }

            PhaseClickScan scan = new PhaseClickScan();

            scan.Count = parameters.Attributes.ContainsKey(Constant.CountAttr) ? parameters.Attributes[Constant.CountAttr] : "-1";

            Int32 count;

            if (!Int32.TryParse(scan.Count, out count))
            {
                return null;
            }
            scan._count = count;

            IPhase click = TemplateGenHelper.GeneratePhaseFromTreeNode(parameters.Descends[Constant.RequestNode].Descends.FirstOrDefault().Value, useBrowser);
            IPhase parse = TemplateGenHelper.GeneratePhaseFromTreeNode(parameters.Descends[Constant.ParseNode].Descends.FirstOrDefault().Value, useBrowser);

            if (!(click is PhaseClick))
            {
                return null;
            }

            scan.Click = click;
            scan.Parse = parse;

            return scan;
        }
    }

    public abstract class PhaseClickScanBase : IPhase
    {
        public String Count { get; set; }

        public IPhase Click { get; set; }

        public IPhase Parse { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
