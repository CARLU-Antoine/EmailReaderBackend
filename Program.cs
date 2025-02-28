using EmailReaderBackend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Middleware HTTP
app.UseHttpsRedirection();
app.UseAuthorization();


app.MapControllers(); 

app.Run();

