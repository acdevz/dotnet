using BookstoreGraphQLDemo.Domains;

var builder = WebApplication.CreateBuilder(args);

/* GraphQL services */
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.MapGraphQL();

app.Run();
