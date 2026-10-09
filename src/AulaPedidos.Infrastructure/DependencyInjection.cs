using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AulaPedidos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        return services;
    }
}




/*
 * 
 * 
 * 1 Coordinador
 * 1.1 Humano revisar
 * 2.1 Arquitecto leer el expediente
*  2.2 Pruebas define alcance y casos de uso de lo que pedimos
*  2.3 Humano revisar arquitectura y pruebas
*  3 Coordinador en el chat inicial crear una sinteisis de 2.1 y 2.2
*  3.1 Humano revisar la sinteisis
*  4 implementador Implementar (seleccionar un modelo chido)
*  5 humnano revisa codigo
*  6 evualuar pruebas
*  7 humano revisa pruebas
*/