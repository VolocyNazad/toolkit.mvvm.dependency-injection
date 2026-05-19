using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace MVVM.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddView<TView>(
            ServiceLifetime lifetime = ServiceLifetime.Scoped) where TView : UserControl
        {
            var viewModelType = GetViewModel(typeof(TView));

            services.Add(new ServiceDescriptor(viewModelType, viewModelType, lifetime));
            services.Add(new ServiceDescriptor(typeof(TView), provider =>
            {
                var viewModel = provider.GetRequiredService(viewModelType);
                TView view = ActivatorUtilities.CreateInstance<TView>(provider);
                view.DataContext = viewModel;
                return view;
            }, lifetime));

            return services;
        }
    }

    private const string ViewSuffix = "View";
    private const string ViewModelSuffix = "ViewModel";
    private static Type GetViewModel(Type viewType)
    {
        var viewName = viewType.Name;

        if (!viewName.EndsWith(ViewSuffix))
            throw new InvalidViewNamingConventionException($"View name '{viewName}' not ends with '{ViewSuffix}'");
        var expectedViewModelName = viewName.Replace(ViewSuffix, ViewModelSuffix);
        var viewModel = viewType.Assembly.GetTypes()
            .FirstOrDefault(t =>
                t.Name == expectedViewModelName && t.IsClass && !t.IsAbstract);

        return viewModel ?? throw new ViewModelNotFoundException($"""
            ViewModel not found for view {viewName}.
            Expected ViewModel name: {expectedViewModelName}
            """);
    }
}
