using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MachineFunctionAPI.Model;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;

namespace MachineFunctionAPI
{
    public static class MachineApi
    {
        [FunctionName("Create")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "machine")] HttpRequest req, ILogger log, 
            [Table("machineitems", Connection = "AzureWebJobsStorage")] IAsyncCollector<ItemTableEntity>machineTable)
        {
            log.LogInformation("Create new item.");
       

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var createMachine = JsonConvert.DeserializeObject<CreateMachineDTO>(requestBody);

            if (createMachine is null || string.IsNullOrWhiteSpace(createMachine?.Name)) {
                return new BadRequestResult();
            }

            var item = new Machine
            {
                Name = createMachine.Name,
                Status = false,
                SentData = createMachine.SentData
            };

            await machineTable.AddAsync(item.ToTableEntity());

            return new OkObjectResult(item);
        }



        [FunctionName("Get")]
        public static async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machine")] HttpRequest req, ILogger log,
        [Table("machineitems", Connection = "AzureWebJobsStorage")] CloudTable machineTable)
        {
            log.LogInformation("Get all item.");

            var query = new TableQuery<ItemTableEntity>();
            var result = await machineTable.ExecuteQuerySegmentedAsync(query, null);

            var response = new MachineList { Machines = result.Select(Mapper.ToMachine).ToList() };
            return new OkObjectResult(response);
        }



        [FunctionName("Put")]
        public static async Task<IActionResult> Put(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "machine/{id}")] HttpRequest req, ILogger log,
        [Table("machineitems", Connection = "AzureWebJobsStorage")] CloudTable machineTable, string id)
        {
            log.LogInformation("Update item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updateMachine = JsonConvert.DeserializeObject<Machine>(requestBody);

            if (updateMachine is null || updateMachine.Id != id)
            {
                return new BadRequestResult();
            }

            var itemTable = updateMachine.ToTableEntity();

            itemTable.ETag = "*";

            var operation = TableOperation.Replace(itemTable);   
            await machineTable.ExecuteAsync(operation);

            return new NoContentResult();
        }



        [FunctionName("Delete")]
        public static async Task<IActionResult> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "machine/{id}")] HttpRequest req, ILogger log,
        [Table("machineitems",/* "MachinePartitionKey"*/ "MachineTodo", "{id}", Connection = "AzureWebJobsStorage")] ItemTableEntity itemToRemove,
        [Table("machineitems", Connection = "AzureWebJobsStorage")] CloudTable machineTable, string id
        )

        {
            log.LogInformation("Delete item.");

            if (string.IsNullOrWhiteSpace(id)) {
                return new BadRequestResult();
            }

            if (itemToRemove is null)
            {
                return new NotFoundResult();
            }

            //var itemTableToDelete = new ItemTableEntity
            //{
            //    PartitionKey = "MachinePartitionKey",
            //    RowKey = id,
            //    ETag = "*"
            //};

            //var operation = TableOperation.Delete(itemTableToDelete);

            var operation = TableOperation.Delete(itemToRemove);
            await machineTable.ExecuteAsync(operation);

            return new NoContentResult();
        }





    }
}
