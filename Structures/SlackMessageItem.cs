using System;

namespace ECCUnofficial.Structures
{
    public class SlackMessageItem
    {
        public long Id { get; set; }
        public string Channel { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public double Timestamp { get; set; }
        public double EventTimestamp { get; set; }
        public DateTime DateTime { get; set; }
    }
}
