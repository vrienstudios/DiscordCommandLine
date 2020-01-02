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
    }
}
