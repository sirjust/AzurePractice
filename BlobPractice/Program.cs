using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;

namespace BlobPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            BlobServiceClient client = new BlobServiceClient(configuration["ConnectionString"]);
            // CreateContainer(client, configuration);
            UploadBlob(client, configuration);
            Console.ReadKey();
        }

        static void UploadBlob(BlobServiceClient client, IConfiguration config)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(config["ContainerName"]);
            BlobClient blobClient = containerClient.GetBlobClient(config["BlobName"]);
            blobClient.Upload(config["BlobLocation"]);

            Console.WriteLine("Blob has been uploaded");
        }

        static void CreateContainer(BlobServiceClient client, IConfiguration config)
        {
            client.CreateBlobContainer(config["ContainerName"]);

            Console.WriteLine("Container created");
        }
    }
}
