using Example03.Infrastructure;
using Example03.Presentation;

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

app.UseAuthorization();

app.MapControllers();

app.Run();