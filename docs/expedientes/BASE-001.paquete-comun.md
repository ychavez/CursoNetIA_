# Paquete común BASE-001 — Esqueleto de AulaPedidos

## Control

- Versión candidata: 1
- Estado: no congelado
- Motivo: faltan commit, diff, archivos nuevos y comprobación de existencia de
  los archivos candidatos.
- Destinatarios previstos:
  - Informe A: Arquitecto
  - Informe B: Pruebas
- Modalidad: consultas manuales en dos chats separados de Visual Studio.
- Este paquete no contiene informes A/B ni síntesis.

No debe entregarse a A/B hasta completar el estado base y declararlo congelado.

## Objetivo

Analizar la creación del esqueleto compilable y verificable de AulaPedidos en
.NET 10, tomando este repositorio como referencia.

El resultado candidato contiene únicamente:

- una solución de AulaPedidos;
- cuatro capas: Domain, Application, Infrastructure y Api;
- referencias entre proyectos;
- composición mediante DI;
- Minimal API;
- documento OpenAPI;
- health check;
- proyectos y pruebas mínimas del esqueleto.

No incluye lógica de negocio.

## Contexto normativo

- `.github/copilot-instructions.md`
- `docs/arquitectura.md`
- `docs/seguridad.md`
- `docs/multiagente-copilot.md`

Hechos relevantes procedentes de esos documentos:

1. Domain no depende de EF, HTTP ni Infrastructure.
2. Application depende de Domain.
3. Infrastructure puede depender de Application y Domain.
4. Api referencia Application e Infrastructure como punto de composición.
5. La superficie HTTP usa Minimal APIs.
6. No se añaden paquetes, servicios ni abstracciones sin necesidad concreta.
7. Los comandos y sus resultados sólo cuentan como evidencia cuando han sido
   realmente ejecutados e identificados.

## Dependencias de compilación candidatas

```
Api -> Application
Api -> Infrastructure
Infrastructure -> Application
Infrastructure -> Domain
Application -> Domain
Domain -> ninguna capa
```

Las pruebas sólo referencian los proyectos necesarios para sus casos.

## Alcance

### Producción

- `src/AulaPedidos.Domain`
- `src/AulaPedidos.Application`
- `src/AulaPedidos.Infrastructure`
- `src/AulaPedidos.Api`

### Pruebas

- `tests/AulaPedidos.ArchitectureTests`
- `tests/AulaPedidos.Api.IntegrationTests`

### Capacidades

- Proyectos con destino .NET 10.
- Solución `AulaPedidos.slnx`.
- Referencias de proyecto conforme al grafo permitido.
- Punto de composición en Api.
- Minimal API.
- Health check básico, con ruta candidata `GET /health`.
- Documento OpenAPI mediante capacidades compatibles con ASP.NET Core
  para .NET 10.
- Pruebas de arquitectura e integración del esqueleto.

La DI debe ser real, pero no se crearán interfaces, servicios o adaptadores
ficticios sólo para llenar el contenedor.

## Exclusiones

- Lógica de catálogo o pedidos.
- Entidades, value objects, agregados y eventos.
- Casos de uso, DTOs de negocio, handlers y mediador.
- EF Core, DbContext, repositorios, migraciones y base de datos.
- Autenticación, autorización, JWT, roles y permisos.
- Notificaciones, outbox, caché y resiliencia.
- Controllers MVC.
- Docker, despliegue y CI/CD.
- Secretos y datos de ejemplo.
- Herramientas auxiliares.
- Cambios en documentación técnica, perfiles o bitácoras.
- Renombrado o eliminación de la solución existente.
- Paquetes y abstracciones no indispensables.

## Archivos candidatos permitidos

- `AulaPedidos.slnx`
- `src/AulaPedidos.Domain/AulaPedidos.Domain.csproj`
- `src/AulaPedidos.Application/AulaPedidos.Application.csproj`
- `src/AulaPedidos.Infrastructure/AulaPedidos.Infrastructure.csproj`
- `src/AulaPedidos.Api/AulaPedidos.Api.csproj`
- `src/AulaPedidos.Api/Program.cs`
- `src/AulaPedidos.Api/Properties/launchSettings.json`, condicionado a necesidad
  de plantilla y ausencia de secretos.
