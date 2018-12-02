using One.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneDemo.Managers
{
    class UserMgr
    {
        public static UserMgr Ins { get; private set; } = new UserMgr();

        HashSet<User> _userSet = new HashSet<User>();
        Dictionary<Client, User> _c2uDic = new Dictionary<Client, User>();

        public void Enter(Client client)
        {
            User user = new User(client);
            _c2uDic[client] = user;
            _userSet.Add(user);            
        }

        public void Exit(Client client)
        {
            if(_c2uDic.ContainsKey(client))
            {
                _c2uDic[client].MarkDestroy();
            }            
        }

        public void RemoveUser(User user)
        {
            _userSet.Remove(user);
            _c2uDic.Remove(user.client);            
        }

        public void Update()
        {
            foreach(var user in _userSet)
            {
                user.Update();
            }
        }
    }
}
