using System;
using System.Collections.Generic;

namespace FalcoA.Core
{
    public class PhaseLogout : IPhase
    {
        private List<IPhase> _logoutPhases = new List<IPhase>();

        public PhaseResult Run(Context context)
        {
            Boolean succ = true;

            foreach (IPhase phase in _logoutPhases)
            {
                PhaseResult pr = phase.Run(context);
                context.PushResult(pr);
                if (!pr.Succeed)
                {
                    succ = false;
                    break;
                }
            }

            PhaseResult result = new PhaseResult(this);
            result.Succeed = succ;

            return result;
        }

        public static PhaseLogout Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends.Count == 0)
            {
                return null;
            }

            PhaseLogout login = new PhaseLogout();

            foreach (TreeNode node in parameters.Descends.Values)
            {
                IPhase phase = TemplateGenHelper.GeneratePhaseFromTreeNode(node, useBrowser);
                if (phase == null)
                {
                    return null;
                }
                login._logoutPhases.Add(phase);
            }

            return login;
        }
    }
}
