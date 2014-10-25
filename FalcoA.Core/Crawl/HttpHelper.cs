using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace FalcoA.Core
{
    public class HttpHelper
    {
        public const string CharsetReg = @"(meta.*?charset=""?(?<Charset>[^\s""'>]+)""?)|(xml.*?encoding=""?(?<Charset>[^\s"">]+)""?)";

        /// <summary>
        /// 获取网页的内容
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="postData">Post的信息</param>
        /// <returns></returns>
        public static string GetHttpContent(string url, string postData = null)
        {
            try
            {
                HttpWebResponse httpRequest = null;
                if (string.IsNullOrWhiteSpace(postData))
                    httpRequest = CreatePostHttpResponse(url, postData);
                else
                    httpRequest = CreateGetHttpResponse(url);

                #region 根据Html头判断
                Encoding Encode = null;
                string Content = null;
                //缓冲区长度
                const int N_CacheLength = 10000;
                //头部预读取缓冲区，字节形式
                var bytes = new List<byte>();
                int count = 0;
                //头部预读取缓冲区，字符串
                String cache = string.Empty;

                //创建流对象并解码
                Stream ResponseStream;
                switch (httpRequest.ContentEncoding.ToUpperInvariant())
                {
                    case "GZIP":
                        ResponseStream = new GZipStream(
                            httpRequest.GetResponseStream(), CompressionMode.Decompress);
                        break;
                    case "DEFLATE":
                        ResponseStream = new DeflateStream(
                            httpRequest.GetResponseStream(), CompressionMode.Decompress);
                        break;

                    default:
                        ResponseStream = httpRequest.GetResponseStream();
                        break;
                }

                try
                {
                    while (
                        !(cache.EndsWith("</head>", StringComparison.OrdinalIgnoreCase)
                          || count >= N_CacheLength))
                    {
                        var b = (byte)ResponseStream.ReadByte();
                        if (b < 0) //end of stream
                        {
                            break;
                        }
                        bytes.Add(b);

                        count++;
                        cache += (char)b;
                    }

                    try
                    {

                        if (httpRequest.CharacterSet == "ISO-8859-1" || httpRequest.CharacterSet == "zh-cn")
                        {
                            Match match = Regex.Match(
                                              cache, CharsetReg,
                                              RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            if (match.Success)
                            {
                                try
                                {
                                    string charset = match.Groups["Charset"].Value;
                                    Encode = System.Text.Encoding.GetEncoding(charset);
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                Encode = System.Text.Encoding.GetEncoding("GB2312");
                            }

                        }
                        else
                        {
                            Encode = System.Text.Encoding.GetEncoding(httpRequest.CharacterSet);
                        }
                    }
                    catch
                    {
                    }

                    //缓冲字节重新编码，然后再把流读完
                    var Reader = new StreamReader(ResponseStream, Encode);
                    Content = Encode.GetString(bytes.ToArray(), 0, count) + Reader.ReadToEnd();
                    Reader.Close();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    httpRequest.Close();
                }
                #endregion 根据Html头判断

                return Content;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreateGetHttpResponse(string url, int timeout = 60000, string userAgent = "", CookieCollection cookies = null)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";

            //设置代理UserAgent和超时
            if (string.IsNullOrWhiteSpace(userAgent))
                userAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.116 Safari/537.36";

            request.UserAgent = userAgent;
            request.Timeout = timeout;
            request.KeepAlive = false;
            request.AllowAutoRedirect = true;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreatePostHttpResponse(string url, string postData, int timeout = 60000, string userAgent = "", CookieCollection cookies = null)
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            if (string.IsNullOrWhiteSpace(userAgent))
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";
            else
                request.UserAgent = userAgent;
            request.Timeout = timeout;
            request.KeepAlive = false;
            request.AllowAutoRedirect = true;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据  
            if (!string.IsNullOrWhiteSpace(postData))
            {
                byte[] data = Encoding.ASCII.GetBytes(postData);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}
