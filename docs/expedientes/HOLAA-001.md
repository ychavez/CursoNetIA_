# HOLAA-001: saludo mínimo de la API

## Objetivo

Añadir a la API existente un endpoint HTTP mínimo en la ruta raíz (`/`) que devuelva exactamente `Hello, World!`, sin modificar la arquitectura, los paquetes ni las capas no relacionadas.

## Archivos consultados

- `src/AulaPedidos.Api/Program.cs`
- `src/AulaPedidos.Api/AulaPedidos.Api.http`
- `tests/AulaPedidos.Api.IntegrationTests/ApiTests.cs`
- `tests/AulaPedidos.Api.IntegrationTests/AulaPedidos.Api.IntegrationTests.csproj`
- `tests/AulaPedidos.ArchitectureTests/ArchitectureTests.cs`
- `.github/instructions/api.instructions.md`
- `.github/instructions/csharp.instructions.md`
- `.github/instructions/tests.instructions.md`

## Archivos modificados

- `src/AulaPedidos.Api/Program.cs`
- `src/AulaPedidos.Api/AulaPedidos.Api.http`
- `tests/AulaPedidos.Api.IntegrationTests/ApiTests.cs`
- `tests/AulaPedidos.ArchitectureTests/ArchitectureTests.cs`
- `docs/expedientes/HOLAA-001.md`

## Alcance

- Se añadió `GET /` mediante Minimal API.
- Se conservó el endpoint existente `GET /health`.
- Se actualizó el archivo `.http` para solicitar la ruta raíz.
- Se añadió una prueba de integración para estado HTTP, tipo de contenido y cuerpo exacto.
- Se actualizó la comprobación de endpoints para contemplar `/` y seguir verificando `/health` y su ausencia de autorización.
- No se añadieron paquetes, entidades, lógica de negocio ni cambios en otras capas.

## Criterios de aceptación

1. `GET /` responde con estado `200 OK`.
2. El cuerpo de `GET /` es exactamente `Hello, World!`.
3. El endpoint raíz usa la ruta `/`, resolviendo la ambigüedad frente a una ruta como `/hello`.
4. Las comprobaciones existentes de salud, OpenAPI, composición y endpoints no se debilitan.
5. La solución y las pruebas existentes continúan compilando y pasando.

## Comprobaciones reales

- `dotnet test tests/AulaPedidos.Api.IntegrationTests/AulaPedidos.Api.IntegrationTests.csproj`: pasó inicialmente tras restaurar dependencias, con 12 pruebas correctas.
- `dotnet test tests/AulaPedidos.Api.IntegrationTests/AulaPedidos.Api.IntegrationTests.csproj --no-restore`: pasó de nuevo tras ajustar la nulabilidad de la aserción, con 12 pruebas correctas.
- `dotnet build CursoNETIA.slnx --no-restore`: no pudo ejecutarse porque faltaban assets de los proyectos de pruebas de arquitectura e infraestructura.
- `dotnet build CursoNETIA.slnx`: pasó tras restaurar dependencias, sin advertencias ni errores.
- `dotnet test CursoNETIA.slnx --no-restore`: pasó después de actualizar la aserción del archivo `.http`: 46 pruebas correctas en los tres proyectos, sin omitidas ni fallos.

## Estado

Completado y validado.

## Pendientes

Ninguno para el alcance de este expediente.
