# AulaPedidos: instrucciones del proyecto

Trabaja en español y usa nombres de código en inglés. Proyecto .NET 10, Minimal APIs, solución CursoNETIA.slnx.

## Arquitectura y revisión de PR
- Domain: entidades y reglas sin dependencias de EF, HTTP ni otras capas.
- Application: casos de uso y contratos; depende de Domain.
- Infrastructure: implementaciones técnicas; depende de Application y, cuando corresponda, Domain.
- Api: composición y transporte mediante Minimal APIs; referencia Application e Infrastructure. Mantén reglas de negocio fuera de endpoints.
- Consulta las instrucciones por ruta en .github/instructions y el código actual antes de recomendar un patrón. Repository y la biblioteca Mediator de martinothamar están solicitados en BASE-001; no usar MediatR ni un mediador propio. Conserva los límites de Repository y Unit of Work cuando correspondan al alcance.
- Revisa errores de comportamiento, referencias invertidas, contratos rotos, autorización, validación, inyección, exposición de secretos y regresiones. Distingue defectos confirmados de hipótesis y preferencias; para cada hallazgo indica ubicación, impacto y corrección concreta.
- Usa async/await y CancellationToken para I/O; evita .Result y .Wait(). No añadas paquetes/patrones sin necesidad. Las utilidades del proyecto se escriben en C#, no en scripts PowerShell.
- Exige pruebas útiles derivadas de requisitos y casos negativos/borde. Los checks de Actions ejecutan tests, arquitectura, cobertura y seguridad. Copilot revisa; sus comentarios no acreditan ejecución ni sustituyen checks obligatorios. No reduzcas controles para conseguir verde.

## Flujo GitHub
- Sigue docs/github-flow.md y docs/multiagente-copilot.md. Para las tareas del flujo, el Coordinador guarda 3-sintesis.md y crea/actualiza su issue; el Implementador publica un único PR vinculado tras código y pruebas. Sin merge automático; la decisión de merge pertenece al humano.
- Una petición de implementar incluye la entrega por este flujo. Respeta las restricciones explícitas de sólo local o sin publicación. Una pregunta sin cambios o sólo análisis no autoriza publicar; si se prohíben cambios, tampoco guardes informes.
- Verifica remoto/repositorio y base antes de publicar (actualmente ychavez/CursoNetIA_, master). Usa rama codex/<tarea>-<descripcion>. Preserva trabajo ajeno, sin force push ni commits a la rama protegida.
- Issue idempotente: guarda su número/URL y la clave `<!-- task:<owner>/<repo>:<tarea> -->`; verifica enlazados y abiertos/cerrados antes de crear. Ante resultado incierto verifica antes de reintentar. PR idempotente por tarea, rama y base; incluye Closes #<issue>.
- Sólo el Implementador hace commit/push/PR. Pruebas puede escribir y ejecutar tests después de la implementación, por turnos; nunca hay escritores simultáneos. El Coordinador sólo edita documentación de coordinación y gestiona issues; no terminal ni código.
- No publiques secretos, logs sensibles ni conversaciones completas. No elimines datos compartidos, cambies credenciales ni amplíes el alcance sin petición. Si falta acceso, conserva lo local e informa el paso pendiente; no inventes URLs ni resultados.

## Trabajo local e informes
- Atiende la petición directamente. No son requisitos un expediente, commit aportado, diff previo, A/B, síntesis, congelación ni aprobación de guardado. Los expedientes y docs/harness-avanzado son contexto histórico/opcional.
- Lee los archivos disponibles y pregunta sólo por un dato material que no puedas obtener del proyecto. No simules otros agentes: el alumno selecciona los roles en Visual Studio.
- Cuando se use 2A/2B: Arquitecto guarda docs/informes/<tarea>/2A-arquitectura.md; Pruebas guarda 2B-pruebas.md. Forman sus propuestas sin leer el informe contrario. El Coordinador lee ambos y guarda 3-sintesis.md; si falta uno, declara la ausencia y sintetiza lo disponible.
- En 2A/2B sólo se edita el informe propio, sin comandos ni cambios de código/tests. En la fase posterior, Pruebas escribe tests y evidencia en 4-pruebas.md; el Implementador conserva la entrega en 4-entrega.md.
- Usa la tarea indicada; sin identificador usa una carpeta descriptiva estable. Antes de guardar comprueba el objetivo: conserva decisiones/aportes ajenos y usa otra carpeta si es otra tarea. No sobrescribas informes originales de otros roles ni reutilices BASE-001/BASE-002 para un objetivo nuevo. Respeta las mayúsculas/minúsculas de los archivos existentes.
- Cada informe identifica objetivo, archivos consultados, conclusiones, supuestos, evidencia real y pendientes. Distingue propuesta de decisión humana. Si no puedes guardar, entrega texto y ruta y declara que no se guardó.
- Conserva CursoNETIA.slnx. La documentación de la aplicación completa es un objetivo, no prueba de capacidades implementadas.

## Comprobación
Compila con dotnet build CursoNETIA.slnx y ejecuta dotnet test CursoNETIA.slnx. Para cobertura y controles de PR sigue docs/github-flow.md. Para la API: dotnet run --project src/AulaPedidos.Api. Reporta comandos y resultados realmente observados, estado analizado y pendientes.

## Estado de la base
BASE-001 es histórico. BASE-002 incorporó Repository y Unit of Work con EF Core/SQLite; consulta docs/informes/BASE-002/3-sintesis.md, su informe 3-implementracion.md y el código actual. Las restricciones de ausencia de persistencia de BASE-001 ya no describen el estado vigente. No añadas entidades ni negocio si la tarea no los solicita.
