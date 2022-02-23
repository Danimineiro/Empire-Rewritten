using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten.Borders
{
    public static class BorderUtils
    {
        public static bool enabled = true;
        public static float BorderAlpha
        {
            get
            {
                return enabled ?  0.8f : 0;
            }
        }
    }
}
