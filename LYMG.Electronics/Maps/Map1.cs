using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYMG.Electronics.Maps
{
    class Map1 : SeriesContext<CHS24B>
    {
        protected override CHS24B CreateDataItem() => new CHS24B();
    }
}
