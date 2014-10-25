using System;

namespace FalcoA.Core.Contract
{
    public class CorePhasesConstants
    {
        public String[] CorePhases =
        {
            "WaitForUrl",
            "WaitForContent",
            "ParseContent",
            "ParseUrl",
            "WriteString",
            "ReadString",
            "WriteImage",
            "ReadImage",
            "Locate",
            "DOMOperation",
        };
    }
}
