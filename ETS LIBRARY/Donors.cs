using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    class Donors : CollectionBase
    {
        public void add(Donor donor)
        {
            List.Add(donor);
        }

        public void remove(Donor donor)
        {
            List.Remove(donor);
        }

        public Donor this[int index]
        {
            get { return (Donor)List[index]; }
            set { List[index] = value; }
        }
    }
}
