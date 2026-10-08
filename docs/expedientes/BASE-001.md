# Expediente BASE-001 — Esqueleto de AulaPedidos

## Control del expediente

- Identificador: `BASE-001`
- Fase actual: E — preparación del expediente
- Modalidad: consultas separadas coordinadas manualmente en Visual Studio
- Directorio de trabajo: `C:\Users\Yael\Curso\CursoNetIA`
- Repositorio: `https://github.com/ychavez/CursoNetIA_`
- Rama informada: `master`
- Solución actualmente informada por el entorno: `CursoNETIA.slnx`
- Paquete común: [BASE-001.paquete-comun.md](BASE-001.paquete-comun.md)
- Estado del paquete común: pendiente de completar y congelar
- Autorización de implementación: pendiente de decisión humana
- Cierre: pendiente
- Confirmación de guardado: pendiente

## Objetivo

Preparar una propuesta para crear el esqueleto compilable y verificable de
AulaPedidos sobre .NET 10, tomando las reglas de arquitectura de este
repositorio como referencia.

La ronda se limita a:

1. Solución .NET 10 de AulaPedidos.
2. Cuatro proyectos de producción:
   - `AulaPedidos.Domain`
   - `AulaPedidos.Application`
   - `AulaPedidos.Infrastructure`
   - `AulaPedidos.Api`
3. Referencias entre proyectos conforme a las fronteras documentadas.
4. Composición mediante inyección de dependencias.
5. API basada exclusivamente en Minimal APIs.
6. Documento OpenAPI expuesto por ASP.NET Core.
7. Health check básico.
8. Proyectos de pruebas para arquitectura/unidad e integración de la API.
9. Pruebas mínimas del esqueleto y de sus fronteras observables.

No se incorporará lógica de catálogo, pedidos, usuarios, persistencia,
autenticación, autorización ni otras reglas de negocio.

## Fuentes y procedencia

- Requisitos de alcance: solicitud humana de la ronda `BASE-001`.
- Destino .NET: contexto del espacio de trabajo y solicitud humana.
- Fronteras entre capas: `docs/arquitectura.md`.
- Reglas de trabajo y calidad: `.github/copilot-instructions.md`.
- Proceso de consulta: `docs/multiagente-copilot.md`.
- Consideraciones de seguridad: `docs/seguridad.md`.
- Estado del repositorio: parcialmente informado por el entorno; faltan commit,
  diff y archivos nuevos.

## Asignación propuesta de consultas

- Informe A: Arquitecto.
- Informe B: Pruebas.
- Sesiones: dos chats nuevos y separados, abiertos manualmente por la persona
  responsable en Visual Studio.
- Entrada de ambos: únicamente el paquete común congelado y los archivos de
  contexto permitidos.
- Independencia: ninguno recibe el informe del otro.
- Presupuesto: una consulta inicial A/B y, como máximo, una aclaración.
- Tiempo de referencia: 10 minutos para análisis y síntesis, ajustable por la
  persona responsable antes de iniciar.

Los informes A/B están pendientes y no se simulan en este expediente.

## Alcance candidato

### Estructura de producción

- `src/AulaPedidos.Domain`
- `src/AulaPedidos.Application`
- `src/AulaPedidos.Infrastructure`
- `src/AulaPedidos.Api`

### Estructura de pruebas

- `tests/AulaPedidos.ArchitectureTests`
- `tests/AulaPedidos.Api.IntegrationTests`

### Dependencias de compilación permitidas

- `AulaPedidos.Domain` no referencia ningún otro proyecto de la solución.
- `AulaPedidos.Application` referencia `AulaPedidos.Domain`.
- `AulaPedidos.Infrastructure` referencia:
  - `AulaPedidos.Application`
  - `AulaPedidos.Domain`
- `AulaPedidos.Api` referencia:
  - `AulaPedidos.Application`
  - `AulaPedidos.Infrastructure`
- Las pruebas referencian únicamente los proyectos necesarios para sus casos.
- No se permiten referencias que inviertan estas fronteras.

### Inyección de dependencias

La composición se realiza desde `AulaPedidos.Api`. Al no existir todavía casos
de uso ni adaptadores reales, no se crearán interfaces o servicios ficticios
sólo para demostrar el contenedor.

