using AtlasPatient.API.DataInjest;
using AtlasPatient.Core.IServices;
using AtlasPatient.Core.Services;
using AtlasPatient.Data;
using AtlasPatient.Data.IRepository;
using AtlasPatient.Data.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnString")));

builder.Services.AddHttpClient<IPatientService, PatientService>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    // Configure handler if needed
                });

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddMassTransit(bcfg =>
{
    bcfg.SetKebabCaseEndpointNameFormatter();
    bcfg.AddConsumer<DataInjestConsumer>();

    bcfg.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["ServiceBus:Host"]!), h => {
            h.Username(builder.Configuration["ServiceBus:User"]);
            h.Password(builder.Configuration["ServiceBus:Password"]);
        });

        cfg.ConfigureEndpoints(context);
    });
});

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

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();

