using Jing;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneClient
{
    class Global:ASingleton<Global>
    {
        public readonly NetModel net = new NetModel();

        protected override void Init()
        {
        
        }

        public override void Destroy()
        {
        
        }        
    }
}
