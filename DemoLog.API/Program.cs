using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
var urlElastic = builder.Configuration.GetValue<string>("Elasticsearch:Uri"); //Busca url elastic

// configuração serilog conectando no elastic 
var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.WithElasticApmCorrelationInfo() // rastreamento de log
    .WriteTo.Elasticsearch(
        options:
            new ElasticsearchSinkOptions(
                new Uri(urlElastic))
            {
                CustomFormatter = new EcsTextFormatter() // formatador Elastic Common Schema 
            })
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.AddSerilog(logger);
// Add services to the container.

builder.Services.AddControllers();
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
