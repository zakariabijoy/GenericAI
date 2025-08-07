
var builder = DistributedApplication.CreateBuilder(args);

//backing services 
var postgres = builder
        .AddPostgres("postgres")
        .WithPgAdmin()
        .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogDb");

var cache = builder
        .AddRedis("cache")
        .WithRedisInsight()
        .WithLifetime(ContainerLifetime.Persistent);

var rabbitMq = builder
        .AddRabbitMQ("rabbitmq")
        .WithManagementPlugin()
        .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder
        .AddKeycloak("keycloak", 8080)
        .WithLifetime(ContainerLifetime.Persistent);

if (builder.ExecutionContext.IsRunMode)
{
    //Data volumes don't work on ACA for Postgres so only add when running locally
    postgres.WithDataVolume();
    cache.WithDataVolume();
    rabbitMq.WithDataVolume();
    keycloak.WithDataVolume();
}

var ollama = builder
        .AddOllama("ollama", 11434)
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent)
        .WithOpenWebUI();

var llama = ollama.AddModel("llama3.2");
var embedding = ollama.AddModel("all-minilm");

//Projects
var catalog = builder
        .AddProject<Projects.Catalog>("catalog")
        .WithReference(catalogDb)
        .WithReference(rabbitMq)
        .WithReference(llama)
        .WithReference(embedding)
        .WaitFor(catalogDb)
        .WaitFor(rabbitMq)
        .WaitFor(llama)
        .WaitFor(embedding);

var basket = builder
        .AddProject<Projects.Basket>("basket")
        .WithReference(cache)
        .WithReference(catalog)
        .WithReference(rabbitMq)
        .WithReference(keycloak)
        .WaitFor(cache)
        .WaitFor(rabbitMq)
        .WaitFor(keycloak);

var webapp = builder
        .AddProject<Projects.WebApp>("webapp")
        .WithExternalHttpEndpoints()
        .WithReference(cache)
        .WithReference(catalog)
        .WithReference(basket)
        .WaitFor(catalog)
        .WaitFor(basket);

builder.Build().Run();
