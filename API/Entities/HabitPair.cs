using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Entities
{
    public class HabitPair
    {
        public String HabitName { get; set; }
        [JsonIgnore]
        public AppUser SourceUser { get; set; }
        public String SourceUserName { get; set; }
        public int SourceUserId { get; set; }
        [JsonIgnore]
        public AppUser OtherUser { get; set; }
        public String OtherUserName { get; set; }
        public int OtherUserId { get; set; }
        public String SourceUserGraph { get; set; }
        public String OtherUserGraph { get; set; }
    }
}