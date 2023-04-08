using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    class Prizes : CollectionBase
    {
        public void add(Prize prize)
        {
            List.Add(prize);
        }

        public void remove(Prize prize)
        {
            List.Remove(prize);
        }

        public Prize this[int index]
        {
            get { return (Prize)List[index]; }
            set { List[index] = value; }
        }
    }
}
