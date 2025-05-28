using App.Controller;
using App.Dependency;
using Microsoft.Extensions.DependencyInjection;

var appDependency = AppDependency.RegisterDependencies();

Start();

void Start()
{
    var cinemaController = appDependency.GetRequiredService<ICinemaController>();
    cinemaController.StartCinemaApplication();
}