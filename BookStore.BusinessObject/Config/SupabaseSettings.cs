using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.Config
{
    public class SupabaseSettings
    {
        public required string Url { get; set; }
        public required string Key { get; set; }
        public required string Bucket { get; set; }
    }
}
