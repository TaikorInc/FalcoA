using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FalcoA.Core
{
    /// <summary>
    /// 将一个包含数据绑定的字符串根据Context来解析出最终的结果
    /// 静态数据绑定: @(...)
    /// 动态数据绑定: #(...)
    /// </summary>
    public class ParameterResolver
    {
        /// <summary>
        /// 先解析动态数据绑定，再解析静态数据绑定
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static String Resolve(String raw, Context context)
        {
            if (raw == Constant.ParameterPrefix)
            {
                return context.Account.UserName;
            }
            else if (raw == Constant.DOMElementPrefix)
            {
                return context.Account.Password;
            }

            raw = ResolveString(raw, Constant.ParameterPrefix, context.ParameterProvider);
            foreach (IDataProvider provider in context.RuntimeProviders)
            {
                raw = ResolveString(raw, Constant.RuntimePrefix, provider);
            }
            raw = ResolveString(raw, Constant.ParameterPrefix, context.ParameterProvider);

            return raw;
        }

        /// <summary>
        /// 根据一个JSON来生成一个DataProvider
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IDataProvider ParseProvider(String json)
        {
            Dictionary<String, String> dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);

            BasicDataProvider provider = new BasicDataProvider();

            if (dict != null && dict.Count > 0)
            {
                foreach (var pair in dict)
                {
                    provider.SetString(pair.Key, pair.Value);
                }
            }

            return provider;
        }

        private static String _extractor = @"\((?<name>[^\)]*)\)";

        public static String ResolveString(String raw, String delimiter, IDataProvider provider)
        {
            if (raw == null || provider == null)
            {
                return raw;
            }

            String pattern = String.Format("{0}{1}", delimiter, _extractor);

            MatchCollection mc = Regex.Matches(raw, pattern);
            HashSet<String> collection = new HashSet<string>();
            foreach (Match m in mc)
            {
                String name = m.Groups["name"].Value;
                if (!collection.Contains(name))
                {
                    collection.Add(name);
                }
            }

            foreach (String name in collection)
            {
                String val = provider.GetString(name);
                if (!String.IsNullOrWhiteSpace(val))
                {
                    String target = String.Format("{0}({1})", delimiter, name);
                    raw = raw.Replace(target, val);
                }
            }

            return raw;
        }

        public static Boolean HasDataBinding(String raw, String delimiter)
        {
            String pattern = String.Format("{0}{1}", delimiter, _extractor);
            return Regex.IsMatch(raw, pattern);
        }
    }
}
