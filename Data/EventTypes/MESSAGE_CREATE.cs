using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSC.Data.EventTypes
{
    class MESSAGE_CREATE
    {
        public class Author
        {
            public string username { get; set; }
            public string id { get; set; }
            public string discriminator { get; set; }
            public string avatar { get; set; }
        }

        public class MESSAGE_CREATE_D
        {
            public int type { get; set; }
            public bool tts { get; set; }
            public DateTime timestamp { get; set; }
            public bool pinned { get; set; }
            public string nonce { get; set; }
            public List<object> mentions { get; set; }
            public List<object> mention_roles { get; set; }
            public bool mention_everyone { get; set; }
            public string id { get; set; }
            public int flags { get; set; }
            public List<object> embeds { get; set; }
            public object edited_timestamp { get; set; }
            public string content { get; set; }
            public string channel_id { get; set; }
            public Author author { get; set; }
            public List<object> attachments { get; set; }
        }

        public class Event_message_create
        {
            public string t { get; set; }
            public int s { get; set; }
            public int op { get; set; }
            public MESSAGE_CREATE_D d { get; set; }
        }
    }
}
