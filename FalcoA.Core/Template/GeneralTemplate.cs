using Gecko;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FalcoA.Core
{
    public class GeneralTemplate : ITemplate
    {
        internal List<IPhase> _phases = new List<IPhase>();

        internal IPhase _login { get; set; }

        internal IPhase _logout { get; set; }

        internal Boolean _needLogin;

        internal Boolean _loggedIn = false;

        private Context _context;

        private readonly Object _contextLock = new Object();

        public Boolean UseBrowser { get; set; }

        public Context Context
        {
            get
            {
                if (_context == null)
                {
                    lock (_contextLock)
                    {
                        if (_context == null)
                        {
                            _context = UseBrowser ? GeckoContext.GetDefaultContext() : BasicContext.GetDefaultContext();
                        }
                    }
                }

                return _context;
            }
        }

        public TemplateCrawlResult Run(IDataProvider provider)
        {
            if (_phases.Count == 0)
            {
                String msg = "No phases configured for this template, aborting";
                throw new InvalidOperationException(msg);
            }

            Context.ParameterProvider = provider;

            RunInternal();

            TemplateCrawlResult result = new TemplateCrawlResult();
            result.JsonResult = _context.JsonResult;
            result.Succeed = true;

            // 如果存在Gecko浏览器则将它释放
            if (UseBrowser)
            {
                GeckoWebBrowser browser = (GeckoWebBrowser)Context.GetService(typeof(GeckoWebBrowser));
                if (browser != null)
                {
                    browser.Dispose();
                }
            }

            return result;
        }

        public Task<TemplateCrawlResult> RunAsync(IDataProvider provider)
        {
            throw new NotImplementedException();
        }

        private void RunInternal()
        {
            foreach (IPhase phase in _phases)
            {
                while (_needLogin)
                {
                    if (_loggedIn)
                    {
                        PhaseResult logoutSucc = _logout.Run(_context);
                        _context.PushResult(logoutSucc);
                        _loggedIn = !logoutSucc.Succeed;
                    }
                    PhaseResult loginSucc = _login.Run(_context);
                    _context.PushResult(loginSucc);
                    _needLogin = !loginSucc.Succeed;
                }

                RunPhase(phase);
            }
        }

        private void RunPhase(IPhase phase)
        {
            PhaseResult pr = phase.Run(_context);
            _context.PushResult(pr);
        }
    }
}
