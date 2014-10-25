using System;
using System.Collections.Generic;

namespace FalcoA.Core
{
    public class TemplateCrawlResult
    {
        public List<String> JsonResult { get; set; }

        public String ErrorMessage { get; set; }

        public Boolean Succeed { get; set; }
    }
}
