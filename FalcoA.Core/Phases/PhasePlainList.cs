using System;
using System.Collections.Generic;
using System.Linq;

namespace FalcoA.Core
{
    public class PhasePlainList : PhasePlainListBase
    {
        public override PhaseResult Run(Context context)
        {
            PhaseResult pr = new PhaseResult(this);
            pr.ListResult = List;

            return pr;
        }

        public static PhasePlainList Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.ListDescends == null || parameters.ListDescends.Count == 0)
            {
                return null;
            }

            PhasePlainList list = new PhasePlainList();

            list.List = new List<string>();

            list.List.AddRange(
                parameters.ListDescends
                .Where(o => !String.IsNullOrWhiteSpace(o.Value))
                .Select(o => o.Value));

            return list;
        }
    }

    public abstract class PhasePlainListBase : IPhase
    {
        public List<String> List { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
