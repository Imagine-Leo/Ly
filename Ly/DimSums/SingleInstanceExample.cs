using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ly.Tools;

namespace Ly.Base
{
    public class TDebug : Ly.Base.SingleInstance<TDebug>
    {
    }

    public class SingleExcel : Ly.Base.SingleInstanceComponent<EasyExcel>
    {
        private new void Awake()
        {
            base.Awake();
        }
    }
}