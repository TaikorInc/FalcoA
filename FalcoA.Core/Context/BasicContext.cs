using System;

namespace FalcoA.Core
{
    public class BasicContext : Context
    {
        public override object GetService(Type serviceType)
        {
            return null;
        }

        public static Context GetDefaultContext()
        {
            return new BasicContext();
        }
    }
}
