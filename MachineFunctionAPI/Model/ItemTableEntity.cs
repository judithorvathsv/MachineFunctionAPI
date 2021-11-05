using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineFunctionAPI.Model
{
   public class ItemTableEntity : TableEntity
    {

        public string Name { get; set; }

        public bool Status { get; set; }

        public string SentData { get; set; }
    }
}
