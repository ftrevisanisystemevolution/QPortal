using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface.Model
{
    public class SenseStream
    {
        public string Id { get; private set; }
        public string Name { get; set; }

        public SenseStream(string id)
        {
            Id = id;
        }
    }
}
