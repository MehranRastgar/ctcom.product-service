using ctcom.ProductService.Data;
using ctcom.ProductService.Mapping;
using ctcom.ProductService.Messaging;
using ctcom.ProductService.Repositories;
using ctcom.ProductService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using ctcom.ProductService.DTOs.Validation;

var builder = WebApplication.CreateBuilder(args);


// Configure the database (replace with your connection string)
var connectionString = builder.Configuration.GetConnectionString("ProductDatabase");
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,               // Maximum number of retries
            maxRetryDelay: TimeSpan.FromSeconds(30),  // Maximum delay between retries
            errorNumbersToAdd: null          // List of error numbers to retry on (optional)
        );
    })
);

// Add FluentValidation and validators
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());


// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));


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
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins(["http://localhost:3000"]) // origins
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials();
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
    app.UseCors("CorsPolicy");
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
