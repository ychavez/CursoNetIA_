# TAREA-003 — Implementación

- **Objetivo:** incorporar únicamente `Product` con `Guid Id`, `Name` y `Sku`.
- **Rol:** Implementador.
- **Fecha:** 2026-10-13, verificada con el reloj del entorno.
- **Issue:** [#10](https://github.com/ychavez/CursoNetIA_/issues/10).
- **Repositorio:** `ychavez/CursoNetIA_`; base remota verificada: `master`.
- **Rama reutilizada:** `codex/tarea-003-crud-productos`, inicialmente en `215619f9d87ba32f3cdaad25a89c74c88d912fc8`, igual que la base.
- **Draft PR:** [#11](https://github.com/ychavez/CursoNetIA_/pull/11), abierto mediante MCP con head y base explícitos; permanece draft.
- **Commit técnico publicado:** [`8480ccaac5e61a0c1db8f3bf80b884d6ef3463f2`](https://github.com/ychavez/CursoNetIA_/commit/8480ccaac5e61a0c1db8f3bf80b884d6ef3463f2).

## Alcance y fuentes

Se aplican la petición humana, `01-coordinacion.md`, `02-arquitectura.md`, `04-sintesis.md` y el issue #10. El CRUD del expediente histórico `docs/expedientes/Producto-003.md` queda aplazado: no se implementan precio, persistencia, handlers ni endpoints.

También se consultaron las instrucciones generales, C#, Domain y tests; `docs/informes/README.md`, `indice.md`, `docs/multiagente-copilot.md`, `docs/github-pr.md`, la plantilla de PR, los proyectos Domain y de pruebas, las pruebas existentes y el workflow CI local. No se modifican informes de otros roles.

## Cambios y archivos afectados

| Archivo | Resultado |
|---|---|
| `src/AulaPedidos.Domain/Product.cs` | Clase pública sellada, constructor completo y propiedades de sólo lectura. No transforma ni valida comercialmente los valores; nulabilidad habilitada por el proyecto. |
| `tests/AulaPedidos.ArchitectureTests/ArchitectureTests.cs` | Sustituye la expectativa histórica de Domain vacío por el conjunto exacto que contiene únicamente Product; conserva las demás restricciones de arquitectura. |
| `tests/AulaPedidos.ArchitectureTests/ProductTests.cs` | Cuatro casos: valores normales, Guid vacío/cadenas vacías, conservación de espacios y mayúsculas, y contrato público de sólo lectura/no anulable/sin Price. |
| `docs/informes/TAREA-003/05-implementacion.md` | Evidencia y pendientes de esta implementación. |

No se cambian proyectos, solución ni dependencias. El constructor y la inmutabilidad son una elección técnica de este incremento, no una regla irreversible para el CRUD futuro.

## Conservación del trabajo existente

La copia local permanece en `master`, con un commit local previo `8fdc772` y numerosos cambios ajenos de agentes, documentación y CI. Se inspeccionaron `git status`, remoto, ramas, historial y referencias remotas. Se publican mediante MCP exclusivamente los cuatro archivos de esta tabla, sin incluir ese commit ni los cambios ajenos. La rama existente corresponde a esta tarea; no se encontró PR por ID ni por head.

## Comprobaciones ejecutadas — primer incremento

1. `dotnet build CursoNETIA.slnx --configuration Release --warnaserror -p:EnableNETAnalyzers=true -p:AnalysisLevel=latest -p:NuGetAudit=true -p:NuGetAuditMode=all -p:NuGetAuditLevel=moderate`: correcto, sin diagnósticos de advertencia/error. Un intento previo con redirección a log fue cancelado; no se usa como evidencia.
2. `dotnet test CursoNETIA.slnx --configuration Release --no-build --no-restore --logger trx --results-directory TestResults/TAREA-003/tests --tl:off`: **49 correctas, 0 fallidas, 0 omitidas** (25 arquitectura, 13 infraestructura, 11 API).
3. `git diff --check`: sin errores de whitespace; Git notificó conversiones LF/CRLF en archivos ajenos, que no se modificaron.
4. Consulta MCP de repositorio, ramas, issue #10 y PR: base remota `master`, permiso push disponible y sin PR de esta tarea.

Evidencia TRX local: `TestResults/TAREA-003/tests/`. Los resultados locales corresponden al contenido de los tres archivos técnicos de este incremento; no sustituyen checks remotos.

## Seguimiento — cobertura y publicación real, 2026-10-13

Se publicó el primer incremento mediante `push_files` MCP. `get_commit` con `full_patch` confirmó exclusivamente los cuatro archivos de la tabla antes de abrir el draft #11. La única diferencia incidental del archivo arquitectónico es su terminador EOF. Se comprobó la igualdad del contenido técnico local/remoto: comparación normalizada para los archivos nuevos y `git diff --ignore-space-at-eol` para el archivo existente. La primera comparación por hash detectó terminadores de línea distintos y no se trató como igualdad binaria.

### Comandos y resultados adicionales ejecutados

```text
dotnet test tests/AulaPedidos.Infrastructure.Tests/AulaPedidos.Infrastructure.Tests.csproj --configuration Release --no-build --no-restore --logger trx --collect:"XPlat Code Coverage" --results-directory TestResults/TAREA-003/infrastructure --tl:off -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
dotnet test tests/AulaPedidos.Api.IntegrationTests/AulaPedidos.Api.IntegrationTests.csproj --configuration Release --no-build --no-restore --logger trx --collect:"XPlat Code Coverage" --results-directory TestResults/TAREA-003/api --tl:off -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
dotnet test tests/AulaPedidos.ArchitectureTests/AulaPedidos.ArchitectureTests.csproj --configuration Release --no-build --no-restore --filter FullyQualifiedName~ProductTests --test-adapter-path "$env:USERPROFILE/.nuget/packages/coverlet.collector/6.0.4/build/netstandard2.0" --logger trx --collect:"XPlat Code Coverage" --results-directory TestResults/TAREA-003/product --tl:off -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
dotnet tool restore
dotnet tool run reportgenerator "-reports:TestResults/TAREA-003/infrastructure/*/coverage.cobertura.xml;TestResults/TAREA-003/api/*/coverage.cobertura.xml;TestResults/TAREA-003/product/*/coverage.cobertura.xml" "-targetdir:TestResults/TAREA-003/CoverageRaw" "-reporttypes:Cobertura;MarkdownSummaryGithub"
dotnet tool run reportgenerator "-reports:TestResults/TAREA-003/infrastructure/*/coverage.cobertura.xml;TestResults/TAREA-003/api/*/coverage.cobertura.xml;TestResults/TAREA-003/product/*/coverage.cobertura.xml" "-targetdir:TestResults/TAREA-003/CoverageReport" "-reporttypes:Cobertura;MarkdownSummaryGithub;HtmlInline_AzurePipelines" "-filefilters:-**/obj/**"
dotnet build CursoNETIA.slnx --configuration Release --no-restore --warnaserror -p:EnableNETAnalyzers=true -p:AnalysisLevel=latest --tl:off
```

- Cobertura: 13 pruebas de infraestructura, 11 de API y 4 de Product correctas. Las otras pruebas arquitectónicas se habían ejecutado sin instrumentación, para conservar la comprobación de tipos exactos.
- Se reutilizó el colector Coverlet 6.0.4 ya restaurado y ReportGenerator 5.5.11 del manifiesto local preexistente, sin cambiar proyectos, paquetes ni configuración ajena.
- Combinación por líneas de **las cuatro capas**: **74/75 = 98,6667 %** de líneas mantenidas; ramas 12/14 = 85,7 %. `Product`: **100 %**. El umbral ≥ 80 % se comprobó leyendo el XML, no promediando suites.
- Reporte completo sin filtro: **107/586 = 18,2594 %**, incluido el código generado. ReportGenerator avisó de fuentes generadas de `obj/` no materializadas en disco; se conservan los contadores en el reporte completo y no se oculta código mantenido.
- Evidencia local: `TestResults/TAREA-003/{infrastructure,api,product}/`, `TestResults/TAREA-003/CoverageReport/{Cobertura.xml,SummaryGithub.md,index.html}` y `TestResults/TAREA-003/CoverageRaw/{Cobertura.xml,SummaryGithub.md}`. Son artefactos ignorados, no subidos al PR; las rutas locales no son enlaces a artefactos de Actions.
- La última compilación confirmó explícitamente **0 advertencias y 0 errores**.
- Tras la medición se repitió `dotnet test CursoNETIA.slnx --configuration Release --no-build --no-restore --logger trx --results-directory TestResults/TAREA-003/final --tl:off`: nuevamente **49/49 correctas**, sin omitidas; TRX en `TestResults/TAREA-003/final/`.

### GitHub y límites de las capacidades

- PR #11: `draft: true`, base `master`, head `codex/tarea-003-crud-productos`, `Closes #10` y enlace al informe. Sin aprobación, auto-merge ni fusión.
- Antes de solicitar revisión se consultaron revisiones MCP (vacías) y los revisores solicitados mediante una lectura REST pública desde el terminal (vacíos). La publicación y la solicitud se hicieron mediante MCP, no mediante REST o git push.
- `request_copilot_review` se ejecutó para #11. La línea temporal pública confirma **review_requested**, revisor **Copilot**, a las **19:58:36Z**; el check [copilot-pull-request-reviewer](https://github.com/ychavez/CursoNetIA_/actions/runs/38459130270/job/115509449322) estaba en curso. No se simuló una revisión ni se inició otro agente escritor. La regla de nuevas revisiones automáticas no es verificable con las herramientas expuestas.
- La [revisión real inicial de Copilot](https://github.com/ychavez/CursoNetIA_/pull/11#pullrequestreview-5509232766), sobre `8480ccaa`, terminó a las **20:00:46Z** como `COMMENTED`. Su único [hallazgo de severidad baja](https://github.com/ychavez/CursoNetIA_/pull/11#discussion_r4263391874) solicita registrar el PR real en el informe. Este seguimiento lo corrige en la cabecera y sustituye el pendiente de apertura, sin cambiar el código. El hilo no se declara aceptado/resuelto por una persona; la nueva revisión sigue pendiente.
- Para `8480ccaa`, [Analyze (csharp)](https://github.com/ychavez/CursoNetIA_/actions/runs/38458985136/job/115508937204) y [CodeQL](https://github.com/ychavez/CursoNetIA_/runs/115509636049) terminaron correctamente. La lectura pública del run confirmó `head_sha=8480ccaac5e61a0c1db8f3bf80b884d6ef3463f2` y evento `dynamic`: **sí existe análisis remoto aunque no haya workflows versionados en master**. No se conoce la configuración exacta ni el estado de sus alertas.
- [GitGuardian Security Checks](https://github.com/ychavez/CursoNetIA_/runs/115508927399): correcto para ese commit. No acredita secret scanning nativo ni push protection.
- El escaneo dirigido MCP devolvió exactamente `Repository does not have GitHub Advanced Security enabled.`: no produjo un resultado de escaneo. Queda no disponible en esta sesión; no se presenta como libre de secretos.
- No hay herramientas MCP expuestas de alertas Dependabot/code scanning, configuración de push protection/rulesets, Autofix, licencias o GitHub Code Quality. Su verificación requiere acceso adicional; no se activaron servicios o planes.
- Los estados remotos descritos pertenecen al commit técnico. Este seguimiento se publicará como incremento **sólo documental**; sus checks y revisión deben volver a consultarse para el nuevo head. Un resultado del primer commit no se acredita como check del nuevo commit.

## Controles

| Control | Estado | Commit y evidencia |
|---|---|---|
| Compilación y analizadores | correcto | Código técnico de `8480ccaa`, sin cambios posteriores de código: build Release local, 0 advertencias/errores. Evidencia en la sección de comandos. Check remoto de build ausente. |
| Pruebas | correcto | Mismo código técnico; TRX locales `TestResults/TAREA-003/tests/`, 49/49. Sin check remoto de pruebas. |
| Cobertura combinada de líneas mantenidas ≥ 80 % y reporte sin filtro | correcto | Mismo código técnico; [medición local](#comandos-y-resultados-adicionales-ejecutados), 74/75 (98,6667 %). Reporte completo 107/586, ambos conservados localmente. Sin artefactos remotos de cobertura. |
| NuGet Audit | correcto | Mismo código/dependencias: build con auditoría transitiva moderate y warnaserror. |
| Dependency Review | pendiente | No aparece check en [PR #11](https://github.com/ychavez/CursoNetIA_/pull/11/checks). |
| CodeQL — ejecución | pendiente | Correcto en `8480ccaa`: [CodeQL](https://github.com/ychavez/CursoNetIA_/runs/115509636049). Revalidación del head documental pendiente; consultar [checks vigentes](https://github.com/ychavez/CursoNetIA_/pull/11/checks). |
| Alertas CodeQL / Dependabot | no disponible | Sin herramientas de alertas; un check correcto no acredita ausencia de alertas. |
| Secret scanning dirigido MCP | no disponible | Respuesta sobre GitHub Advanced Security no habilitado; no se generó un escaneo. GitGuardian correcto sólo en `8480ccaa`. |
| Push protection y configuración nativa de secretos | no disponible | Sin herramienta para comprobar ajustes remotos; no se declaran habilitados. |
| Copilot code review | pendiente | [Revisión inicial real](https://github.com/ychavez/CursoNetIA_/pull/11#pullrequestreview-5509232766) sobre `8480ccaa`, COMMENTED: un hallazgo documental corregido aquí. Verificación/revisión del siguiente head documental pendiente; no acredita el último push. |
| GitHub Code Quality / Autofix | no disponible | La sesión no expone herramientas para verificar configuración, licencia o alertas; no se activan planes. |

## Pendientes

- Consultar checks/status/revisiones del head tras publicar este seguimiento documental y solicitar Copilot para ese commit si no existe revisión automática; reflejar el estado vigente en la descripción del mismo PR #11. No hacer polling continuo ni repetir solicitudes sobre el mismo commit.
- Incorporar posteriormente evidencia remota de build/tests/cobertura y Dependency Review. La configuración local CI/security es trabajo ajeno no publicado por esta tarea; no modificarla ni incluirla para conseguir verde. CodeQL remoto existente se registra por separado.
- Conservar los artefactos locales; no existen artefactos remotos de cobertura disponibles en esta entrega.
- Verificar alertas, push protection, Dependabot, CodeQL, rulesets, Autofix y Code Quality cuando existan capacidades y acceso apropiados. La ausencia de herramientas no demuestra ausencia de alertas.
- Mantener el PR draft: aceptación humana pendiente, sin aprobación, auto-merge ni fusión.
