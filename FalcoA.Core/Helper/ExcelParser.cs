using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace FalcoA.Core
{
    public class ExcelParser
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

                try
                {
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        IWorkbook workbook = null;
                        try
                        {
                            // 尝试创建xlsx格式
                            workbook = new XSSFWorkbook(ms);
                        }
                        catch (Exception)
                        {
                            // 尝试创建xls格式
                            ms.Seek(0, SeekOrigin.Begin);
                            workbook = new HSSFWorkbook(ms);
                        }

                        Int32 nrSheets = workbook.NumberOfSheets;
                        StringBuilder wholeDocument = new StringBuilder();
                        for (int i = 0; i < nrSheets; i++)
                        {
                            ISheet sheet = workbook.GetSheetAt(i);
                            foreach (IRow row in sheet)
                            {
                                foreach (ICell cell in row)
                                {
                                    wholeDocument.Append(cell.ToString());
                                }
                            }
                        }

                        return wholeDocument.ToString();
                    }
                }
                catch (Exception e)
                {
                    return String.Empty;
                }
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
