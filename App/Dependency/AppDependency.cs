using App.Controller;
using Domain.Accessor;
using Domain.CinemaConsole;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using NLog.Extensions.Logging;
using Service.MenuItemSelection;
using Service.Screen;
using Service.SeatSelection;

namespace App.Dependency;

public static class AppDependency
{
    public static ServiceProvider RegisterDependencies()
    {

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var buildServiceProvider = new ServiceCollection()
            .AddTransient<ICinemaConsole, CinemaConsole>()
            .AddSingleton<ICinemaAccessor,CinemaAccessor>()
            .AddTransient<IMenuItemSelectionService, BookTicketsService>()
            .AddTransient<IMenuItemSelectionService, CheckBookingsService>()
            .AddTransient<IMenuItemSelectionService, ExitService>()
            .AddTransient<ISeatSelectionService, SeatSelectionService>()
            .AddTransient<IScreenService, ScreenService>()
            .AddSingleton<ICinemaController, CinemaController>()
            .AddSingleton(config)
            .AddLogging(loggerBuilder =>
            {
                loggerBuilder.AddConfiguration(new ConfigurationManager());
                loggerBuilder.AddNLog();
            })
            .BuildServiceProvider();
        return buildServiceProvider;
    }
}