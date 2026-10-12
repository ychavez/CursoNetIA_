# Issues, pull requests y controles de calidad

El Coordinador guarda la síntesis y su issue; el Implementador publica un PR después de código y pruebas. Copilot revisa arquitectura, patrones y defectos. GitHub Actions ejecuta tests, mide cobertura y audita dependencias; CodeQL analiza vulnerabilidades. El humano decide el merge.

Repositorio: [ychavez/CursoNetIA_](https://github.com/ychavez/CursoNetIA_), rama base actual `master`. Verifica el remoto y la base al trabajar desde otra copia. Los perfiles y prompts están descritos en [el flujo de clase](multiagente-copilot.md) y [la configuración de Copilot](harness-copilot.md).

## Coordinador: síntesis e issue sin duplicados

Guarda `docs/informes/<tarea>/3-sintesis.md` antes de publicar. Incluye objetivo, fuentes, acuerdos/diferencias, alcance/exclusiones, criterios de aceptación y plan de pruebas. Para tareas sin consultas 2A/2B usa la petición y los archivos realmente disponibles como fuentes.

Conserva una clave estable por repositorio y tarea. Ejemplo de esta configuración:

```text
<!-- task:ychavez/CursoNetIA_:GITHUB-001 -->
```

1. Lee primero el issue ya enlazado en la síntesis y comprueba repositorio, marcador y objetivo.
2. Si no hay enlace, busca/lista issues abiertos y cerrados. Verifica el marcador exacto en su cuerpo; una coincidencia de título no basta. Si la búsqueda falla o está incompleta, no crees otro issue.
3. Si existe uno para esa tarea, actualiza la sección pertinente conservando aportes ajenos. Si está cerrado, registra el estado; no lo reabras ni crees otro sin determinar una nueva tarea. Si hay varios, declara el conflicto y no publiques un duplicado.
4. Si no existe, créalo mediante el MCP conectado con el contenido de la síntesis y el formato de [la plantilla](../.github/ISSUE_TEMPLATE/tarea.md). No copies conversaciones completas ni secretos. Incluye aceptación comprobable y pruebas, además de las reglas de arquitectura que apliquen.
5. Guarda número, URL, marcador, repositorio, base y estado de publicación en la síntesis. Ante un timeout o una respuesta incierta, vuelve a consultar antes de reintentar.

Una consulta de sólo análisis o un trabajo limitado a local no publica. Si faltan herramientas/permisos, conserva el cuerpo preparado y el estado `pendiente de publicación`; la implementación local puede continuar. Una petición explícita de no hacer cambios excluye también guardar informes.

## Implementador y Pruebas: un turno de escritura a la vez

El Implementador comprueba el estado local y crea/reutiliza una rama `codex/<tarea>-<descripcion>`. Conserva cambios ajenos y no trabaja directamente sobre la rama protegida. Escribe código y tests pertinentes. Cuando participe Pruebas, termina su turno antes de ceder la escritura.

En 2B, Pruebas sólo analiza y guarda su propio informe, sin ejecutar comandos. En la fase posterior puede escribir tests y su configuración y ejecutar comprobaciones. Registra resultados en `4-pruebas.md` conservando 2B. Un defecto de producción vuelve al Implementador con un caso reproducible. Los dos roles no editan simultáneamente.

Sólo el Implementador hace commit, push y PR. Revisa el diff completo, selecciona explícitamente los archivos de la tarea y publica su rama en el remoto verificado. Busca un PR de la misma tarea/rama/base antes de crear otro, incluyendo los cerrados si hubo un resultado incierto. Usa [la plantilla de PR](../.github/pull_request_template.md), `Closes #<numero>` del issue correcto, síntesis, resultados reales y pendientes. Conserva la entrega en `4-entrega.md` e informa la URL real.

El código y las pruebas locales deben estar terminados antes de declarar listo el PR. Si faltan verificaciones locales necesarias o hay fallos locales, cualquier publicación es un borrador que explica esos pendientes. Con las locales aprobadas se puede crear listo para revisión para disparar CI/Copilot, declarando los controles remotos en curso sin afirmar que el merge está aprobado. Tras corregir y verificar un borrador, el Implementador lo convierte a listo y solicita Copilot si hace falta; la revisión automática está configurada para PR listos y nuevos pushes. No hace merge ni habilita auto-merge.

## Qué se comprueba en el PR

| Control | Fuente | Criterio |
|---|---|---|
| `Build, tests and coverage` | [.github/workflows/ci.yml](../.github/workflows/ci.yml) | Build Release sin warnings, tests de solución y cobertura de líneas ≥80% |
| Arquitectura | `tests/AulaPedidos.ArchitectureTests` dentro de CI | Dependencias, contratos y restricciones aplicables de la base |
| `NuGet audit` | [.github/workflows/security.yml](../.github/workflows/security.yml) | Audita paquetes directos/transitivos; falla ante vulnerabilidades altas/críticas o error al obtener datos de vulnerabilidades |
| `Dependency review` | El mismo workflow, en PR | Revisa cambios de dependencias y falla ante vulnerabilidades altas/críticas |
| CodeQL | Configuración predeterminada administrada por GitHub | C#, suite extended, amenazas remotas/locales; el ruleset exige resultados y bloquea alertas de seguridad altas/críticas y hallazgos con nivel error |
| Copilot review | Instrucciones comunes y por rutas | Arquitectura, patrones, errores, seguridad y pruebas faltantes; hallazgos con ubicación e impacto |
| Dependabot / secretos | Configuración del repositorio | Alertas y actualizaciones de seguridad; secret scanning y push protection |

Los tres checks nombrados se requieren en el ruleset y se vinculan a la app GitHub Actions. CodeQL se exige por su regla de code scanning. La revisión de Copilot aporta comentarios; no representa tests ejecutados ni una aprobación humana. La ausencia de hallazgos no prueba ausencia de vulnerabilidades.

La cobertura es agregada de líneas instrumentadas de producción. No se exige cobertura de ramas ni cobertura diferencial de líneas nuevas en esta versión. Sólo se excluyen archivos generados en `obj`; no se excluyen archivos de aplicación para subir el porcentaje. El gate falla ante un reporte vacío/inválido y compara el valor sin redondear. El informe HTML ayuda a revisar líneas omitidas; el porcentaje no sustituye pruebas de comportamiento.

## Ejecutar las mismas comprobaciones localmente

Requiere SDK .NET 10. Ejecuta desde la raíz. Usa una carpeta nueva/vacía para cada ejecución de resultados y reportes; si conservas una ejecución anterior, usa otro nombre en las tres rutas correspondientes para no mezclar cobertura vieja.

```text
dotnet restore CursoNETIA.slnx
dotnet tool restore
dotnet build CursoNETIA.slnx -c Release --no-restore --warnaserror
dotnet run --file tools/CoverageGate.cs -- --self-test
dotnet test CursoNETIA.slnx -c Release --no-build --no-restore --collect "XPlat Code Coverage" --settings coverage.runsettings --logger trx --results-directory artifacts/test-results
dotnet tool run reportgenerator "-reports:artifacts/test-results/**/coverage.cobertura.xml" "-targetdir:artifacts/coverage" "-reporttypes:Html;Cobertura;MarkdownSummaryGithub"
dotnet run --file tools/CoverageGate.cs -- artifacts/coverage/Cobertura.xml 80
dotnet restore CursoNETIA.slnx --force --no-cache -p:NuGetAudit=true -p:NuGetAuditMode=all -p:NuGetAuditLevel=high -warnaserror:NU1900,NU1903,NU1904
```

Conserva cada resultado antes de pasar al siguiente. Una ejecución fallida no se vuelve satisfactoria porque haya un reporte antiguo. `artifacts/coverage/index.html` contiene el detalle; `Cobertura.xml`, el resultado combinado; `SummaryGithub.md`, el resumen. CI publica el resumen en la ejecución de Actions y conserva TRX/HTML/Cobertura en el artefacto `test-results-and-coverage` durante 14 días. No se requieren tokens de servicios de cobertura externos.

## Configuración remota y mantenimiento

Los archivos JSON del repositorio documentan la configuración remota; guardarlos no la aplica por sí solo. Para otra copia/fork deben comprobarse las capacidades de la cuenta, permisos de administración y estado remoto.

- **Ruleset:** [.github/github-ruleset.json](../.github/github-ruleset.json), aplicado a la rama predeterminada. Requiere PR, conversaciones resueltas, base actualizada, tres checks y CodeQL. Bloquea force push y borrado. No tiene bypass configurado. Actualmente exige cero aprobaciones formales porque sólo está el propietario como colaborador; el merge sigue siendo una decisión humana. Al incorporar otro revisor se puede exigir su aprobación.
- **Copilot:** la regla solicita revisión automática y revisiones en nuevos pushes; no revisa borradores automáticamente. Comprueba que las instrucciones personalizadas estén activadas y que la cuenta tenga acceso a Copilot review. En un PR existente sin revisión pendiente, el Implementador puede pedirla por MCP o desde Reviewers. [Configuración oficial de revisión](https://docs.github.com/en/copilot/how-tos/copilot-on-github/set-up-copilot/configure-code-review).
- **CodeQL:** [.github/codeql-default-setup.json](../.github/codeql-default-setup.json) conserva el payload de configuración. Default setup está administrado por GitHub, sin otro workflow `codeql.yml` duplicado. Revisa análisis y alertas desde Security and quality; los análisis programados también cubren riesgos posteriores. [Configuración oficial de CodeQL](https://docs.github.com/en/code-security/how-tos/find-and-fix-code-vulnerabilities/configure-code-scanning/configure-code-scanning).
- **Dependencias:** [.github/dependabot.yml](../.github/dependabot.yml) programa las actualizaciones. Mantén habilitados Dependency graph, Dependabot alerts y security updates. El workflow de seguridad también audita periódicamente el estado existente.
- **MCP y Git:** consulta [Harness Copilot](harness-copilot.md). Una conexión integrada en Copilot no demuestra que sus herramientas estén habilitadas para cada perfil ni que tengan escritura en este repositorio. El Coordinador necesita issues y el Implementador PR/revisión; Git necesita publicar la rama. No almacenes credenciales en estos archivos.

Las reglas ejecutables se administran en GitHub, no mediante frases como «bloquea este PR» dentro de las instrucciones de IA. [Referencia de rulesets y checks obligatorios](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/available-rules-for-rulesets). El formato común y los archivos por rutas orientan la revisión de Copilot. [Uso y personalización de revisiones](https://docs.github.com/en/copilot/how-tos/copilot-on-github/use-copilot-agents/copilot-code-review).

## Estado de la primera configuración

El 12 de octubre de 2026 se verificó el repositorio público `ychavez/CursoNetIA_` y se creó el [issue GITHUB-001, #1](https://github.com/ychavez/CursoNetIA_/issues/1). El [ruleset 24945310](https://github.com/ychavez/CursoNetIA_/rules/24945310) está activo. Se confirmaron Dependabot alerts/security updates y secret scanning/push protection. CodeQL default setup completó su [validación inicial](https://github.com/ychavez/CursoNetIA_/actions/runs/38290891257), con cero alertas abiertas en la consulta inicial.

La verificación local registró 45 tests aprobados, 66/67 líneas cubiertas (98.51%) y 11/14 ramas (78.57%); el umbral obligatorio sólo aplica a líneas. NuGet audit y la validación sintáctica de workflows finalizaron correctamente. Primera validación de PR pendiente: el resultado de CI, solicitud efectiva de revisión Copilot y URL final se registran en [la síntesis de GITHUB-001](informes/GITHUB-001/3-sintesis.md). Este párrafo es evidencia de esa configuración, no un indicador del estado de futuros commits.
