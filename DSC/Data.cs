using System.Collections.Generic;

namespace DSC
{
    public static class StaticData
    {
        public static Data.User CurrentUser { get; set; }
        public static List<Data.Guild> Guilds { get; set; }
        public static List<Data.EventTypes.MESSAGE_CREATE.Event_message_create> Messages = new List<Data.EventTypes.MESSAGE_CREATE.Event_message_create>();
    }
}
