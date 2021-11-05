using MachineFunctionAPI.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineFunctionAPI
{
   public static class Mapper
    {
        public static ItemTableEntity ToTableEntity(this Machine item) {

            return new ItemTableEntity
            {
                Name = item.Name,
                Status = item.Status,
                SentData = item.SentData,
                PartitionKey = "MachineTodo",
                RowKey = item.Id
            };
        }

        public static Machine ToMachine(this ItemTableEntity itemTable)
        {
            return new Machine
            {
                Id=itemTable.RowKey,
                Name = itemTable.Name,
                Status = itemTable.Status,
                SentData = itemTable.SentData,        
            };
        }
    }
}
