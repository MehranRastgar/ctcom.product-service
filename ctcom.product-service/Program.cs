using ctcom.ProductService.Data;
using ctcom.ProductService.Messaging;
using ctcom.ProductService.Repositories;
using ctcom.ProductService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Configure the database (replace with your connection string)
var connectionString = builder.Configuration.GetConnectionString("ProductDatabase");
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Add MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// Register the RabbitMessageProducer
builder.Services.AddScoped<IMessageProducer, RabbitMessageProducer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add controllers
builder.Services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
