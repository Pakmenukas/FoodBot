// See https://aka.ms/new-console-template for more information

using FoodBot.Application;
using FoodBot.Application.Bank;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = CreateHostBuilder().Build();
host.Services.ApplyMigrations();

var mediator = host.Services.GetService<ISender>();

mediator.Send(new AddMoneyCommand(123, new AddMoneyCommand.Target(123, "Gabrielius"), 20)).Wait();

return;

static IHostBuilder CreateHostBuilder()
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.ConfigureApplication();
            services.ConfigureInfrastructure();
        });
}