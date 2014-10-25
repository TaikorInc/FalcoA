using System;

namespace FalcoA.Core
{
    public class PhaseAccount : IPhase
    {
        public String Username { get; set; }

        public String Password { get; set; }

        public PhaseResult Run(Context context)
        {
            Account account = new Account();
            account.UserName = context.Resolve(Username);
            account.Password = context.Resolve(Password);
            context.Account = account;

            PhaseResult pr = new PhaseResult(this);
            pr.Succeed = true;
            return pr;
        }

        public static PhaseAccount Create(TreeNode parameters, Boolean userBrowser = false)
        {
            if (!parameters.Descends.ContainsKey(Constant.UsernameNode) ||
                !parameters.Descends.ContainsKey(Constant.PasswordNode))
            {
                return null;
            }

            PhaseAccount account = new PhaseAccount();

            account.Username = parameters.Descends[Constant.UsernameNode].Value;
            account.Password = parameters.Descends[Constant.PasswordNode].Value;

            return account;
        }
    }
}
