using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GMF.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesWithAttribute(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var typesWithAttributes = assembly.GetTypes()
                    .Where(type => CustomAttributeExtensions.GetCustomAttribute<ServiceDescriptorAttribute>((MemberInfo)type) != null);
                
                foreach (var type in typesWithAttributes)
                {
                    var attribute = type.GetCustomAttribute<ServiceDescriptorAttribute>();
                    var lifetime = attribute?.Lifetime ?? ServiceLifetime.Transient;
                    
                    services.Add(new ServiceDescriptor(type.GetInterfaces().FirstOrDefault(), type, lifetime));
                }
            }
            
            return services;
        }
    }
}