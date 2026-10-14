# TAREA-003 — Implementación del CRUD de productos

- Rol: Implementador. Fecha: 2026-10-13.
- Objetivo vigente: CRUD con Id Guid, Price decimal, Description y AlternateDescription; sin Name/Sku.
- Issue verificado: [#10](https://github.com/ychavez/CursoNetIA_/issues/10).
- Base verificada local/remota: master, `0ec4f67d70051ef91207c598cc055d32772f2fd1`.
- Rama MCP: `codex/tarea-003-crud-precio-descripciones`.
- PR real: [#19, borrador](https://github.com/ychavez/CursoNetIA_/pull/19), creado tras comprobar el primer diff remoto `11e3244ccbe118d6b3da11f2c7d67d7a9cc8194e`.

## Contexto y conservación

Consultados petición, índice y convención, síntesis 04, expediente histórico (sustituido por corrección humana), guías multiagente/PR, instrucciones generales y por capa, contratos, composición, repositorio, pruebas y workflows existentes.
El PR histórico #11 está cerrado y corresponde al contrato sustituido, no se reabre ni altera. La nueva rama parte de master para no arrastrar Name/Sku. Se conservan los informes locales 01–04 y `.github.zip`, excluidos de la publicación. Product.cs y ProductTests.cs estaban vacíos, sin contenido que sobrescribir. No se modifica almacenamiento compartido.

## Primer incremento verificable

Entidad Product con identidad no vacía e inmutable al actualizar; precios decimales y textos conservados literalmente. No se imponen precio no negativo, descripción obligatoria, trim ni límites: siguen sin decisión humana. Ambas descripciones admiten null. El servidor generará el Id en el alta.

Ejecutados localmente:
- `dotnet build CursoNETIA.slnx --configuration Release --warnaserror -p:EnableNETAnalyzers=true -p:AnalysisLevel=latest`: correcto, 0 advertencias/errores.
- `dotnet test tests/AulaPedidos.ArchitectureTests --configuration Release --no-build --logger trx --results-directory TestResults/TAREA-003/incremento`: 25/25 correctos.
- Evidencia local: `TestResults/tarea-003-incremento-build.log` y TRX en `TestResults/TAREA-003/incremento/`. No se publican artefactos binarios.

La adaptación arquitectónica local aún no está publicada en este primer incremento. Pendientes: handlers, rutas, persistencia/esquema, integración, cobertura y controles remotos.

## Controles

| Control | Estado | Commit y evidencia |
|---|---|---|
| Compilación y analizadores | correcto | Incremento local sobre 0ec4f67; log anterior; no acredita aún el diff remoto incompleto |
| Pruebas | correcto | Local, 25/25 arquitectura; TRX anterior |
| Cobertura combinada ≥80 % y reporte sin filtro | pendiente | Por ejecutar sobre implementación completa |
| NuGet Audit | pendiente | Por ejecutar explícitamente en modo all/moderate |
| Dependency Review | pendiente | Consultar checks del PR |
| CodeQL y alertas | pendiente | Consultar checks; herramientas de alertas no expuestas |
| Secret scanning / push protection | no disponible | Sin herramientas para verificar configuración, permisos y alertas |
| Copilot code review | pendiente | Solicitar tras apertura, comprobando duplicados |
| GitHub Code Quality / Autofix | no disponible | Sin herramientas de configuración/licencias/alertas; no se activan planes |

La configuración `.vscode/settings.json` solicita los toolsets; no hay capacidad disponible en esta sesión para recargar la conexión o inspeccionar el selector del IDE. No se sustituye MCP por otra vía de publicación.

## Entrega completa — 2026-10-13

PR real: **[#19](https://github.com/ychavez/CursoNetIA_/pull/19)**, borrador; misma rama y base master. El incremento completo se publicó por MCP en `ee559a7f91e576b45db889475f6a0eb7f5cd59f1`. Este informe, la documentación y el ajuste final de migraciones se publican después en la misma rama. La descripción del PR registrará el SHA real de entrega y su estado. Las tablas anteriores conservan el primer incremento, no son el estado final. No se reabre el PR histórico #11 ni se modifica el issue.

### Cambios y archivos afectados

- `src/AulaPedidos.Domain/Product.cs`: Id Guid no vacío y conservado al editar, Price decimal, Description y AlternateDescription nullable; sin Name/Sku ni reglas comerciales no aprobadas.
- `src/AulaPedidos.Application/Products/{ProductDto,ProductRequests,ProductHandlers}.cs`: DTO y cinco mensajes/handlers Mediator oficial, Repository/UnitOfWork, Update/Remove y SaveChanges explícitos, CancellationToken en I/O.
- `src/AulaPedidos.Infrastructure/Persistence/AppDbContext.cs` y `Migrations/{AddProducts,AppDbContextModelSnapshot}.cs`: mapeo SQLite, decimal TEXT sin double, migración inicial aditiva y snapshot. Ajuste final: modelo histórico de la migración independiente del snapshot regenerable.
- `src/AulaPedidos.Api/{Program,ProductEndpoints}.cs`: cinco operaciones /products, respuestas tipadas/OpenAPI, ProblemDetails técnico; POST 201/Location, GET 200, PUT 200 conservando Id, DELETE 204 vacío, inexistentes/Guid mal formado 404. JSON inválido, precio ausente/nulo/no representable: 400 sin escritura. Listado por Id. Ambas descripciones nullable, textos literales, precio negativo/cero permitido porque no hay restricciones comerciales aprobadas.
- Migración explícita `--migrate`, separada del arranque HTTP; nunca recrea la base. Antes de aplicarla revisar conexión y respaldar. Una Products externa requiere análisis previo: la migración inicial falla ante esa tabla sin convertir/borrar datos.
- `tests/AulaPedidos.ArchitectureTests/{ArchitectureTests,ProductTests}.cs`, `tests/AulaPedidos.Api.IntegrationTests/{ApiTests,ProductApiTests}.cs`, `tests/AulaPedidos.Infrastructure.Tests/ProductPersistenceTests.cs`: dominio, arquitectura/composición adaptadas, integración con Mediator real y SQLite aislado por caso, CRUD entre ámbitos, otro producto intacto, JSON/fronteras/identidad/nulabilidad/cancelación, migración repetible y rollback sólo efímero conservando tabla ajena.
- `docs/products.md` y este informe. Sin paquetes/proyectos nuevos ni cambios de umbral/exclusiones. Informes 01–04, zip y trabajo ajeno excluidos.

### Incidencias observadas y corregidas

1. La primera suite completa falló por inventario API y aislamiento: ConfigureAppConfiguration no sustituyó la cadena capturada previamente por AddInfrastructure. Corregido reemplazando explícitamente opciones/configuración EF y comprobando DataSource **antes** de migrar. El primer ensayo creó/escribió `tests/AulaPedidos.Api.IntegrationTests/bin/Release/net10.0/aulapedidos.db` (creación 17:49:39 local), archivo de salida ignorado, no la base de src/Api ni raíz. Se conserva sin borrar/modificar desde el diagnóstico. Esta incidencia matiza la afirmación inicial sobre almacenamiento; no se oculta ni se limpia destructivamente. No se ha constatado acceso a una base compartida externa.
2. Primer ensayo CLI falló antes de abrir la base: el parser consumía la conexión como valor de --migrate. Corregido separando el indicador. Dos ejecuciones posteriores sobre `TestResults/TAREA-003/migrate-command.db`: códigos 0 y 0, la segunda sin aplicar migraciones.
3. Copilot detectó en el primer commit el inventario arquitectónico aún no publicado: [hallazgo real](https://github.com/ychavez/CursoNetIA_/pull/19#discussion_r4265290306), [revisión](https://github.com/ychavez/CursoNetIA_/pull/19#pullrequestreview-5511418971). Corrección publicada, CI de ee559a7 correcto. Hilo todavía abierto; no se simula aceptación ni resolución.
4. La comparación inicial por terminal produjo falsos positivos Unicode. Repetida mediante lectura UTF-8 normalizando sólo saltos de línea: los 14 archivos publicados coinciden salvo los dos de migración por el ajuste final posterior. Se corrige un blanco final remoto de Program. No se ignoran diferencias de código.

### Comandos ejecutados y resultados

- `dotnet restore CursoNETIA.slnx -p:NuGetAudit=true -p:NuGetAuditMode=all -p:NuGetAuditLevel=moderate --warnaserror`: correcto sin avisos; `TestResults/tarea-003-audit.log`.
- `dotnet build CursoNETIA.slnx -c Release --no-restore --warnaserror -p:EnableNETAnalyzers=true -p:AnalysisLevel=latest`: correcto tras ajuste final, 0 advertencias/errores; `TestResults/tarea-003-entrega-build.log`.
- `dotnet test CursoNETIA.slnx -c Release --no-build --no-restore --logger trx --results-directory TestResults/TAREA-003/entrega/all`: **67/67**, 25 arquitectura, 14 infraestructura y 28 API; 0 fallidas/omitidas.
- Infraestructura y API repetidas con `--collect:"XPlat Code Coverage"` y directorios `TestResults/TAREA-003/entrega/{infrastructure,api}`. Arquitectura sin instrumentación por inspección de tipos exactos.
- `dotnet tool restore`; ReportGenerator 5.5.11 combina ambos XML, sin promedios. Reporte sin filtro `TestResults/TAREA-003/entrega/CoverageRaw`: **403/789 = 51,08 %**. Reporte mantenido `TestResults/TAREA-003/entrega/CoverageReport`, único filtro `-**/obj/**`: **194/201 = 96,52 %**, cuatro capas, incluyendo migraciones y snapshot. Reportes originales conservados; no se excluye código mantenido.
- `python .github/scripts/check_coverage.py TestResults/TAREA-003/entrega/CoverageReport/Cobertura.xml`: correcto, 96,52 % ≥80 %.
- CLI ejecutado dos veces con `dotnet src/AulaPedidos.Api/bin/Release/net10.0/AulaPedidos.Api.dll --migrate "--ConnectionStrings:Default=Data Source=<workspace>/TestResults/TAREA-003/migrate-command.db;Pooling=False"`: 0/0; `TestResults/tarea-003-migrate-corregido.log`. Los comandos generales de docs/products.md son instrucciones operativas, no ejecución sobre la base del usuario.
- `git diff --check` local correcto; repetir remoto tras corregir el blanco final. Logs iniciales/finales, TRX y ambos reportes se conservan localmente; no se publican bases, binarios ni artefactos.

### Controles de entrega

Los resultados locales corresponden a los archivos técnicos de entrega. Los checks de ee559a7 son antecedentes: **no acreditan el head posterior** de este informe/ajuste. El commit real se consulta en el [historial de rama](https://github.com/ychavez/CursoNetIA_/commits/codex/tarea-003-crud-precio-descripciones) y se registra en la descripción del PR.

| Control | Estado | Commit y evidencia |
|---|---|---|
| Compilación/analizadores local | correcto | Código de entrega; entrega-build, 0 advertencias/errores |
| Pruebas locales | correcto | Código de entrega; 67/67, TRX entrega/all |
| Cobertura local ≥80 % | correcto | Código de entrega; 194/201, 96,52 %, entrega/CoverageReport |
| Reporte sin filtro | correcto | Código de entrega; 403/789, 51,08 %, entrega/CoverageRaw |
| Build/tests/cobertura remoto | pendiente | Revalidar head de entrega en [checks #19](https://github.com/ychavez/CursoNetIA_/pull/19/checks). ee559a7 [correcto](https://github.com/ychavez/CursoNetIA_/actions/runs/38487146612/job/115603417048); artefactos no descargados |
| NuGet Audit | correcto | Local all/moderate; dependencias sin cambios. ee559a7 [correcto](https://github.com/ychavez/CursoNetIA_/actions/runs/38487146612/job/115603416687); consultar nuevo head |
| Dependency Review | pendiente | Revalidar entrega; ee559a7 [correcto](https://github.com/ychavez/CursoNetIA_/actions/runs/38487146624/job/115603417260) |
| CodeQL ejecución | pendiente | Revalidar entrega; ee559a7 [en curso](https://github.com/ychavez/CursoNetIA_/actions/runs/38487146624/job/115603417553) al consultar |
| Alertas CodeQL/Dependabot | no disponible | Sin herramientas de alertas; éxito de check no demuestra ausencia |
| Secret scanning/push protection | no disponible | Sin acceso a ajustes/licencias/alertas. GitGuardian ee559a7 [correcto](https://github.com/ychavez/CursoNetIA_/runs/115603404751), no acredita controles nativos |
| Copilot code review | pendiente | Revalidar entrega sin duplicar. ee559a7 [solicitada y en curso](https://github.com/ychavez/CursoNetIA_/actions/runs/38487230844/job/115603700283); hallazgo inicial corregido, hilo abierto |
| GitHub Code Quality / Copilot Autofix | no disponible | Sin herramientas de disponibilidad/permisos/configuración; no se activan planes |

Pendientes de cierre: checks/revisión del head final, controles no accesibles y aceptación humana. Se mantiene **draft #19**, sin aprobación, auto-merge, fusión ni cambios de reglas. Las restricciones comerciales no aprobadas no se presentan como decisiones humanas.

## Seguimiento de revisiones — entrega final

La [segunda revisión Copilot de ee559a7](https://github.com/ychavez/CursoNetIA_/pull/19#pullrequestreview-5511487407) terminó con hallazgos. No se presenta como aprobada. Se atienden:
- [Esquema en instalación limpia](https://github.com/ychavez/CursoNetIA_/pull/19#discussion_r4265354854): `docs/api.md` se actualiza al contrato real y enlaza el comando --migrate antes del arranque. No se migra automáticamente almacenamiento compartido. Se ejecutó el flujo completo sobre `TestResults/TAREA-003/startup-clean.db`: migración correcta y host HTTP real con `/health` 200 y `/products` 200 `[]`; evidencia `TestResults/tarea-003-startup-{migrate,smoke,server}.log`. Se detuvo exclusivamente ese proceso de prueba.
- [Modelo histórico mutable](https://github.com/ychavez/CursoNetIA_/pull/19#discussion_r4265354903): BuildTargetModel independiente, comprobado con migración/rollback aislados.
- [Contrato HTTP antiguo](https://github.com/ychavez/CursoNetIA_/pull/19#discussion_r4265354932): `docs/api.md` refleja /products, los cuatro campos, validación, rutas técnicas reales y capacidades no implementadas; remite a docs/products.md.
- CodeQL terminó correctamente en ee559a7, **pero notificó cinco alertas 25–29 en la revisión**: [25](https://github.com/ychavez/CursoNetIA_/security/code-scanning/25), [26](https://github.com/ychavez/CursoNetIA_/security/code-scanning/26), [27](https://github.com/ychavez/CursoNetIA_/security/code-scanning/27), [28](https://github.com/ychavez/CursoNetIA_/security/code-scanning/28), [29](https://github.com/ychavez/CursoNetIA_/security/code-scanning/29). Son evidencia accesible aunque no haya herramienta de inventario global. Se conserva y dispone la factoría raíz de WebApplicationFactory además de su derivada; Path.Join evita descartar partes anteriores; se elimina un cast redundante usando ProductRequest y se proyectan solicitudes con Select y disposición explícita. Sin supresiones.

Verificaciones repetidas tras esas correcciones: build Release estricto **0 advertencias/errores**, solución **67/67** tests, cobertura **194/201 = 96,52 %** mantenida y **403/789 = 51,08 %** sin filtro. Evidencia vigente en `TestResults/tarea-003-revision-*.log` y `TestResults/TAREA-003/revision/{all,infrastructure,api,CoverageReport,CoverageRaw}`. Gate de cobertura correcto y diff local sin errores. Estos resultados sustituyen la evidencia local de entrega anterior, que se conserva.

Estado actualizado de controles: alertas CodeQL conocidas **pendiente** de nuevo análisis que verifique las correcciones; inventario completo de alertas CodeQL/Dependabot **no disponible** por MCP. Copilot **pendiente** de revisión del head que publica estas correcciones. Sus hilos anteriores permanecen abiertos; no se dispone de herramienta para responder/resolverlos ni se simula su resolución. Build/tests/cobertura, auditoría, Dependency Review, CodeQL y GitGuardian remotos del head final deben comprobarse de nuevo: los resultados correctos de ee559a7 son sólo antecedentes. La descripción de #19 recogerá los resultados observados del SHA nuevo, sin polling continuo. Se conserva el borrador para aceptación humana.
