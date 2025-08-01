
var builder = DistributedApplication.CreateBuilder(args);

//backing services 
var postgres = builder
        .AddPostgres("postgres")
        .WithPgAdmin()
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogDb");

var cache = builder
        .AddRedis("cache")
        .WithRedisInsight()
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);

var rabbitMq = builder
        .AddRabbitMQ("rabbitmq")
        .WithManagementPlugin()
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder
        .AddKeycloak("keycloak", 8080)
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);

//Projects
var catalog = builder
        .AddProject<Projects.Catalog>("catalog")
        .WithReference(catalogDb)
        .WithReference(rabbitMq)
        .WaitFor(catalogDb)
        .WaitFor(rabbitMq);

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
