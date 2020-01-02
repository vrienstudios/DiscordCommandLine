using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSC.Data.EventTypes
{
    public class RD
    {
        public string user_id { get; set; }
        public string message_id { get; set; }
        public Emoji emoji { get; set; }
        public string channel_id { get; set; }
    }

    public class MESSAGE_REACTION_ADD
    {
        public string t { get; set; }
        public int s { get; set; }
        public int op { get; set; }
        public RD d { get; set; }
    }
}
