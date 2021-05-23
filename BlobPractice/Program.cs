using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
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
            // UploadBlob(client, configuration);
            // ListBlobs(client, configuration);
            // DownloadBlob(client, configuration);
            var sas = GenerateSas(client, configuration);
            Console.WriteLine(sas);
            Console.ReadKey();
        }

        private static Uri GenerateSas(BlobServiceClient client, IConfiguration configuration)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(configuration["ContainerName"]);
            BlobClient blobClient = containerClient.GetBlobClient(configuration["BlobName"]);

            BlobSasBuilder builder = new BlobSasBuilder()
            {
                BlobContainerName = configuration["ContainerName"],
                BlobName = configuration["BlobName"],
                Resource = "b"
            };

            builder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.List);

            builder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);

            return blobClient.GenerateSasUri(builder);
        }

        private static void DownloadBlob(BlobServiceClient client, IConfiguration configuration)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(configuration["ContainerName"]);
            BlobClient blobClient = containerClient.GetBlobClient(configuration["BlobName"]);

            // It looks like the file is overwritten if the name is the same
            blobClient.DownloadTo(configuration["DownloadLocation"]);
            Console.WriteLine("Blob has been downloaded");
        }

        static void ListBlobs(BlobServiceClient client, IConfiguration configuration)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(configuration["ContainerName"]);
            foreach (BlobItem item in containerClient.GetBlobs())
            {
                Console.WriteLine(item.Name);
            }
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
