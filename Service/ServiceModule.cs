using Domain.Accessor;
using Microsoft.Extensions.DependencyInjection;
using Service.MenuItemSelection;
using Service.Screen;
using Service.SeatSelection;

namespace Service;

public static class ServiceModule
{
    public static ServiceProvider RegisterDependencies()
    {
        var buildServiceProvider = new ServiceCollection()
            .AddSingleton<ICinemaAccessor, CinemaAccessor>()
            .AddTransient<IMenuItemSelectionService, BookTicketsService>()
            .AddTransient<IMenuItemSelectionService, CheckBookingsService>()
            .AddTransient<IMenuItemSelectionService, ExitService>()
            .AddTransient<ISeatSelectionService, SeatSelectionService>()
            .AddTransient<IScreenService, ScreenService>()
            .BuildServiceProvider();
        return buildServiceProvider;
    }
}