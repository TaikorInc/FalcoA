using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FalcoA.Core
{
    public enum DocumentType
    {
        PDF = 0,

        WORD,

        EXCEL,
    }

    public class PhaseParseFile : IPhase
    {
        public String FileUrlTagName { get; set; }

        public String ListID { get; set; }

        public String Binding { get; set; }

        public Boolean Save { get; set; }

        /// <summary>
        /// 待解析的文档的类型：
        /// PDF，WORD，EXCEL
        /// </summary>
        public DocumentType Type { get; set; }

        public String Directory { get; set; }

        public PhaseResult Run(Context context)
        {
            List<String> bind = null;
            if (!String.IsNullOrWhiteSpace(Binding))
            {
                if (context.JsonResults.ContainsKey(Binding))
                {
                    bind = context.JsonResults[Binding];
                }
                else
                {
                    bind = new List<string>();
                }
            }
            else
            {
                PhaseResult last = context.Stack.LastOrDefault();
                if (last != null)
                {
                    bind = last.ListResult;
                }
            }

            PhaseResult pr = new PhaseResult(this);
            pr.ListResult = new List<string>();
            pr.Succeed = true;

            if (bind == null)
            {
                pr.Succeed = false;
            }
            else
            {
                // 将Json中PDF的Url下载并替换成文字
                foreach (String json in bind)
                {
                    Dictionary<String, String> dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
                    if (dict.ContainsKey(FileUrlTagName))
                    {
                        String url = dict[FileUrlTagName];
                        String content = null;
                        switch (Type)
                        {
                            case DocumentType.PDF:
                                content = PdfParser.Extract(url, Directory);
                                if (String.IsNullOrWhiteSpace(content))
                                {
                                    content = PdfParser.Extract(url, Directory);
                                }
                                break;
                            case DocumentType.WORD:
                                content = WordParser.Extract(url, Directory);
                                if (String.IsNullOrWhiteSpace(content))
                                {
                                    content = WordParser.Extract(url, Directory);
                                }
                                break;
                            case DocumentType.EXCEL:
                                content = ExcelParser.Extract(url, Directory);
                                if (String.IsNullOrWhiteSpace(content))
                                {
                                    content = ExcelParser.Extract(url, Directory);
                                }
                                break;
                        }


                        dict[FileUrlTagName] = content.Replace(Environment.NewLine, String.Empty);
                        
                        pr.ListResult.Add(JsonConvert.SerializeObject(dict));
                    }
                }

                // 将结果保存
                if (Save)
                {
                    context.JsonResult.AddRange(pr.ListResult);
                }

                // 保存ListID等会可以在其他Phase中作为绑定对象
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
            }

            return pr;
        }

        public static PhaseParseFile Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (!parameters.Attributes.ContainsKey(Constant.FileUrlTagAttr) ||
                !parameters.Attributes.ContainsKey(Constant.DocumentTypeAttr))
            {
                return null;
            }

            PhaseParseFile file = new PhaseParseFile();

            file.Save = parameters.Attributes.ContainsKey(Constant.SaveAttr) ? Constant.True(parameters.Attributes[Constant.SaveAttr]) : false;
            file.Binding = parameters.Attributes.ContainsKey(Constant.ListBindingAttr) ? parameters.Attributes[Constant.ListBindingAttr] : null;
            file.ListID = parameters.Attributes.ContainsKey(Constant.ListIDAttr) ? parameters.Attributes[Constant.ListIDAttr] : null;
            file.FileUrlTagName = parameters.Attributes[Constant.FileUrlTagAttr];
            file.Type = ParseDocumentType(parameters.Attributes[Constant.DocumentTypeAttr]);
            file.Directory = parameters.Descends.ContainsKey(Constant.DirectoryNode) ? parameters.Descends[Constant.DirectoryNode].Value : null;

            return file;
        }

        public static DocumentType ParseDocumentType(String type)
        {
            switch (type)
            {
                case "PDF":
                    return DocumentType.PDF;
                case "EXCEL":
                    return DocumentType.EXCEL;
                case "WORD":
                    return DocumentType.WORD;
                default:
                    throw new NotSupportedException(type);
            }
        }
    }
}
