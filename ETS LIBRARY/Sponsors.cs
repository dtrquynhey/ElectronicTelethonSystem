using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    class Sponsors : CollectionBase
    {
        public void add(Sponsor sponsor)
        {
            List.Add(sponsor);
        }

        public void remove(Sponsor sponsor)
        {
            List.Remove(sponsor);
        }

        public Sponsor this[int index]
        {
            get { return (Sponsor)List[index]; }
            set { List[index] = value; }
        }

        public void update(string sponsorID, string fn, string ln)
        {
            foreach (Sponsor sponsor in List)
            {
                if (sponsorID == sponsor.getID())
                {
                    sponsor.FirstName = fn;
                    sponsor.LastName = ln;
                }
            }
        }
    }
}
