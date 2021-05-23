using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            CosmosClient client = new CosmosClient(configuration["ConnectionString"],
                new CosmosClientOptions() { AllowBulkExecution=true });

            // CreateDatabaseAndContainer(client, configuration);
            // AddItemToContainer(client, configuration);
            // AddMultipleItemsToContainer(client, configuration);
            // ReadItems(client, configuration);
            // UpdateItem(client, configuration);
            DeleteItem(client, configuration);

            Console.ReadKey();
        }

        private static void DeleteItem(CosmosClient client, IConfiguration configuration)
        {
            Container container = client.GetContainer(configuration["DatabaseName"], configuration["ContainerName"]);

            string id = "2";
            PartitionKey partitionKey = new PartitionKey("C00020");

            container.DeleteItemAsync<Course>(id, partitionKey).GetAwaiter().GetResult();

            Console.WriteLine($"Item with id {id} has been deleted");
        }

        private static void UpdateItem(CosmosClient client, IConfiguration configuration)
        {
            Container container = client.GetContainer(configuration["DatabaseName"], configuration["ContainerName"]);

            string id = "2";
            PartitionKey partitionKey = new PartitionKey("C00020");

            ItemResponse<Course> response = container.ReadItemAsync<Course>(id, partitionKey).GetAwaiter().GetResult();

            Course course = response.Resource;

            course.rating = 4.8m;

            container.ReplaceItemAsync<Course>(course, id, partitionKey).GetAwaiter().GetResult();

            Console.WriteLine($"Item with id {id} has been updated");
        }

        private static void ReadItems(CosmosClient client, IConfiguration configuration)
        {
            Container container = client.GetContainer(configuration["DatabaseName"], configuration["ContainerName"]);

            string query = "SELECT * FROM c WHERE c.courseid='C00010'";

            QueryDefinition definition = new QueryDefinition(query);

            FeedIterator<Course> iterator = container.GetItemQueryIterator<Course>(definition);

            while (iterator.HasMoreResults)
            {
                FeedResponse<Course> response = iterator.ReadNextAsync().GetAwaiter().GetResult();
                foreach(var course in response)
                {
                    Console.WriteLine($"Course id is {course.courseid}");
                    Console.WriteLine($"Course name is {course.coursename}");
                    Console.WriteLine($"Course rating is {course.rating}");
                }
            }
        }

        private static void AddMultipleItemsToContainer(CosmosClient client, IConfiguration configuration)
        {
            List<Course> courses = new List<Course>()
            {
                new Course()
                {
                    id = "1",
                    courseid = "C00010",
                    coursename = "AZ-204",
                    rating = 4.5m
                },
                new Course()
                {
                    id = "2",
                    courseid = "C00020",
                    coursename = "AZ-303",
                    rating = 4.6m
                },
                new Course()
                {
                    id = "3",
                    courseid = "C00030",
                    coursename = "AZ-900",
                    rating = 4.7m
                },
                new Course()
                {
                    id = "4",
                    courseid = "C00040",
                    coursename = "AZ-104",
                    rating = 4.9m
                }
            };
            Container container = client.GetContainer(configuration["DatabaseName"], configuration["ContainerName"]);

            List<Task> tasks = new List<Task>();

            foreach(var course in courses)
            {
                tasks.Add(container.CreateItemAsync<Course>(course, new PartitionKey(course.courseid)));
            }

            Task.WhenAll(tasks).GetAwaiter().GetResult();

            Console.WriteLine("Bulk insert completed");
        }

        private static void AddItemToContainer(CosmosClient client, IConfiguration configuration)
        {
            Course course = new Course()
            {
                id = "1",
                courseid = "C00010",
                coursename = "AZ-204",
                rating = 4.5m
            };

            Container container = client.GetContainer(configuration["DatabaseName"], configuration["ContainerName"]);

            container.CreateItemAsync<Course>(course, new PartitionKey(course.courseid)).GetAwaiter().GetResult();

            Console.WriteLine($"Course with id {course.courseid} added");
        }

        private static void CreateDatabaseAndContainer(CosmosClient client, IConfiguration config)
        {
            client.CreateDatabaseAsync(config["DatabaseName"]).GetAwaiter().GetResult();

            Database database = client.GetDatabase(config["DatabaseName"]);
            database.CreateContainerAsync(config["ContainerName"], config["PartitionKey"]).GetAwaiter().GetResult();

            Console.WriteLine("Database and container created");
        }
    }
}
