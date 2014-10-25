using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace FalcoA.Core
{
    public class PdfParser
    {
        private static String GeneratePath(String dir, String url)
        {
            if (String.IsNullOrWhiteSpace(dir) || String.IsNullOrWhiteSpace(url))
            {
                return String.Empty;
            }

            String file = url.Substring(url.LastIndexOf('/') );
            if(string.IsNullOrEmpty(file))
            {
                file = "a";
            }
            else { file = file.Substring(1); }
            String bare = file.IndexOf('.') > 0 ? file.Substring(0, file.IndexOf('.')) : file;

            file = String.Format("{0}-{1}.pdf", bare, DateTime.Now.ToFileTime());
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

                using (PdfReader reader = new PdfReader(data))
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        ITextExtractionStrategy extract = new SimpleTextExtractionStrategy();
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            sb.Append(PdfTextExtractor.GetTextFromPage(reader, i, extract));
                            reader.ReleasePage(i);
                        }                                            
                        return sb.ToString();
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
