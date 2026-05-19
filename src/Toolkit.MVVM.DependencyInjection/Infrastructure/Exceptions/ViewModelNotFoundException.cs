namespace MVVM.DependencyInjection.Infrastructure.Exceptions;

internal sealed class ViewModelNotFoundException(string message) : Exception(message);
