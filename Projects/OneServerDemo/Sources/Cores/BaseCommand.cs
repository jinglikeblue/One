using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    abstract class BaseCommand
    {
        public abstract void Excute();
        public abstract void Terminate();
    }
}
