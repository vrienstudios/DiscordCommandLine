using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSC.Data.EventTypes
{
    public class DMsgDelete
    {
        public string id { get; set; }
        public string channel_id { get; set; }
    }

    public class MESSAGE_DELETE
    {
        public string t { get; set; }
        public int s { get; set; }
        public int op { get; set; }
        public DMsgDelete d { get; set; }
    }
}
