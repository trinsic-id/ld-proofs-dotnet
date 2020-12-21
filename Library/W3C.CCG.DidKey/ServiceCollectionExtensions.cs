using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using W3C.CCG.DidCore;
using W3C.CCG.DidKey;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a driver for did:key method
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddDidKey(this IServiceCollection services)
        {
            services.AddSingleton<IDidDriver, DidKeyDriver>();
            return services;
        }
    }
}
