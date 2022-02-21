using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten
{
    internal interface IResourceStat
    {
        float GetValue(ResourceStat stat);
    }
}
