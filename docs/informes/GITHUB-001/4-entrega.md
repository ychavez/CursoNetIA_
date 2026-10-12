# Entrega GITHUB-001

- Repositorio: `ychavez/CursoNetIA_`.
- Issue: [#1](https://github.com/ychavez/CursoNetIA_/issues/1).
- PR: [#2](https://github.com/ychavez/CursoNetIA_/pull/2).
- Rama: `codex/github-quality-flow`, base `master`.
- Commit de implementación y configuración: `000788570e1064be8779ccac8d1d4701e6ef20ab`. Este informe se añade después para conservar los enlaces reales, sin autorreferenciar el SHA del propio documento.
- Síntesis: [3-sintesis.md](3-sintesis.md).

## Resultado local

| Comprobación ejecutada | Resultado |
|---|---|
| `dotnet build CursoNETIA.slnx -c Release --warnaserror` | Aprobado, cero advertencias y errores |
| Tests con Coverlet y `coverage.runsettings` | 45/45: 21 arquitectura, 11 API, 13 Infrastructure |
| ReportGenerator y gate con mínimo 80% | 66/67 líneas = 98.51%; 11/14 ramas = 78.57%; aprobado |
| `CoverageGate.cs --self-test` | 10 comprobaciones aprobadas |
| Gate con umbral 99% | Falló como se esperaba |
| Reporte inexistente, vacío/inválido y glob sin informes | Rechazados; no producen un resultado verde |
| Auditoría NuGet directas/transitivas, high/critical | Aprobada |
| `actionlint` 1.7.12 sobre ambos workflows | Aprobado |
| `git diff --check` | Aprobado |

Los comandos completos y ubicaciones de artefactos están en [la guía](../../github-flow.md). La evidencia local final se produjo en `artifacts/verification/coverage`, directorio ignorado por Git. No se cambiaron fuentes funcionales de la aplicación.

## Configuración y ejecución en GitHub

- [Ruleset activo](https://github.com/ychavez/CursoNetIA_/rules/24945310): PR, conversaciones resueltas, checks obligatorios vinculados a GitHub Actions, CodeQL y solicitud automática de Copilot en PR listos y nuevos pushes.
- CodeQL default setup: C#, consultas extendidas y fuentes remotas/locales. [Validación inicial en master](https://github.com/ychavez/CursoNetIA_/actions/runs/38290891257) aprobada, cero alertas abiertas en la consulta de activación.
- Dependabot alerts/security updates y secret scanning/push protection confirmados habilitados por API.
- El PR activó realmente CI, Security, CodeQL y `copilot-pull-request-reviewer`. Los [checks del PR](https://github.com/ychavez/CursoNetIA_/pull/2/checks) son la fuente del estado actual, asociado al commit que muestran; la descripción del PR registra su revisión final.
- No se hizo merge ni se habilitó auto-merge. Los workflows/calendario de Dependabot se incorporarán a `master` al integrar el PR.

El MCP del IDE no se probó operando una sesión de Visual Studio desde este chat. Los perfiles quedan preparados para las herramientas disponibles y la [guía de configuración](../../harness-copilot.md) indica cómo comprobar el panel Tools. Esto es distinto de las operaciones GitHub y ejecuciones remotas anteriores, verificadas realmente.
