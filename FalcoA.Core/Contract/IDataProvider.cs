using System;

namespace FalcoA.Core
{
    /// <summary>
    /// ParameterProvider类用来与Template在运行爬虫任务的时候进行数据的交互。
    /// 主要提供字符串、数字、验证码图片的读写。
    /// Template
    /// </summary>
    public interface IDataProvider
    {
        Int32 GetInt(String name);

        void SetInt(String name, Int32 val);

        String GetString(String name);

        void SetString(String name, String val);
    }
}
