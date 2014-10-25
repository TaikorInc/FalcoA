using NPOI.XWPF.UserModel;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace FalcoA.Core
{
    public class WordParser
    {
        private static String GeneratePath(String dir, String url)
        {
            if (String.IsNullOrWhiteSpace(dir) || String.IsNullOrWhiteSpace(url))
            {
                return String.Empty;
            }

            String file = url.Substring(url.LastIndexOf('/'));
            String bare = Path.GetFileNameWithoutExtension(file);
            String extension = Path.GetExtension(file);

            file = String.Format("{0}-{1}.{2}", bare, DateTime.Now.ToFileTime(), extension);
            return Path.Combine(dir, file);
        }

        public static String Extract(String url, String saveTo = null)
        {
            try
            {
                WebClient wc = new WebClient();
                byte[] data = wc.DownloadData(url);

                // 如果设置了保存到文件，则写入文件
                if (!String.IsNullOrWhiteSpace(saveTo) && data != null && data.Length > 0)
                {
                    if (!Directory.Exists(saveTo))
                    {
                        Directory.CreateDirectory(saveTo);
                    }
                    String file = GeneratePath(saveTo, url);
                    try
                    {
                        using (FileStream fs = new FileStream(file, FileMode.CreateNew))
                        {
                            fs.Write(data, 0, data.Length);
                        }
                    }
                    catch (IOException ioe)
                    {
                    }
                }

                using (MemoryStream ms = new MemoryStream(data))
                {
                    XWPFDocument doc = new XWPFDocument(ms);

                    StringBuilder wholeDocument = new StringBuilder();

                    foreach (XWPFParagraph para in doc.Paragraphs)
                    {
                        String text = para.Text;
                        wholeDocument.Append(text);
                    }

                    foreach (XWPFTable table in doc.Tables)
                    {
                        String text = table.Text;
                        if (!String.IsNullOrWhiteSpace(text))
                        {
                            text = text.Replace("\t", String.Empty);
                        }
                        wholeDocument.Append(text);
                    }

                    return wholeDocument.ToString();
                }
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
