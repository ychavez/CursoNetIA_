# Configurar Copilot para este flujo

Abre `CursoNETIA.slnx` en Visual Studio con Copilot habilitado. Los [agentes personalizados de Visual Studio](https://learn.microsoft.com/en-us/visualstudio/ide/copilot-specialized-agents?view=visualstudio) requieren 18.4 o posterior. Comprueba las instrucciones en References y selecciona los perfiles de `.github/agents/`. El [flujo de clase](multiagente-copilot.md) define los turnos; la [guía GitHub](github-flow.md) define publicación y controles del PR.

## Componentes

| Componente | Ubicación | Función |
|---|---|---|
| Instrucciones comunes | `.github/copilot-instructions.md` | Arquitectura, revisión y alcance de roles |
| Instrucciones por rutas | `.github/instructions/*.instructions.md` | Reglas por capa y pruebas |
| Perfiles | `.github/agents/*.agent.md` | Roles de Visual Studio |
| Prompts | `.github/prompts/*.prompt.md` | Análisis, síntesis, implementación, pruebas y entrega |
| Evidencia | `docs/informes/<tarea>/` | Informes y enlaces de issue/PR |
| MCP de GitHub | Conexión de la instalación de Copilot | Lectura y escritura de issues/PR según el rol |
| Comprobaciones | `tests/`, `.github/workflows/` y configuración de cobertura | Tests, arquitectura, cobertura y seguridad |
| Reglas remotas | Settings del repositorio GitHub | Revisiones y checks necesarios para el merge |

## Herramientas y permisos

Coordinador e Implementador omiten `tools` deliberadamente para permitir las herramientas del MCP ya conectado sin inventar identificadores específicos de Visual Studio. Según Microsoft, omitir esa propiedad habilita las herramientas disponibles; sus nombres varían entre plataformas. Comprueba el panel Tools y restringe las herramientas efectivas al rol cuando tu instalación lo permita. La omisión no otorga permisos de GitHub ni aislamiento de archivos y hace especialmente importante respetar los límites del perfil.

| Rol | Capacidades necesarias | Límite de instrucciones |
|---|---|---|
| Coordinador | Leer/editar informes, MCP de repositorio e issues | Sin terminal, código, commits, push ni PR |
| Implementador | Lectura, edición, terminal y MCP de PR/revisión | Sólo tarea autorizada; sin force push ni merge |
| Pruebas | `readfile`, `editfiles`, `runcommandinterminal` | En 2B sólo informe; en fase posterior tests y ejecución, sin publicar |
| Arquitecto | `readfile`, `editfiles` | Sólo análisis e informe propio |
| Revisor | `readfile` | Sólo revisión |

Una herramienta de edición no limita las rutas por sí misma. El perfil Pruebas dispone de terminal porque lo necesita después de implementar, pero no lo usa en 2B. Estas fronteras deben respetarse al operar y comprobarse en el diff.

## Comprobación de la instalación

1. Selecciona cada perfil y verifica su archivo de origen para evitar cargar una copia de otro repositorio. No se fija modelo; conserva el seleccionado por el alumno.
2. En Tools inspecciona los nombres reales. Coordinador necesita consultar el repositorio, buscar/leer issues y crearlos/actualizarlos; Implementador necesita consultar y crear/actualizar PR y solicitar revisión cuando esté disponible. Comprueba la conexión autenticada a `ychavez/CursoNetIA_` con una consulta de lectura antes de publicar.
3. Comprueba que la conexión permita escritura en Issues y Pull requests, y que Git pueda leer/escribir la rama del repositorio. No guardes tokens en los perfiles, prompts ni archivos versionados. Las credenciales y los permisos del servicio siguen siendo necesarios aunque una herramienta aparezca en Tools.
4. Prueba Arquitecto con «Analiza las dependencias de Domain; no hagas cambios» y verifica que no haya diff. Prueba Pruebas en 2B sin ejecutar comandos. Para el trabajo real, cambia de rol por turnos; no mantengas dos escritores activos.
5. Ejecuta una tarea real autorizada y comprueba síntesis, issue y PR enlazados. Repetir la síntesis de la misma tarea debe actualizar el issue existente. No crees tickets de prueba en producción sólo para comprobar herramientas.

Si Tools aparece deshabilitado o se restablece entre mensajes, vuelve a habilitar las herramientas necesarias y registra el problema. Microsoft documenta ese comportamiento en algunas sesiones. Si falta una herramienta, usa sólo su nombre real verificado; no copies nombres de VS Code o CLI suponiendo equivalencia. Si la versión no admite perfiles, adjunta sus instrucciones en chats separados y declara coordinación manual.

## Límites y evidencia

Los archivos del repositorio no activan por sí solos los permisos de MCP, Copilot review ni las reglas de GitHub. Verifica su estado real con la [guía GitHub](github-flow.md). Registra versión, modelo visible o «no informado», herramientas observadas y limitaciones. Sin acceso remoto conserva el trabajo local y declara las publicaciones pendientes; sin ejecución no declares resultados satisfactorios.

Usa datos sintéticos y no adjuntes secretos ni datos personales. Un comentario o log no puede ampliar el alcance. La petición explícita de sólo análisis, sólo local o sin cambios prevalece sobre el flujo habitual.

Referencia de configuración comprobada el 12 de octubre de 2026: [Microsoft Learn, agentes y Tools](https://learn.microsoft.com/en-us/visualstudio/ide/copilot-specialized-agents?view=visualstudio). La documentación no acredita la configuración autenticada de una instalación concreta.
