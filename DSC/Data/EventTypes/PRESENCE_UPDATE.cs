using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSC.Data.EventTypes
{
    public class PRD
    {
        public User user { get; set; }
        public string status { get; set; }
        public List<string> roles { get; set; }
        public object premium_since { get; set; }
        public string nick { get; set; }
        public string guild_id { get; set; }
        public Game game { get; set; }
        public ClientStatus client_status { get; set; }
        public List<Activity> activities { get; set; }
    }

    public class PRESENCE_UPDATE
    {
        public string t { get; set; }
        public int s { get; set; }
        public int op { get; set; }
        public PRD d { get; set; }
    }
}