Si A/B recomiendan métodos de registro por capa, deberán justificar los tipos
reales que registrarían y cualquier dependencia adicional necesaria.

### Superficie HTTP

- Uso exclusivo de Minimal APIs.
- Endpoint de health check, candidato: `GET /health`.
- OpenAPI habilitado mediante las capacidades de ASP.NET Core para .NET 10.
- No se crean endpoints de catálogo, pedidos, autenticación o negocio.
- No se añade Swagger UI salvo decisión humana posterior y justificación
  separada; publicar el documento OpenAPI satisface esta ronda.

## Exclusiones

Quedan fuera de `BASE-001`:

- Entidades, value objects, agregados y eventos de dominio.
- Casos de uso, comandos, consultas, handlers, DTOs de negocio y mediador.
- EF Core, DbContext, migraciones, repositorios y bases de datos.
- Autenticación, autorización, JWT, roles y permisos.
- Catálogo, pedidos, usuarios y notificaciones.
- Outbox, caché, reintentos y circuit breaker.
- Docker, despliegue, CI/CD y configuración de producción.
- Secretos, credenciales y datos personales.
- Herramientas auxiliares.
- ADR, bitácoras y demás documentación técnica.
- Paquetes, capas o abstracciones no indispensables para el esqueleto aprobado.
- Renombrar o eliminar la solución existente sin decisión humana expresa.
- Cualquier lógica ficticia destinada únicamente a hacer pasar pruebas.

## Archivos candidatos permitidos

La lista deberá validarse contra el estado exacto antes de autorizar la
implementación:

- `AulaPedidos.slnx`
- `src/AulaPedidos.Domain/AulaPedidos.Domain.csproj`
- `src/AulaPedidos.Application/AulaPedidos.Application.csproj`
- `src/AulaPedidos.Infrastructure/AulaPedidos.Infrastructure.csproj`
- `src/AulaPedidos.Api/AulaPedidos.Api.csproj`
- `src/AulaPedidos.Api/Program.cs`
- `src/AulaPedidos.Api/Properties/launchSettings.json`, sólo si lo genera o
  necesita la plantilla aprobada y no contiene secretos.
- `tests/AulaPedidos.ArchitectureTests/AulaPedidos.ArchitectureTests.csproj`
- Archivos de prueba mínimos dentro de
  `tests/AulaPedidos.ArchitectureTests/`.
- `tests/AulaPedidos.Api.IntegrationTests/AulaPedidos.Api.IntegrationTests.csproj`
- Archivos de infraestructura y prueba mínimos dentro de
  `tests/AulaPedidos.Api.IntegrationTests/`.

No están autorizados archivos fuera de esta lista. Cualquier necesidad de
`Directory.Build.props`, administración central de paquetes, configuración,
documentación u otro archivo vuelve a síntesis y decisión humana.

## Criterios de aceptación candidatos

### Positivos

1. `AulaPedidos.slnx` contiene exactamente los cuatro proyectos de producción y
   los proyectos de pruebas aprobados.
2. Todos los proyectos tienen como destino .NET 10.
3. Las referencias de compilación respetan la dirección documentada.
4. `AulaPedidos.Domain` permanece independiente de Application,
   Infrastructure, HTTP y persistencia.
5. `AulaPedidos.Api` usa Minimal APIs y actúa como punto de composición.
6. La aplicación inicia con el contenedor de dependencias válido.
7. `GET /health` responde satisfactoriamente cuando el proceso está saludable.
8. El documento OpenAPI puede obtenerse en la ruta configurada y representa la
   superficie HTTP incluida en la ronda.
9. Las pruebas automatizadas verifican al menos:
   - fronteras de referencias entre capas;
   - respuesta satisfactoria de health check;
   - disponibilidad del documento OpenAPI.
10. Restore, build y tests terminan correctamente sobre el estado implementado,
	según salidas reales aportadas por la persona que los ejecute.
11. El diff final no contiene secretos ni lógica de negocio.

### Negativos

