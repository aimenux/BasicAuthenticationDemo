using Example07;
using Example07.Infrastructure;
using Example07.Presentation;
using Example07.Presentation.Authentication;

var builder = WebApplication.CreateBuilder(args)
    .WithHttpsOnly()
    .AddPresentation()
    .AddInfrastructure();

builder.Services.AddBasicAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app
    .MapControllers()
    .RequireAuthorization();

app.Run();