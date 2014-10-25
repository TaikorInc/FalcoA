using System;
using System.IO;
using System.Net;

namespace FalcoA.Core
{
    public class DownloadHelper
    {
        private static String GeneratePath(String dir, String url, String forceExt = null)
        {
            if (String.IsNullOrWhiteSpace(dir) || String.IsNullOrWhiteSpace(url))
            {
                return String.Empty;
            }

            String file = url.Substring(url.LastIndexOf('/'));
            String bare = Path.GetFileNameWithoutExtension(file);
            String extension = Path.GetExtension(file);

            file = String.Format("{0}-{1}.{2}", bare, DateTime.Now.ToFileTime(), forceExt ?? extension);
            return Path.Combine(dir, file);
        }

        public static Boolean DownloadFile(String url, String saveTo, String extension = null)
        {
            WebClient wc = new WebClient();
            String path = GeneratePath(saveTo, url, extension);

            try
            {
                wc.DownloadFile(url, saveTo);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
