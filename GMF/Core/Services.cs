using System;
using System.Collections.Generic;
using DynamicData;
using GMF.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GMF
{
    public static class Services
    {
        static IServiceProvider ServiceProvider { get; set; }
        
        public static void Initialize(IEnumerable<ServiceDescriptor> serviceDescriptors = null)
        {
            IServiceCollection collection = new ServiceCollection();
            
            if (serviceDescriptors != null)
            {
                collection.AddRange(serviceDescriptors);
            }
            
            collection.AddServicesWithAttribute();
            ServiceProvider = collection.BuildServiceProvider();
        }
        
        public static T GetService<T>(Boolean required = true)
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("Service provider is not initialized.");
            }
            
            return required ? ServiceProvider.GetRequiredService<T>() : ServiceProvider.GetService<T>();
        }
        
        public static IServiceScope CreateScope()
        {
            return ServiceProvider.CreateScope();
        }
    }
}