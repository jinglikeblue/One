using System.Text;

namespace One.WebSocket
{
    public class MessageUtility
    {
        /// <summary>
        /// 将数据转换成byte数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] TransformData(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// 将byte数组转换成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TransformData(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
