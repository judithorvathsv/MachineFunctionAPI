using System;
using System.Threading.Tasks;
using MachineFunctionAPI.Model;
using Microsoft.Azure.Cosmos.Table;
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
        public static async Task Run([QueueTrigger("machinequeue", Connection = "AzureWebJobsStorage")] Machine todo,  
        [Blob("blobstorage", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
        ILogger log)
        {

            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{todo.Id}.txt");
            var lastmodified = blob.Properties.LastModified;
            await blob.UploadTextAsync($"Created a new task: {todo.SentData}, for {todo.Id} machine");
            log.LogInformation($"Queue trigger function processed: {todo.SentData}");
        }
    }
}
