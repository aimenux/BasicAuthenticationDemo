using Example01.Infrastructure;
using Example01.Presentation;
using Example01.Presentation.Authentication;

var app = WebApplication.CreateBuilder(args)
    .AddPresentation()
    .AddInfrastructure()
    .Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseBasicAuthentication();

app.MapControllers();

app.Run();