using System;
using System.Collections.Generic;

namespace FalcoA.Core
{
    public abstract class Context : IServiceProvider
    {
        /// <summary>
        /// 最终的结果，用Json格式存储
        /// </summary>
        public List<String> JsonResult { get; set; }

        /// <summary>
        /// 流程堆栈
        /// </summary>
        public List<PhaseResult> Stack { get; set; }

        /// <summary>
        /// 最近一次http请求返回的结果
        /// </summary>
        public String LastRequestContent { get; set; }

        /// <summary>
        /// 创建时提供的静态的参数
        /// </summary>
        public IDataProvider ParameterProvider { get; set; }

        /// <summary>
        /// 运行时提供的临时的动态的参数
        /// </summary>
        public Stack<IDataProvider> RuntimeProviders { get; set; }

        /// <summary>
        /// 含有ListID的Phase返回的列表结果会按ListID存储在JsonResults中
        /// </summary>
        public Dictionary<String, List<String>> JsonResults { get; set; }

        /// <summary>
        /// 登录使用的账号
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// 获取服务，目前应用在获取Gecko浏览器实例
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public abstract object GetService(Type serviceType);

        /// <summary>
        /// 将一次流程的结果放入堆栈
        /// </summary>
        /// <param name="result"></param>
        public void PushResult(PhaseResult result)
        {
            Stack.Add(result);
        }

        public Context()
        {
            Stack = new List<PhaseResult>();
            JsonResult = new List<string>();
            RuntimeProviders = new Stack<IDataProvider>();
            JsonResults = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// 运行时解析模板中的数据绑定
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public String Resolve(String raw)
        {
            return ParameterResolver.Resolve(raw, this);
        }
    }
}
