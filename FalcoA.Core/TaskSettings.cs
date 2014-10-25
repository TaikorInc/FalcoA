using Palas.Common.Lib.Entity;
using System;

namespace FalcoA.Core
{
    /// <summary>
    /// 一次任务运行的设置
    /// </summary>
    public class TaskSettings
    {
        /// <summary>
        /// 抓取任务对应的Crawl
        /// </summary>
        public CrawlEntity Crawl { get; set; }

        /// <summary>
        /// 出错是否把错误信息发布到全局消息服务上
        /// </summary>
        public Boolean ReportOnError { get; set; }

        /// <summary>
        /// 出错后重试任务的次数
        /// </summary>
        public Int32 RetryOnError { get; set; }
    }
}
