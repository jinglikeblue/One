using One;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    class CheckCloseCommand : BaseCommand, IMainLoopLogic
    {
        public override void Excute()
        {
            if (Global.Ins.core.isExit)
            {
                OneLog.I("退出程序");
            }
        }

        public override void Terminate()
        {
            
        }
    }
}
