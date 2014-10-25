using Gecko;
using System;

namespace FalcoA.Core
{
    public class GeckoContext : Context
    {
        private GeckoWebBrowser _browser;

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(GeckoWebBrowser))
            {
                return _browser;
            }
            else
            {
                return null;
            }
        }

        public GeckoContext(GeckoWebBrowser browser)
        {
            _browser = browser;
        }

        public static Context GetDefaultContext()
        {
            GeckoWebBrowser browser = new GeckoWebBrowser();
            GeckoContext gc = new GeckoContext(browser);
            return gc;
        }
    }
}
