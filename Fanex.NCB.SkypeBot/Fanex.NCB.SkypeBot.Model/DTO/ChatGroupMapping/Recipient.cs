using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping
{
    public class Recipient
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int RecipientTypeId { get; set; }
        public string Status { get; set; }

    }
}
