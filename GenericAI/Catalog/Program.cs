
using Microsoft.SemanticKernel;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogDb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

// Register Ollama-based chat & embedding
builder.AddOllamaApiClient(connectionName: "ollama-llama3-2")
       .AddChatClient();

builder.AddOllamaApiClient(connectionName: "ollama-all-minilm")
       .AddEmbeddingGenerator();

// Register an pg vector store for semantic search
builder.Services.AddSingleton<NpgsqlDataSource>(sp =>
{
    NpgsqlDataSourceBuilder dataSourceBuilder = new("Host=localhost;Port=5434;Username=postgres;Password=password;Database=vactordbtest;");
    dataSourceBuilder.UseVector();
    return dataSourceBuilder.Build();
});

builder.Services.AddPostgresVectorStore();
builder.Services.AddPostgresCollection<int, ProductVector>("products");

// Register an in-memory vector store for semantic search
builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.UseMigration();

app.MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();
