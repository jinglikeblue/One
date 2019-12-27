using Jing;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    class Global:ASingleton<Global>
    {
        public readonly CoreModel core = new CoreModel();

        protected override void Init()
        {
        
        }

        public override void Destroy()
        {
        
        }        
    }
}
