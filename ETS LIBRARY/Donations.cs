using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    class Donations : CollectionBase
    {
        public void add(Donation donation)
        {
            List.Add(donation);
        }

        public void remove(Donation donation)
        {
            List.Remove(donation);
        }

        public Donation this[int index]
        {
            get { return (Donation)List[index]; }
            set { List[index] = value; }
        }

    }
}
