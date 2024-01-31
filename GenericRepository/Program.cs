using GenericRepository.Abstract;
using GenericRepository.Concreate;
using GenericRepository.DbContext;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var service = builder.Services;

// DbContext
service.AddDbContext<AppDbContext>();

service.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

// Add services to the container.

builder.Services.AddControllers();

service.AddControllers().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
