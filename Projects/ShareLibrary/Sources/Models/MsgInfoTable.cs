using System;
using System.Collections.Generic;
using System.Text;

namespace Share
{
    /// <summary>
    /// 协议信息表
    /// </summary>
    public class MsgInfoTable
    {
        Dictionary<int, MsgInfoVO> _id2InfoDic = new Dictionary<int, MsgInfoVO>();
        Dictionary<Type, MsgInfoVO> _dataType2InfoDic = new Dictionary<Type, MsgInfoVO>();

        public void AddMsgInfo(MsgInfoVO vo)
        {
            _id2InfoDic[vo.id] = vo;
            _dataType2InfoDic[vo.dataType] = vo;
        }

        public MsgInfoVO GetMsgInfo(int id)
        {
            MsgInfoVO value;
            _id2InfoDic.TryGetValue(id, out value);
            return value;
        }

        public MsgInfoVO GetMsgInfo<TDataType>()
        {
            return GetMsgInfo(typeof(TDataType));
        }

        public MsgInfoVO GetMsgInfo(Type dataType)
        {
            MsgInfoVO value;
            _dataType2InfoDic.TryGetValue(dataType, out value);
            return value;
        }
    }
}
