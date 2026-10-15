# TAREA-004 — Implementación de validación en Mediator

- Rol: Implementador. Fecha: 2026-10-14.
- Objetivo: ADR 006, FluentValidation antes de handlers y traducción centralizada a HTTP 400.
- Issue verificado: [#20](https://github.com/ychavez/CursoNetIA_/issues/20).
- Draft PR real: [#21](https://github.com/ychavez/CursoNetIA_/pull/21).
- Rama publicada mediante GitHub MCP: `codex/tarea-004-validacion-mediator`.
- Estado: integración técnica implementada y verificada; V12 (reglas productivas) pendiente, sin inventar reglas. No constituye aceptación humana.

## Contexto y conservación del trabajo previo

Leídos: instrucciones generales y de C#/Application/Api/tests; convención e índice; flujo multiagente y guía de PR; ADR 006; coordinación, arquitectura, propuesta de pruebas y síntesis de TAREA-004; proyectos, composición, mensajes, Domain, pruebas e inventario arquitectónico; `docs/products.md`; plantilla de PR y workflows. Consultados el issue #20, ramas, PR existentes, diff remoto, checks y revisión real de Copilot mediante MCP.

El remoto es `ychavez/CursoNetIA_`; su rama predeterminada verificada es `master`. El CRUD actual corresponde al PR abierto #19 y aún no está en master. Se creó **un PR apilado sobre `codex/tarea-003-crud-precio-descripciones`**, commit base `c8e0066babc018011935212feec258bc19131680`, para no atribuir el CRUD a esta tarea. La rama se creó mediante MCP y se conserva durante toda la tarea. Tras integrar #19, cualquier cambio de base de #21 requiere intervención humana; no se ha cambiado la base de un PR existente.

El workspace estaba en `master`, con un commit local adicional, cambios previos en el índice, ejemplos HTTP, espacios de endpoints/handler y documentos locales de TAREA-004. No se cambió la rama local, no se creó commit local ni se hizo stash/reset. Se conservaron esos cambios. No se publicaron el ZIP, binarios, bases, artefactos ni modificaciones ajenas de endpoints, handler o ejemplos. Los informes 01–04 se leyeron y conservaron, sin editarlos ni publicarlos en nombre de otros roles. Se publica el registro y el ADR existentes de esta tarea como contexto, sin cambiar su contenido.

## Cambios y archivos

1. `src/AulaPedidos.Application/AulaPedidos.Application.csproj`: FluentValidation **12.1.1**, compatible con net10.0 mediante net8.0; licencia Apache-2.0 según el paquete restaurado. Sin paquete de escaneo ni cambios de Mediator 3.0.2.
2. `src/AulaPedidos.Application/Validation/ValidationBehavior.cs`: `IPipelineBehavior<TMessage,TResponse>` y firma oficial de Mediator; validación asíncrona secuencial, token conservado, todos los fallos acumulados y `ValidationException` antes del handler. Comprobación de cancelación al entrar, antes de cada validador y antes de continuar. Sin capturar fallos técnicos. Colección de fallos local a cada envío.
3. `src/AulaPedidos.Api/Program.cs`: `options.PipelineBehaviors = [typeof(ValidationBehavior<,>)]` y registro de `ValidationExceptionHandler`. Mantiene Mediator scoped y el middleware existente.
4. `src/AulaPedidos.Api/ValidationExceptionHandler.cs`: sólo maneja `ValidationException`; devuelve `ValidationProblemDetails` con 400 y errores por campo sin descartar mensajes. Reutiliza `IProblemDetailsService`; fallback JSON explícito si Accept no admite su escritor. No serializa excepción, valores intentados ni estado personalizado. Cancelaciones y errores técnicos quedan sin manejar por este traductor.
5. `tests/AulaPedidos.Api.IntegrationTests/ValidationBehaviorTests.cs`: 8 casos aislados de continuación, errores acumulados, secuencialidad determinista, token, cancelación y fallos inesperados.
6. `tests/AulaPedidos.Api.IntegrationTests/ValidationApiTests.cs`: 6 casos de integración/traductor: JSON válido rechazado por validators, múltiples errores, comandos/consultas con ISender real, scopes, confidencialidad en Production, fallo técnico 500 y ausencia de efectos en alta/edición. También prueba Accept XML sin perder el contrato JSON. SQLite efímero en memoria, sin tocar la base del usuario.
7. `tests/AulaPedidos.ArchitectureTests/ArchitectureTests.cs`: actualiza únicamente los inventarios autorizados por ADR 006 y refuerza la prohibición de FluentValidation en Domain. No elimina restricciones de capas.
8. `docs/validation.md`: composición, registro explícito de futuros validadores, seguridad de mensajes, contrato y límites.
9. Este informe, junto con el índice y el ADR preexistentes de la tarea, completa la documentación publicada.

## Reglas productivas: decisión basada en el contrato existente

Inventariados CreateProductCommand, UpdateProductCommand, DeleteProductCommand, GetProductByIdQuery y GetProductsQuery. `docs/products.md` aprueba precios negativos/cero, textos opcionales sin trim ni límites, y 404 para GUID vacío. Domain protege la identidad al construir Product, pero ello no autoriza cambiar el contrato de consulta a 400. La presencia y representación decimal de price ya se verifican por binding HTTP.

Por tanto **no se añaden validadores productivos ni registros vacíos de reglas inventadas**. Los validadores de prueba se registran sólo en la fixture sobre mensajes reales; la fixture no registra el behavior ni sustituye ISender. Para futuras reglas aprobadas se documenta registro explícito scoped en AddApplication. V12 sigue pendiente de requisitos funcionales verificables; los dobles acreditan el mecanismo, no una validación comercial completa. No hay cambios en Domain, EF, migraciones ni contratos del CRUD.

## Comandos ejecutados y resultados

Todos ejecutados desde la raíz; los comandos siguientes no son sugerencias:

| Comprobación | Resultado real y evidencia local |
|---|---|
| `dotnet build CursoNETIA.slnx --configuration Release --nologo -warnaserror` | Correcto: 0 errores, 0 advertencias. `artifacts/tarea-004-build.log`. |
| `dotnet test CursoNETIA.slnx --configuration Release --no-build --no-restore --logger trx --results-directory TestResults/tarea-004-verified` | **81/81**: arquitectura 25, infraestructura 14, API 42; ninguno omitido. `artifacts/tarea-004-tests-verified.log`, TRX en esa carpeta. |
| `dotnet test tests/AulaPedidos.Api.IntegrationTests/AulaPedidos.Api.IntegrationTests.csproj --configuration Release --no-build --no-restore --collect:"XPlat Code Coverage" --logger trx --results-directory TestResults/tarea-004-coverage-api` | 42/42; reporte original del collector conservado. |
| Mismo comando de cobertura para `tests/AulaPedidos.Infrastructure.Tests/AulaPedidos.Infrastructure.Tests.csproj`, destino `TestResults/tarea-004-coverage-infrastructure` | 14/14; reporte original conservado. |
| `dotnet tool restore` y dos ejecuciones de `dotnet tool run reportgenerator` con ambos reportes anteriores | Reporte combinado bruto y reporte mantenido; sólo el segundo añade `-filefilters:-**/obj/**`. Resultados descritos abajo. |
| `dotnet list CursoNETIA.slnx package --vulnerable --include-transitive` | Ningún paquete vulnerable comunicado en los siete proyectos por los orígenes consultados. `artifacts/tarea-004-audit.log`. No acredita ausencia de alertas Dependabot. |
| Cotejo de los 8 archivos técnicos publicados con el contenido local usando Git fetch/show y UTF-8 | Coinciden, normalizando sólo BOM/finales de línea; no se publicaron cambios ajenos. Diff remoto revisado antes de abrir el PR. `git diff --check` entre base y rama publicada, sin incidencias. |

ReportGenerator 5.5.11 combinó ambos reportes reales, sin sumar porcentajes ni duplicar las copias In/ del collector:

- Código mantenido: **97,0 % (231/238 líneas)**, 4 ensamblados; `artifacts/tarea-004-coverage/Cobertura.xml` y `SummaryGithub.md`. Supera 80 %, sin excluir código mantenido.
- Sin filtro, incluyendo generado: **54,5 % (453/831 líneas)**; `artifacts/tarea-004-coverage-raw/Cobertura.xml` y `SummaryGithub.md`, conservados junto al anterior.
- Los artefactos locales no se suben como archivos del PR; CI publica sus propios TRX y reportes mediante el workflow. No se afirma haber descargado el artefacto remoto.

### Evidencia previa y correcciones

El primer incremento `50f0a96` compiló, pero fallaron dos pruebas locales de inventario arquitectónico; el check remoto también falló. Se conservan `artifacts/tarea-004-tests.log` y la evidencia [del primer check](https://github.com/ychavez/CursoNetIA_/actions/runs/38656525687/job/116167494536). Se actualizaron explícitamente los inventarios y se repitieron todas las pruebas: no se silenciaron diagnósticos ni redujeron umbrales.

La primera ejecución global con collector produjo aviso de collector ausente en arquitectura (ese proyecto no lo referencia). Después se ejecutó la solución completa sin collector y se recopiló cobertura por separado en los dos proyectos configurados, igual que CI. No se presenta el aviso anterior como prueba de cobertura de arquitectura.

Copilot se solicitó realmente sobre `50f0a96`, sin solicitud automática detectada. Su [revisión](https://github.com/ychavez/CursoNetIA_/pull/21#pullrequestreview-5526244169) señaló tres asuntos: inventarios arquitectónicos, integración HTTP y pruebas del behavior. Los tres están atendidos por los archivos publicados en `0133636` y `82a1a59`, con 81 pruebas locales correctas y CI correcto sobre `82a1a59`. Los hilos seguían abiertos al consultar: corregir código no equivale a resolverlos ni a recibir aprobación. Se solicitará revisión del HEAD documental final si no existe una automática para él.

## Controles de publicación

Instantánea sobre el último commit técnico comprobado **`82a1a59e7a632fa79aadd43154be8fb6a6543c4b`**. La siguiente publicación añade sólo contexto/informe: sus nuevos checks y revisión no quedan acreditados por resultados de commits anteriores. La descripción de #21 recoge la consulta posterior del HEAD vigente.

| Control | Estado | Commit y evidencia |
|---|---|---|
| Compilación y analizadores | correcto | `82a1a59`; [CI](https://github.com/ychavez/CursoNetIA_/actions/runs/38657001343/job/116169017275), build local con warnaserror: 0 advertencias. |
| Pruebas | correcto | `82a1a59`; mismo CI y 81/81 locales. |
| Cobertura combinada mantenida ≥80 %, más reporte bruto | correcto | `82a1a59`; mismo check CI correcto; medición local 97,0 %, bruto 54,5 %. Artefactos remotos no descargados. |
| NuGet Audit | correcto | `82a1a59`; [check](https://github.com/ychavez/CursoNetIA_/actions/runs/38657001343/job/116169017508) y auditoría local sin vulnerables comunicados. |
| Dependency Review | correcto | `82a1a59`; [check](https://github.com/ychavez/CursoNetIA_/actions/runs/38657001331/job/116169267531). |
| CodeQL | en curso | `82a1a59`; [check](https://github.com/ychavez/CursoNetIA_/actions/runs/38657001331/job/116169267887). Alertas abiertas no consultables con las herramientas expuestas. |
| Secret scanning / push protection / Dependabot: consulta de alertas y configuración | no disponible | Sin herramientas MCP expuestas para comprobar permisos, licencias, configuración o alertas. No implica que estén deshabilitados. [GitGuardian correcto](https://github.com/ychavez/CursoNetIA_/runs/116169011165) en `82a1a59` no los sustituye. |
| Copilot code review del nuevo HEAD | pendiente | Revisión real de `50f0a96` conservada; correcciones en `82a1a59` pendientes de reevaluación. No se repite solicitud del mismo commit. |
| Copilot Autofix / GitHub Code Quality | no disponible | Sin capacidades expuestas para comprobar disponibilidad/licencias o consultar resultados; no se activa ningún plan. |
| Checks del próximo commit documental | pendiente | Se consultarán tras publicar; no se atribuyen checks anteriores al nuevo HEAD. |

Los toolsets opcionales constan en `.vscode/settings.json`, pero no exponen aquí herramientas de alertas, Actions/artefactos o configuración de seguridad. No hubo un error de llamada a una herramienta ausente: se inspeccionaron los esquemas realmente ofrecidos. Tampoco hay una herramienta para refrescar la conexión/caché o cambiar el selector de VS Code; esa comprobación queda pendiente en el cliente. No se sustituyó MCP por gh/API directa ni se asumieron permisos.

## Pendientes y siguiente paso

- Confirmar reglas comerciales antes de implementar V12. No se inventa validación de precio, longitud, obligatoriedad, unicidad ni GUID vacío.
- Consultar checks del HEAD posterior a esta publicación y obtener la nueva revisión de Copilot; los tres hilos iniciales siguen abiertos a reevaluación.
- Verificar alertas, permisos y funciones de seguridad/calidad en un cliente que exponga esas capacidades; un check CodeQL correcto no significa cero alertas abiertas.
- Mantener #21 como **draft**, dependiente de #19. No se marcó listo, aprobó, habilitó auto-merge ni fusionó ningún PR. La aceptación final es humana.
- La cobertura técnica cubre V01–V11 y V13–V18 mediante pruebas nuevas y regresión existente; V12 permanece explícitamente pendiente. No se simulan informes de Pruebas/Revisor ni una aprobación de Copilot.

## Publicación en GitHub

Publicados mediante MCP el primer incremento verificable y las pruebas/inventarios en la misma rama y PR #21. Commits técnicos: `50f0a96` → `0133636` → `82a1a59`. Se preserva el número y [URL real del draft](https://github.com/ychavez/CursoNetIA_/pull/21). Este informe se publica como incremento documental; el diff y los checks actuales se consultan después, y su estado se registra en la descripción de #21. No se publica un comentario de otro rol ni se modifica el issue para simular cierre.

## Seguimiento: CodeQL y cambio remoto concurrente

Tras publicar el informe en `b13ab0e`, el usuario aplicó una sugerencia real de **Copilot Autofix**, commit [64f1eab](https://github.com/ychavez/CursoNetIA_/commit/64f1eabc8d66f4c971b63d8c7bbc138a106b55ab). Se inspeccionó antes de volver a publicar. Se conserva su intención de eliminar un cast mediante un CreateProductCommand como cuerpo JSON tipado, sin depender de un null sin tipo en un anónimo. Se comprobó la disponibilidad de una propuesta aplicada de Autofix, no permisos para invocarlo desde esta sesión.

CodeQL añadió dos hallazgos reales en el PR:
- [Alerta 32: Useless upcast](https://github.com/ychavez/CursoNetIA_/security/code-scanning/32): se elimina el cast con el mensaje tipado. Su hilo ya figuraba resuelto por la intervención remota; no se atribuye esa resolución al Implementador.
- [Alerta 31: Missing Dispose](https://github.com/ychavez/CursoNetIA_/security/code-scanning/31): ValidationFactory ahora deriva de WebApplicationFactory y configura el host mediante override, sin crear una factory base que se pierda. Sus consumidores conservan await using. El cierre de la alerta necesita el análisis posterior.
- Se conserva la [nota humana sobre un futuro patrón Result](https://github.com/ychavez/CursoNetIA_/pull/21#discussion_r4277552155) y la resolución humana del hilo HTTP. No se implementa ese cambio futuro ni se sustituye el contrato aceptado.

Verificación posterior ejecutada: build Release con -warnaserror, 0 errores/advertencias; solución completa **81/81** en `TestResults/tarea-004-codeql-fixes`, log `artifacts/tarea-004-tests-codeql-fixes.log`. Cobertura API repetida en `TestResults/tarea-004-codeql-coverage` (42/42), combinada con infraestructura sin cambios: **97,0 % (231/238)** mantenido y **54,5 % (453/831)** bruto en las rutas de artifacts anteriores. No se modificó código productivo tras su verificación.

Esta tabla reemplaza sólo el estado vigente; la anterior se conserva como instantánea histórica de 82a1a59, no como validación del nuevo HEAD:

| Control | Estado | Commit y evidencia |
|---|---|---|
| Compilación estricta y pruebas locales | correcto | Árbol que incorpora 64f1eab con ambas correcciones revisadas; 81/81 y logs/TRX anteriores. CI del nuevo HEAD pendiente. |
| Cobertura local combinada y reporte bruto | correcto | Mismo árbol: 97,0 % y 54,5 %, reportes reales conservados. |
| Build/tests/cobertura, NuGet Audit, Dependency Review y CodeQL remotos | pendiente | Se consultarán tras publicar; SHA y enlaces actuales en #21, sin reutilizar checks de 82a1a59 como actuales. |
| Alertas 31/32 | pendiente | Correcciones implementadas y probadas; falta confirmar el análisis y estado posterior. Hilo 32 resuelto por intervención remota; 31 seguía abierto. |
| Copilot code review del próximo HEAD | pendiente | Solicitar después del push si no existe una automática para ese commit. |
| Copilot Autofix | correcto | Propuesta aplicada constatada en 64f1eab, evaluada y adaptada; efectos comprobados con build/tests. No implica herramienta MCP para invocarlo. |
| Alertas completas, secret scanning/push protection, permisos/licencias y GitHub Code Quality | no disponible | Algunos comentarios CodeQL accesibles no sustituyen el inventario de alertas ni habilitan las capacidades ausentes. |

Siguen pendientes V12, reevaluación y aceptación humana. Se mantiene el draft #21 y su base; informes ajenos intactos.
