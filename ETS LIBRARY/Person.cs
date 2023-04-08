using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    abstract class Person
    {
        private string firstName;
        private string lastName;

        public Person(string fn, string ln)
        {
            this.firstName = fn;
            this.lastName = ln;
        }

        public string FirstName { get { return firstName; } set { firstName = value; } }
        public string LastName { get { return lastName; } set { lastName = value; } }

        public virtual string toString()
        {
            return FirstName + "," + LastName;
        }

        public string getFN()
        {
            return FirstName;
        }
    }
}
