using System;
using System.Collections.Generic;

namespace FalcoA.Core
{
    public class PhaseResult
    {
        public Boolean Succeed { get; set; }

        public String PhaseName { get; set; }

        private Dictionary<String, String> _stringsInternal;

        private Dictionary<String, Int32> _intsInternal;

        private Dictionary<String, Boolean> _boolsInternal;

        private readonly Object _lockInt = new Object();

        private readonly Object _lockString = new Object();

        private readonly Object _lockBoolean = new Object();

        public List<String> ListResult { get; set; }

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

        private Dictionary<String, Boolean> _bools
        {
            get
            {
                if (_boolsInternal == null)
                {
                    lock (_lockBoolean)
                    {
                        if (_boolsInternal == null)
                        {
                            _boolsInternal = new Dictionary<String, Boolean>();
                        }
                    }
                }
                return _boolsInternal;
            }
        }

        public PhaseResult(IPhase phase)
        {
            PhaseName = phase.GetType().AssemblyQualifiedName;
        }

        #region Get

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

        public Boolean GetBoolean(String name)
        {
            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            lock (_lockBoolean)
            {
                if (_bools.ContainsKey(name))
                {
                    return _bools[name];
                }
                else
                {
                    throw new IndexOutOfRangeException(String.Format("不存在名为{0}的布尔型变量", name));
                }
            }
        }

        #endregion

        #region Set

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

        public void SetBoolean(String name, Boolean val)
        {
            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            lock (_lockBoolean)
            {
                _bools[name] = val;
            }
        }

        #endregion
    }
}
