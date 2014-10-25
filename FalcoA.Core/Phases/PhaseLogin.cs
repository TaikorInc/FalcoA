using System;
using System.Collections.Generic;

namespace FalcoA.Core
{
    //TODO:把PhaseLogin和PhaseLogout用一样的方式处理，提供子模板的选项
    public class PhaseLogin : IPhase
    {
        private List<IPhase> _loginPhases = new List<IPhase>();

        public PhaseResult Run(Context context)
        {
            Boolean succ = true;

            foreach (IPhase phase in _loginPhases)
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

        public static PhaseLogin Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends.Count == 0)
            {
                return null;
            }

            PhaseLogin login = new PhaseLogin();
            
            foreach (TreeNode node in parameters.ListDescends)
            {
                IPhase phase = TemplateGenHelper.GeneratePhaseFromTreeNode(node, useBrowser);
                if (phase == null)
                {
                    return null;
                }
                login._loginPhases.Add(phase);
            }

            return login;
        }
    }
}
