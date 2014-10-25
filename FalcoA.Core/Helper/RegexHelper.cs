using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FalcoA.Core
{
    public class RegexHelper
    {
        public static List<String> ParseGroupIndexNames(String patternStr)
        {
            MatchCollection mc = Regex.Matches(patternStr, @"\(\?<(?<id>[^>]*)>");
            List<String> ids = new List<string>();

            foreach (Match match in mc)
            {
                ids.Add(match.Groups["id"].Value);
            }

            return ids;
        }
    }
}
