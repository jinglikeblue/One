using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressTesting.Sources
{
    class StressTestingVO
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public string host;

        /// <summary>
        /// 主机端口号 
        /// </summary>
        public int port;

        /// <summary>
        /// 线程数量
        /// </summary>
        public int threadCount;

        /// <summary>
        /// 每线程构建的链接数量
        /// </summary>
        public int connectionCountPerThread;

        /// <summary>
        /// 消息发送的内容
        /// </summary>
        public byte[] msgData;
    }
}
