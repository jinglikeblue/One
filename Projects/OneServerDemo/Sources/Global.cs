using Jing;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    class Global:ASingleton<Global>
    {
        public readonly CoreModel core = new CoreModel();

        public readonly RoomModel room = new RoomModel();

        public readonly RoleModel roles = new RoleModel();

        protected override void Init()
        {
        
        }

        public override void Destroy()
        {
        
        }        
    }
}
