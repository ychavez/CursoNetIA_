# GITHUB-001: agentes, GitHub y controles de calidad

## Objetivo y decisión

Petición humana: añadir al flujo de AulaPedidos la creación de issues y PR, revisión de arquitectura y patrones por Copilot, detección de vulnerabilidades, ejecución de tests y control de cobertura. Autorizado en el chat el 12 de octubre de 2026.

Repositorio: `ychavez/CursoNetIA_` (rama predeterminada `master`).

Issue: [#1 — Integrar agentes con issues, PR y controles de calidad](https://github.com/ychavez/CursoNetIA_/issues/1).

Marcador de idempotencia: `<!-- task:ychavez/CursoNetIA_:GITHUB-001 -->`.

Rama de implementación: `codex/github-quality-flow`.

PR: [#2 — Integra issues y PR con CI, cobertura y revisión Copilot](https://github.com/ychavez/CursoNetIA_/pull/2).

## Fuentes consultadas

- Instrucciones y agentes de `.github/`, código de `src/` y pruebas de `tests/`.
- Informes existentes y estado real de BASE-002; documentación anterior que aún describe una aplicación futura.
- Documentación oficial de GitHub para MCP, code review, Actions, rulesets, Dependabot y CodeQL.
- Documentación oficial de Visual Studio para herramientas de agentes y de .NET para cobertura y auditoría NuGet.
- GitHub API: repositorio público, acceso de administración, rama `master`, un único colaborador propietario, sin rulesets ni protección previa.

No se realizaron consultas 2A/2B de una nueva funcionalidad de negocio ni se inventaron esos informes. Esta tarea configura el proceso solicitado sobre la aplicación existente.

## Alcance acordado

1. El Coordinador guarda la síntesis y crea o actualiza el issue de la misma tarea; conserva referencias y evita duplicados.
2. Implementador escribe código de producción. Pruebas diseña casos en 2B y puede escribir y ejecutar tests durante la implementación; los roles se turnan y no editan el mismo archivo simultáneamente.
3. Implementador es el único publicador del PR; registra resultados reales, enlaza el issue y atiende checks y hallazgos dentro del alcance.
4. Copilot aplica reglas concretas de capas, patrones y seguridad. Los checks ejecutables son responsabilidad de Actions y CodeQL.
5. CI compila y ejecuta todas las pruebas; agrega cobertura por código fuente y exige al menos 80% de líneas. Solo se excluye código generado en `obj` y ensamblados de pruebas.
6. Auditoría NuGet y Dependency Review bloquean vulnerabilidades altas/críticas. Dependabot mantiene paquetes y acciones.
7. CodeQL usa la configuración administrada por GitHub, suite extendida para C#, con análisis remoto y local. No se añade un segundo workflow CodeQL que compita con ella.
8. La rama predeterminada requiere PR, checks, resolución de conversaciones y análisis CodeQL sin errores ni alertas de seguridad altas/críticas. Se solicita Copilot al abrir PR y tras nuevos pushes; no en borradores.
9. No se habilita merge automático. El propietario decide integrar. El número de aprobaciones obligatorias es cero porque el repositorio solo tiene al propietario, que no puede aprobar su propio PR; al incorporar otro revisor se puede elevar a uno.

## Criterios y evidencia

| Criterio | Comprobación |
|---|---|
| Trazabilidad sin duplicados | Issue #1 y síntesis con el mismo marcador; PR de la rama vinculado al issue. |
| Aplicación sin regresiones | Build Release sin advertencias; 45/45 tests aprobados antes y después de añadir el colector. |
| Cobertura obligatoria | 66/67 líneas (98.51%), 11/14 ramas (78.57%). El gate exige líneas 80%, pasa 10 autopruebas y rechaza umbral 99%, reporte ausente e inválido. ReportGenerator rechaza glob sin informes. |
| Seguridad ejecutable | Jobs `NuGet audit`, `Dependency review` y análisis administrado CodeQL. |
| Protección remota | Ruleset activo 24945310 verificado por API; checks asociados a GitHub Actions (app 15368). |
| Límites de agentes | No publicar en peticiones de solo análisis; no ejecutar herramientas inexistentes ni afirmar éxitos sin respuesta. |

## Estado de activación

- Issue #1 creado realmente.
- Dependabot alerts y security updates habilitados; secret scanning y push protection habilitados.
- CodeQL configurado con ejecución semanal; [CodeQL Setup](https://github.com/ychavez/CursoNetIA_/actions/runs/38290891257) completado correctamente y cero alertas abiertas en la base.
- [Ruleset 24945310](https://github.com/ychavez/CursoNetIA_/rules/24945310) activo: checks, CodeQL, revisión automática Copilot y protección de `master`.
- Auditoría NuGet local aprobada; `actionlint` 1.7.12 valida ambos workflows sin errores.
- El PR #2 inició CI, seguridad, CodeQL y revisión automática de Copilot. Los checks y comentarios del PR conservan el resultado real de cada commit; consulta [la entrega](4-entrega.md).

## Pendientes y límites

El MCP instalado en Visual Studio pertenece a esa sesión. Los agentes quedan preparados para usar las herramientas disponibles; su activación en el panel Tools y las aprobaciones nativas del IDE se comprueban allí. Este trabajo no cambia credenciales ni preferencias globales de Visual Studio.

Las pruebas de arquitectura existentes también fijan restricciones del esqueleto BASE-002. Una futura funcionalidad que agregue tipos/proyectos debe actualizar esas expectativas justificadamente, conservando los límites de capas.
