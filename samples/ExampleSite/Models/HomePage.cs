using System;
using Kasbah.Content.Models;

namespace ExampleSite.Models
{
    public class HomePage : Item
    {
        public string Body { get; set; }
        public string ShortText { get; set; }
        public string LongText { get; set; }
        public DateTime Date { get; set; }
    }
}