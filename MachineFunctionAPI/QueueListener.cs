using System;
using System.Threading.Tasks;
using MachineFunctionAPI.Model;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MachineFunctionAPI
{
    public static class QueueListener
    {
        //[FunctionName("QueueListener")]
        //public static void Run([QueueTrigger("todos", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        //{
        //    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        //}


        [FunctionName("QueueListener")]
        public static async Task Run([QueueTrigger("machineItem", Connection = "AzureWebJobsStorage")] Machine todo,  
        [Blob("machineItem", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
        TraceWriter log)
        {
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{todo.Id}.txt");
            await blob.UploadTextAsync($"Created a new task: {todo.SentData}");
            log.Info($"Queue trigger function processed: {todo.SentData}");
        }
    }
}
