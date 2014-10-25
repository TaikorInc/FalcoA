using System;
using System.Reflection;

namespace FalcoA.Core
{
    public class TemplateGenHelper
    {
        private static readonly String _plainTypeFormat = "FalcoA.Core.PhasePlain{0}";

        private static readonly String _browserTypeFormat = "FalcoA.Core.Phase{0}";

        public static IPhase GeneratePhaseFromTreeNode(TreeNode node, Boolean useBrowser = false)
        {
            if (node == null)
            {
                return null;
            }

            // 使用Reflection调用对应的PhaseXXX类的Build方法来创建
            String phaseTypeName = String.Format(
                    useBrowser ? _browserTypeFormat : _plainTypeFormat, node.Name);
            Type phaseType = Type.GetType(phaseTypeName);
            if (phaseType == null)
            {
                phaseType = Type.GetType(String.Format(_browserTypeFormat, node.Name));
            }

            if (phaseType == null)
            {
                return null;
            }

            MethodInfo creator = phaseType.GetMethod(Constant.PhaseCreatorMethod);
            return (IPhase)creator.Invoke(null, new object[] { node, useBrowser });
        }

        public static DomElementLocator GenerateLocator(String raw)
        {
            if (!raw.StartsWith(Constant.DOMElementPrefix))
            {
                return null;
            }

            LocatorMethod method;

            raw = raw.Substring(1);
            if (raw.StartsWith(Constant.DOMIDPrefix))
            {
                method = LocatorMethod.ID;
                raw = raw.Substring(Constant.DOMIDPrefix.Length);
            }
            else if (raw.StartsWith(Constant.DOMNamePrefix))
            {
                method = LocatorMethod.Name;
                raw = raw.Substring(Constant.DOMNamePrefix.Length);
            }
            else if (raw.StartsWith(Constant.DOMXPathPrefix))
            {
                method = LocatorMethod.XPath;
                raw = raw.Substring(Constant.DOMXPathPrefix.Length);
            }
            else
            {
                return null;
            }

            return new DomElementLocator(method, raw);
        }
    }
}
