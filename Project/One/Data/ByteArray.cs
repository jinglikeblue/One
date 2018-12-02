using System;
using System.Net;
using System.Text;

namespace One.Net
{
    /// <summary>
    /// 字节数组操作
    /// </summary>
    public class ByteArray
    {
        /// <summary>
        /// byte类型占用字节数
        /// </summary>
        public const byte BYTE_SIZE = 1;
        /// <summary>
        /// char类型占用字节数
        /// </summary>
        public const byte CHAR_SIZE = 2;
        /// <summary>
        /// float类型占用字节数
        /// </summary>
        public const byte FLOAT_SIZE = 4;
        /// <summary>
        /// short类型占用字节数
        /// </summary>
        public const byte SHORT_SIZE = 2;
        /// <summary>
        /// int类型占用字节数
        /// </summary>
        public const byte INT_SIZE = 4;
        /// <summary>
        /// long类型占用字节数
        /// </summary>
        public const byte LONG_SIZE = 8;      

        /// <summary>
        /// 默认使用的文本编码(全局生效）
        /// </summary>
        public static Encoding defaultEncoding = Encoding.UTF8;

        /// <summary>
        /// 默认的缓冲区大小(全局生效）
        /// </summary>
        public static int defaultBufferSize = 65535;

        /// <summary>
        /// 字节数组
        /// </summary>
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// 定义的字节数组大小
        /// </summary>
        public int Size
        {
            get
            {
                return Bytes.Length;
            }
        }

        /// <summary>
        /// 字节序是否是大端
        /// </summary>
        bool _isBigEndian;

        /// <summary>
        /// 是否需要转换字节序
        /// </summary>
        bool _isNeedConvertEndian = false;

        /// <summary>
        /// 字节数组操作位置
        /// </summary>
        int _pos = 0;

        /// <summary>
        /// 有效字节大小次奥
        /// </summary>
        int _available = 0;

        /// <summary>
        /// 目前有效字节大小
        /// </summary>
        public int Available
        {
            get { return _available; }
        }        

        /// <summary>
        /// 将数据转为字节数组导出
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Available];
            Array.Copy(Bytes, 0, bytes, 0, Available);
            return bytes;
        }

        public ByteArray(byte[] bytes, bool isBigEndian = true)
        {
            Init(bytes, isBigEndian);
        }

        public ByteArray(bool isBigEndian = true)
        {
            Init(new byte[defaultBufferSize], isBigEndian);
        }

        public ByteArray(int bufferSize, bool isBigEndian = true)
        {                       
            Init(new byte[Available], isBigEndian);            
        }

        void Init(byte[] bytes, bool isBigEndian = true)
        {
            _isBigEndian = isBigEndian;
            if(isBigEndian != BitConverter.IsLittleEndian)
            {
                _isNeedConvertEndian = true;
            }
            Bytes = bytes;
            SetPos(0);
        }

        /// <summary>
        /// 充值指针位置
        /// </summary>
        public void Reset()
        {
            SetPos(0);
        }

        /// <summary>
        /// 移动指针位置
        /// </summary>
        /// <param name="v">移动的偏移值</param>
        public void MovePos(int v)
        {            
            SetPos(_pos + v);
        }

        /// <summary>
        /// 设置指针位置
        /// </summary>
        /// <param name="v">指针的位置</param>
        public void SetPos(int v)
        {
            _pos = v;
        }


        #region write
        public void Write(short v)
        {
            if(_isNeedConvertEndian)
            {
                v = IPAddress.HostToNetworkOrder(v);
            }

            Write(BitConverter.GetBytes(v));
        }

        public void Write(int v)
        {
            if (_isNeedConvertEndian)
            {                
                v = IPAddress.HostToNetworkOrder(v);
            }

            Write(BitConverter.GetBytes(v));            
        }

        public void Write(long v)
        {
            if (_isNeedConvertEndian)
            {
                v = IPAddress.HostToNetworkOrder(v);
            }

            Write(BitConverter.GetBytes(v));
        }

        public void Write(float v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(char v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(string v)
        {
            Write(v, defaultEncoding);
        }

        public void Write(string v, Encoding encoding)
        {
            Write(encoding.GetBytes(v));
        }

        public void Write(byte v)
        {
            Bytes[_pos] = v;
            _available += 1;
            MovePos(1);
        }

        public void Write(byte[] sourceBytes)
        {
            Write(sourceBytes, 0, sourceBytes.Length);            
        }

        /// <summary>
        /// 将目标字节数组从sourceIndex指定的位置，读取length长度写入
        /// </summary>
        /// <param name="sourceBytes">读取的字节数组</param>
        /// <param name="sourceIndex">读取开始的位置</param>
        /// <param name="length">读取的长度</param>
        /// <returns></returns>
        public void Write(byte[] sourceBytes, int sourceIndex, int length)
        {
            Array.Copy(sourceBytes, sourceIndex, Bytes, _pos, length);
            _available += length;
            MovePos(length);
        }

        #endregion


        #region read
        public short ReadShort()
        {
            short v = BitConverter.ToInt16(Bytes, _pos);
            if (_isNeedConvertEndian)
            {
                v = IPAddress.NetworkToHostOrder(v);
            }
            MovePos(SHORT_SIZE);
            return v;
        }

        public int ReadInt()
        {
            int v = BitConverter.ToInt32(Bytes, _pos);
            if (_isNeedConvertEndian)
            {
                v = IPAddress.NetworkToHostOrder(v);
            }
            MovePos(INT_SIZE);
            return v;
        }

        public long ReadLong()
        {
            long v = BitConverter.ToInt64(Bytes, _pos);
            if (_isNeedConvertEndian)
            {
                v = IPAddress.NetworkToHostOrder(v);
            }
            MovePos(LONG_SIZE);
            return v;
        }

        public float ReadFloat()
        {            
            float v = BitConverter.ToSingle(Bytes, _pos);
            MovePos(FLOAT_SIZE);
            return v;
        }

        public char ReadChar()
        {
            char v = BitConverter.ToChar(Bytes, _pos);
            MovePos(CHAR_SIZE);
            return v;
        }

        public string ReadString(int length)
        {
            return ReadString(defaultEncoding, length);
        }

        public string ReadString(Encoding encoding, int length)
        {
            string v = encoding.GetString(Bytes, _pos, length);
            MovePos(length);
            return v;
        }

        public byte ReadByte()
        {
            byte v = Bytes[_pos];
            MovePos(BYTE_SIZE);
            return v;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(Bytes, _pos, bytes, 0, length);
            MovePos(length);
            return bytes;
        }
        #endregion
    }
}