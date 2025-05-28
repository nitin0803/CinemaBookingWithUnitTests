using App.Controller;
using Domain.Accessor;
using Domain.CinemaConsole;
using Microsoft.Extensions.DependencyInjection;
using Service.MenuItemSelection;
using Service.Screen;
using Service.SeatSelection;

namespace App.Dependency;

public static class AppDependency
{
    public static ServiceProvider RegisterDependencies()
    {
        var buildServiceProvider = new ServiceCollection()
            .AddTransient<ICinemaConsole, CinemaConsole>()
            .AddSingleton<ICinemaAccessor,CinemaAccessor>()
            .AddTransient<IMenuItemSelectionService, BookTicketsService>()
            .AddTransient<IMenuItemSelectionService, CheckBookingsService>()
            .AddTransient<IMenuItemSelectionService, ExitService>()
            .AddTransient<ISeatSelectionService, SeatSelectionService>()
            .AddTransient<IScreenService, ScreenService>()
            .AddSingleton<ICinemaController, CinemaController>()
            .BuildServiceProvider();
        
        return buildServiceProvider;
    }
}