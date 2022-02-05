using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p =>
        p.AllowAnyOrigin()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

var jsonBytes = File.ReadAllBytes("./posts.json");
var jsonDoc = JsonDocument.Parse(jsonBytes);
var posts = JsonSerializer.Deserialize<List<Post>>(jsonDoc);

app.MapGet("/posts", () => Results.Ok(posts));
app.MapGet("/posts/{id}", (int id) =>
{
    var post = posts.FirstOrDefault(x => x.Id == id);
    if (post == null) return Results.NotFound();
    return Results.Ok(post);
});
app.MapPost("/posts", (Post post) =>
{
    posts.Add(post);
    return Results.Ok();
});
app.MapPut("/posts/{id}", (int id, Post post) =>
{
    var index = posts.FindIndex(x => x.Id == id);
    if (index < 0) return Results.NotFound();
    posts[index] = post;
    return Results.Ok();
});
app.MapDelete("/posts/{id}", (int id) =>
{
    var index = posts.FindIndex(x => x.Id == id);
    if (index >= 0) posts.RemoveAt(index);
    return Results.Ok();
});

app.Run();

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
}