1. No aparecen referencias desde Domain hacia otra capa.
2. Application no referencia Infrastructure ni Api.
3. Infrastructure no referencia Api.
4. No existen controllers MVC ni endpoints de negocio.
5. No existen entidades, repositorios, DbContext, migraciones ni datos de
   ejemplo.
6. No se registran servicios ficticios para aparentar uso de DI.
7. La respuesta del health check no expone excepciones, configuración sensible
   ni detalles internos.
8. OpenAPI no incorpora rutas de negocio fuera del alcance.
9. No se modifica ni elimina `CursoNETIA.slnx` sin autorización expresa.
10. No se añaden paquetes o archivos fuera del alcance aprobado.

### Borde

1. La API debe poder construirse sin Infrastructure contener implementaciones
   reales.
2. Un proyecto vacío de Domain no justifica introducir tipos de dominio
   artificiales.
3. La selección de rutas de OpenAPI y health check debe evitar colisiones y
   quedar comprobada en pruebas.
4. Si la plantilla genera archivos adicionales, éstos no se aceptan
   automáticamente: deben incorporarse al alcance mediante decisión humana.

## Matriz preliminar para el rol Pruebas

| Caso | Resultado esperado | Evidencia pendiente |
|---|---|---|
| Grafo de referencias | Coincide con las dependencias permitidas | Inspección de proyectos y prueba de arquitectura |
| Referencia prohibida desde Domain | Ausente | Prueba de arquitectura |
| Inicio de la API | Host creado sin errores de DI | Prueba de integración |
| `GET /health` | Respuesta HTTP satisfactoria | Prueba de integración |
| Documento OpenAPI | Respuesta satisfactoria y documento válido | Prueba de integración |
| Ruta de negocio no incluida | No existe | Inspección de OpenAPI |
| Compilación | Sin errores | Salida real aportada por quien ejecute |
| Suite de pruebas | Sin fallos | Salida real aportada por quien ejecute |
| Secretos | Ninguno en el diff | Revisión del diff |

Pruebas deberá revisar esta matriz desde los requisitos antes de que se autorice
la implementación.

## Riesgos iniciales

1. Añadir tipos ficticios sólo para demostrar DI o referencias.
2. Introducir paquetes innecesarios mediante plantillas.
3. Confundir publicación de OpenAPI con la inclusión obligatoria de una UI.
4. Romper la solución existente al crear o renombrar la nueva solución.
5. Generar archivos adicionales fuera de la lista autorizada.
6. Probar únicamente que compila, sin comprobar referencias ni superficie HTTP.
7. Usar una ruta o API obsoleta para OpenAPI en .NET 10.
8. Declarar resultados de build o tests sin ejecución real.
9. Congelar A/B sobre un commit o diff no identificado.

## Estado base

- Directorio: `C:\Users\Yael\Curso\CursoNetIA`
- Repositorio activo informado:
  `https://github.com/ychavez/CursoNetIA_`
- Rama informada: `master`
- Solución existente informada:
  `C:\Users\Yael\Curso\CursoNetIA\CursoNETIA.slnx`
- Entorno informado: Visual Studio Community 2026 `18.9.3`
- Destino solicitado: .NET 10
- Commit: pendiente de aportación humana.
- Cambios locales: pendientes de aportación humana.
- Archivos nuevos relevantes: pendientes de aportación humana.
- Existencia previa de los archivos candidatos: pendiente de comprobación.

Este estado incompleto bloquea la congelación del paquete y el inicio de A/B.

## Condiciones de parada

La ronda vuelve a decisión humana si:

- cambia el estado base;
- se necesita modificar un archivo no permitido;
- se requiere lógica de negocio;
- aparece una dependencia o paquete no justificado;
- se pretende renombrar o retirar la solución existente;
- A/B trabajan sobre paquetes diferentes;
- falta uno de los informes;
- se agota el presupuesto;
- no existe evidencia suficiente para un criterio.

## Estado del proceso

- Preparación del expediente: en curso.
- Estado exacto: pendiente.
- Paquete común congelado: no.
- Informe A: pendiente.
- Informe B: pendiente.
- Síntesis: pendiente.
- Decisión humana de implementación: pendiente.
- Implementador único: no designado.
- Implementación: no iniciada.
- Verificación: pendiente.
- Decisión humana de cierre: pendiente.