using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FalcoA.Core
{
    public class BasicDataProvider : IDataProvider
    {
        private Dictionary<String, String> _stringsInternal;

        private Dictionary<String, Int32> _intsInternal;

        private Dictionary<String, String> _strings
        {
            get
            {
                if (_stringsInternal == null)
                {
                    lock (_lockString)
                    {
                        if (_stringsInternal == null)
                        {
                            _stringsInternal = new Dictionary<String, String>();
                        }
                    }
                }
                return _stringsInternal;
            }
        }

        private Dictionary<String, Int32> _ints
        {
            get
            {
                if (_intsInternal == null)
                {
                    lock (_lockString)
                    {
                        if (_intsInternal == null)
                        {
                            _intsInternal = new Dictionary<String, Int32>();
                        }
                    }
                }
                return _intsInternal;
            }
        }

        private readonly Object _lockInt = new Object();

        private readonly Object _lockString = new Object();

        /// <summary>
        /// 读取一个整数
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="System.ArgumentNullException">name为null</exception>
        /// <exception cref="System.IndexOutOfRangeException">name变量不存在</exception>
        /// <returns></returns>
        public int GetInt(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            lock (_lockInt)
            {
                if (_ints.ContainsKey(name))
                {
                    return _ints[name];
                }
                else
                {
                    throw new IndexOutOfRangeException(String.Format("不存在名为{0}的整形数", name));
                }
            }
        }

        public void SetInt(string name, int val)
        {
            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            lock (_lockInt)
            {
                _ints[name] = val;
            }
        }

        /// <summary>
        /// 读取一个字符串
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="System.ArgumentNullException">name为null或空白字符</exception>
        /// <exception cref="System.IndexOutOfRangeException">name变量不存在</exception>
        /// <returns></returns>
        public string GetString(string name)
        {
            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            lock (_lockString)
            {
                if (_strings.ContainsKey(name))
                {
                    return _strings[name];
                }
                else
                {
                    throw new IndexOutOfRangeException(String.Format("不存在名为{0}的字符串", name));
                }
            }
        }

        public void SetString(string name, string val)
        {
            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            lock (_lockString)
            {
                _strings[name] = val;
            }
        }

        public static BasicDataProvider CreateFromXml(String xml)
        {
            BasicDataProvider provider = new BasicDataProvider();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeList list = doc.SelectNodes(String.Format("/{0}/*", Constant.ParameterProviderNode));

            foreach (XmlNode node in list)
            {
                String name = node.Name;
                String val = node.InnerText.Trim();

                Int32 valInt;
                if (Int32.TryParse(val, out valInt))
                {
                    provider.SetInt(name, valInt);
                }
                else
                {
                    provider.SetString(name, val);
                }
            }

            return provider;
        }

        public static BasicDataProvider CreateFromFile(String uri)
        {
            using (StreamReader sr = new StreamReader(uri))
            {
                return CreateFromXml(sr.ReadToEnd());
            }
        }

        public static BasicDataProvider CreateFromJson(String json)
        {
            BasicDataProvider provider = new BasicDataProvider();
            Dictionary<String, String> dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
            foreach (var pair in dict)
            {
                provider.SetString(pair.Key, pair.Value);
            }

            return provider;
        }
    }
}
