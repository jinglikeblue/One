using One.Net;
using System.Collections.Generic;

namespace OneDemo.Managers
{
    class UserMgr
    {
        public static UserMgr Ins { get; private set; } = new UserMgr();

        HashSet<User> _userSet = new HashSet<User>();
        Dictionary<TcpClient, User> _c2uDic = new Dictionary<TcpClient, User>();        

        public void Enter(TcpClient client)
        {
            User user = new User(client);
            _c2uDic[client] = user;
            _userSet.Add(user);            
        }

        public void Exit(TcpClient client)
        {
            if(_c2uDic.ContainsKey(client))
            {
                _c2uDic[client].MarkDestroy();
            }            
        }

        public void Update()
        {
            HashSet<User> destroySet = new HashSet<User>();

            foreach (var user in _userSet)
            {
                if(false == user.Update())
                {
                    destroySet.Add(user);
                }
            }

            //清除要删除的对象
            foreach(var user in destroySet)
            {
                _userSet.Remove(user);
                _c2uDic.Remove(user.client);
            }
        }
    }
}
