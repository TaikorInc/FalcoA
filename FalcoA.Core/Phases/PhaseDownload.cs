using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FalcoA.Core
{
    public class PhaseDownload : IPhase
    {
        public String UrlTagName { get; set; }

        public String Binding { get; set; }

        public String Directory { get; set; }

        public String Extension { get; set; }

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
                Int32 downloadSucc = 0;
                foreach (String json in bind)
                {
                    Dictionary<String, String> dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
                    Boolean succ = false;
                    if (dict.ContainsKey(UrlTagName))
                    {
                        String url = dict[UrlTagName];

                        succ = DownloadHelper.DownloadFile(url, Directory);
                        if (!succ)
                        {
                            succ = DownloadHelper.DownloadFile(url, Directory);
                        }
                    }

                    downloadSucc += succ ? 1 : 0;
                }
                pr.SetInt("download", downloadSucc);
                pr.SetInt("total", bind.Count);
            }

            return pr;
        }

        public static PhaseDownload Create(TreeNode parameters, Boolean useBrowser = false)
        {
            if (!parameters.Attributes.ContainsKey(Constant.FileUrlTagAttr))
            {
                return null;
            }

            PhaseDownload pdf = new PhaseDownload();

            pdf.Binding = parameters.Attributes.ContainsKey(Constant.ListBindingAttr) ? parameters.Attributes[Constant.ListBindingAttr] : null;
            pdf.Extension = parameters.Attributes.ContainsKey(Constant.FileExtensionAttr) ? parameters.Attributes[Constant.FileExtensionAttr] : null;
            pdf.UrlTagName = parameters.Attributes[Constant.UrlTagAttr];
            pdf.Directory = parameters.Descends.ContainsKey(Constant.DirectoryNode) ? parameters.Descends[Constant.DirectoryNode].Value : null;

            return pdf;
        }
    }
}
