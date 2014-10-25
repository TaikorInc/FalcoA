using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FalcoA.Core
{
    /// <summary>
    /// ParseContent和ParseUrl不同，后者要求只有一个Match。
    /// 而ParseContent可以有多个Match。Match的结果作为多个JSON字符串保存
    /// </summary>
    public class PhaseParseContent : PhaseParseContentBase
    {
        private String _regex;

        private String _nested;

        private String _baseXPath;

        private Dictionary<String, String> _xpaths;

        private Dictionary<String, DomElementLocator> _images;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Succeed表示是否成功，ListResult表示抓到的Json数据</returns>
        public override PhaseResult Run(Context context)
        {
            Initialize(context);

            if (String.IsNullOrWhiteSpace(context.LastRequestContent))
            {
            }

            String content = context.LastRequestContent;

            List<String> jsonResult = new List<String>();

            //=======================================================================
            // 用正则表达式
            jsonResult.AddRange(ParseByRegex(content));

            //=======================================================================
            // 用XPath抓取
            jsonResult.AddRange(ParseByXPath(content));

            PhaseResult pr = new PhaseResult(this);
            pr.Succeed = true;
            pr.ListResult = jsonResult;

            if (Save)
            {
                context.JsonResult.AddRange(jsonResult);
            }

            if (!String.IsNullOrWhiteSpace(ListID))
            {
                if (context.JsonResults.ContainsKey(ListID))
                {
                    context.JsonResults[ListID] = pr.ListResult;
                }
                else
                {
                    context.JsonResults.Add(ListID, pr.ListResult);
                }
            }

            return pr;
        }

        private List<String> ParseByXPath(String content)
        {
            List<String> result = new List<String>();

            if (_xpaths != null && !String.IsNullOrWhiteSpace(_baseXPath))
            {
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(content);

                // 先获取所有RecordLevel的数据
                var records = hd.DocumentNode.SelectNodes(_baseXPath);

                // 对每一个record，使用相对的XPath来定位字段
                foreach (HtmlNode node in records)
                {
                    Dictionary<String, String> item = new Dictionary<string, string>();
                    foreach (KeyValuePair<String, String> xpath in _xpaths)
                    {
                        if (String.Equals(xpath.Key, Constant.NestedNode, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var part = node.SelectSingleNode(xpath.Value);
                            if (null != part)
                            {
                                String innerHtml = part.InnerHtml.Trim();
                                result.AddRange(ParseNestedItem(innerHtml, _nested));
                            }
                        }
                        else
                        {
                            var part = node.SelectSingleNode(xpath.Value);
                            if (null != part)
                            {
                                item.Add(xpath.Key, part.InnerText.Trim());
                            }
                        }
                    }
                    result.Add(JsonConvert.SerializeObject(item));
                }
            }

            return result;
        }

        private List<String> ParseByRegex(String content)
        {
            List<String> result = new List<string>();

            if (!String.IsNullOrWhiteSpace(_regex))
            {
                MatchCollection mc = Regex.Matches(content, _regex);
                if (mc.Count > 0)
                {
                    List<String> ids = RegexHelper.ParseGroupIndexNames(_regex);
                    foreach (Match m in mc)
                    {
                        // 单个item
                        Dictionary<String, String> item = new Dictionary<string, string>();
                        foreach (String id in ids)
                        {
                            if (String.Equals(id, Constant.NestedNode, StringComparison.CurrentCultureIgnoreCase))
                            {
                                var nests = ParseNestedItem(m.Groups[id].Value, _nested);
                                result.AddRange(nests);
                            }
                            else
                            {
                                item[id] = m.Groups[id].Value;
                            }
                        }

                        // 把解析结果加入到JsonResult
                        result.Add(JsonConvert.SerializeObject(item));
                    }
                }
            }

            return result;
        }

        // TODO::多层嵌套的Item
        /// <summary>
        /// 处理嵌套的Item，一个Item可以嵌套多个Item。但是目前只支持嵌套一层
        /// </summary>
        /// <param name="nested"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private List<String> ParseNestedItem(String nested, String pattern)
        {
            List<String> result = new List<string>();

            MatchCollection mc = Regex.Matches(nested, pattern);
            if (mc.Count > 0)
            {
                List<String> ids = RegexHelper.ParseGroupIndexNames(pattern);
                foreach (Match m in mc)
                {
                    Dictionary<String, String> item = new Dictionary<string, string>();
                    foreach (String id in ids)
                    {
                        item.Add(id, m.Groups[id].Value);
                    }
                    result.Add(JsonConvert.SerializeObject(item));
                }
            }

            return result;
        }

        /// <summary>
        /// 截取xpath的公共部分
        /// </summary>
        /// <param name="xpaths"></param>
        /// <returns></returns>
        private String TrimCommonXPath(List<String> xpaths)
        {
            Int32 seq = 0;

            return null;
        }

        private void Initialize(Context context)
        {
            _regex = context.Resolve(RegularExpression);
            _nested = context.Resolve(NestedRegularExpression);
            _baseXPath = context.Resolve(BaseXPath);

            if (XPaths != null && XPaths.Count > 0)
            {
                _xpaths = new Dictionary<string, string>();
                foreach (var pair in XPaths)
                {
                    _xpaths.Add(pair.Key, context.Resolve(pair.Value));
                }
            }

            if (Images != null && Images.Count > 0)
            {
                _images = new Dictionary<String, DomElementLocator>();
                foreach (var pair in Images)
                {
                    pair.Value.Locator = context.Resolve(pair.Value.Locator);
                    _images.Add(pair.Key, pair.Value);
                }
            }
        }

        public static PhaseParseContent Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (parameters.Descends == null)
            {
                return null;
            }

            try
            {
                PhaseParseContent parseContent = new PhaseParseContent();

                parseContent.Save = false;
                if (parameters.Attributes.ContainsKey(Constant.SaveAttr))
                {
                    String save = parameters.Attributes[Constant.SaveAttr];
                    parseContent.Save = Constant.True(save);
                }

                parseContent.ListID = parameters.Attributes.ContainsKey(Constant.ListIDAttr) ? parameters.Attributes[Constant.ListIDAttr] : null;

                if (parameters.Descends.ContainsKey(Constant.NestedRegexNode))
                {
                    parseContent.NestedRegularExpression = parameters.Descends[Constant.NestedRegexNode].Value;
                }

                if (parameters.Descends.ContainsKey(Constant.RegexNode))
                {
                    parseContent.RegularExpression = parameters.Descends[Constant.RegexNode].Value;
                }
                else
                {
                    parseContent.BaseXPath = parameters.Descends[Constant.BaseXPathNode].Value;
                    TreeNode xpaths = parameters.Descends[Constant.XPathNode];
                    if (xpaths == null)
                    {
                        return null;
                    }
                    else
                    {
                        parseContent.XPaths = new Dictionary<string, string>();
                        foreach (var pair in xpaths.Descends)
                        {
                            parseContent.XPaths.Add(pair.Key, pair.Value.Value);
                        }
                    }
                }

                return parseContent;
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }
    }

    public abstract class PhaseParseContentBase : IPhase
    {
        //Regex[String]
        //XPath[JSON]
        //NestedRegex[String]
        //Image[String/JSON]

        public String RegularExpression { get; set; }

        public String NestedRegularExpression { get; set; }

        public String BaseXPath { get; set; }

        public String ListID { get; set; }

        /// <summary>
        /// [name, xpath] pair，相对路径
        /// </summary>
        public Dictionary<String, String> XPaths { get; set; }

        /// <summary>
        /// [name, locator] pair
        /// </summary>
        public Dictionary<String, DomElementLocator> Images { get; set; }

        public Boolean Save { get; set; }

        public abstract PhaseResult Run(Context context);
    }
}
