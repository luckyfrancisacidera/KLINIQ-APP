using FluentValidation;
using Kliniq.Application.Common.Behaviors;
using MediatR;
using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kliniq.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);
            services.AddSingleton<NegationDetector>();
            services.AddSingleton<ISymptomAnalysisService, ExplainableSymptomAnalysisService>();
            return services;
        }
    }
}