- `tests/AulaPedidos.ArchitectureTests/AulaPedidos.ArchitectureTests.csproj`
- Archivos mínimos de prueba dentro de
  `tests/AulaPedidos.ArchitectureTests/`.
- `tests/AulaPedidos.Api.IntegrationTests/AulaPedidos.Api.IntegrationTests.csproj`
- Archivos mínimos de infraestructura y prueba dentro de
  `tests/AulaPedidos.Api.IntegrationTests/`.

Cualquier archivo adicional requiere nueva síntesis y decisión humana.

## Criterios de aceptación

1. La solución contiene los cuatro proyectos de producción y los proyectos de
   pruebas aprobados.
2. Todos tienen como destino .NET 10.
3. Las referencias coinciden con el grafo permitido.
4. Domain no referencia otras capas ni tecnologías de transporte o
   persistencia.
5. Api utiliza Minimal APIs y es el punto de composición.
6. El host se crea sin errores de DI.
7. `GET /health` ofrece una respuesta satisfactoria sin detalles sensibles.
8. El documento OpenAPI está disponible y sólo refleja las rutas incluidas en
   esta ronda.
9. Existen pruebas automatizadas para:
   - fronteras entre capas;
   - health check;
   - disponibilidad de OpenAPI.
10. Restore, build y tests concluyen correctamente según evidencia real.
11. El diff no contiene lógica de negocio, secretos ni archivos fuera del
	alcance.

## Casos negativos y de borde

- Domain no referencia Application, Infrastructure ni Api.
- Application no referencia Infrastructure ni Api.
- Infrastructure no referencia Api.
- No existen controllers ni endpoints de negocio.
- No existen entidades, repositorios, DbContext o migraciones.
- No se registran servicios ficticios para aparentar DI.
- OpenAPI no contiene operaciones de catálogo, pedidos o autenticación.
- La API puede construirse aunque Infrastructure no tenga adaptadores reales.
- Una capa vacía no justifica tipos artificiales.
- Los archivos adicionales producidos por una plantilla no quedan autorizados
  automáticamente.
- No se modifica `CursoNETIA.slnx` sin una decisión humana expresa.

## Riesgos que A/B deben evaluar

1. Paquetes mínimos y compatibles con .NET 10 para OpenAPI y pruebas.
2. Forma de verificar el grafo de referencias sin sobrediseñar.
3. Forma de probar el host, health check y OpenAPI con el mínimo acoplamiento.
4. Archivos generados por las plantillas que deberían autorizarse o excluirse.
5. Posible convivencia entre `CursoNETIA.slnx` y `AulaPedidos.slnx`.
6. Riesgo de crear servicios ficticios sólo para demostrar DI.
7. Qué evidencia invalidaría la propuesta.

## Entregable solicitado a cada lector

Cada informe debe distinguir:

- hechos observados;
- supuestos;
- alternativa recomendada;
- alternativas descartadas y motivo;
- paquetes o archivos adicionales imprescindibles;
- riesgos;
- evidencia necesaria;
- preguntas pendientes;
- qué invalidaría su recomendación.

Ningún lector edita archivos ni afirma haber ejecutado comandos.

## Estado base pendiente

- Directorio: `C:\Users\Yael\Curso\CursoNetIA`
- Repositorio:
  `https://github.com/ychavez/CursoNetIA_`
- Rama informada: `master`
- Solución existente informada: `CursoNETIA.slnx`
- Visual Studio informado: Community 2026 `18.9.3`
- Destino solicitado: .NET 10
- Commit: pendiente de aportación humana.
- Diff local: pendiente de aportación humana.
- Archivos nuevos relevantes: pendientes de aportación humana.
- Existencia previa de archivos candidatos: pendiente de comprobación.

Hasta completar estos datos, esta versión no está congelada y las consultas A/B
no deben comenzar.