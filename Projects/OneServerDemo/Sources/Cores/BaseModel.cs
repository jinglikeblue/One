using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    abstract class BaseModel
    {
        public BaseModel()
        {
            OnInit();
        }

        protected abstract void OnInit();
        protected abstract void OnDestroy();
    }
}
