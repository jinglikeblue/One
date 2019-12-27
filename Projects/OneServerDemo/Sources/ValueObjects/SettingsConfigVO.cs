using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    /// <summary>
    /// 配置数据对象
    /// </summary>
    class SettingsConfigVO
    {
        /// <summary>
        /// 监听的主机地址
        /// </summary>
        public string host = "0.0.0.0";

        /// <summary>
        /// 监听端口
        /// </summary>
        public int port = 1875;

        /// <summary>
        /// 主逻辑调用间隔
        /// </summary>
        public int mainLogicLoopIntervalMS = 25;

        /// <summary>
        /// 是否允许日志输出，1为true
        /// </summary>
        public int logOutputEnable = 0;

        /// <summary>
        /// 日志文件输出目录
        /// </summary>
        public string logOutputDir = null;

        /// <summary>
        /// 日志保留天数
        /// </summary>
        public int logKeepDays = 7;
    }
}
