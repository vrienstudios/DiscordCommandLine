using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSC
{
    public static class StaticData
    {
        public static Data.User CurrentUser { get; set; }
        public static List<Data.Guild> Guilds { get; set; }
        public static List<Data.EventTypes.MESSAGE_CREATE.Event_message_create> Messages {get; set;}
    }
}
