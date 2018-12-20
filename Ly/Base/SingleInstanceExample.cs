using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ly.Base
{

    public class TDebug : Ly.Base.SingleInstance<TDebug>
    {

    }
    public class SingleExcel : Ly.Base.SingleInstanceComponent<Ly.Excel.EasyExcel>
    {
        private void Awake()
        {
            base.Awake();
        }
    }

}
