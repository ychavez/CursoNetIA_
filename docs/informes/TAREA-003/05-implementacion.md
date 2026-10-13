# TAREA-003 — Implementación

- **Objetivo:** incorporar únicamente `Product` con `Guid Id`, `Name` y `Sku`.
- **Rol:** Implementador.
- **Fecha:** 2026-10-13, verificada con el reloj del entorno.
- **Issue:** [#10](https://github.com/ychavez/CursoNetIA_/issues/10).
- **Repositorio:** `ychavez/CursoNetIA_`; base remota verificada: `master`.
- **Rama reutilizada:** `codex/tarea-003-crud-productos`, inicialmente en `215619f9d87ba32f3cdaad25a89c74c88d912fc8`, igual que la base.
- **Draft PR:** pendiente de apertura mediante MCP tras publicar este primer incremento verificado.

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

Evidencia TRX local: `TestResults/TAREA-003/tests/`. Los resultados locales corresponden al contenido de los tres archivos técnicos de este incremento; no acreditan checks remotos todavía.

## Controles

| Control | Estado | Commit y evidencia |
|---|---|---|
| Compilación y analizadores | correcto | Contenido local del incremento; build estricto ejecutado. |
| Pruebas | correcto | Contenido local del incremento; [TRX locales](../../../TestResults/TAREA-003/tests/), 49/49. |
| Cobertura combinada de líneas mantenidas ≥ 80 % y reporte sin filtro | pendiente | Pendiente de medición real. |
| NuGet Audit | correcto | Build con auditoría transitiva, nivel moderate y warnaserror. |
| Dependency Review | pendiente | Sin PR/check vigente todavía. |
| CodeQL y alertas de seguridad | pendiente | Sin PR/check vigente todavía; no hay herramienta MCP de alertas expuesta. |
| Secret scanning / push protection | pendiente | Disponibilidad remota sin verificar; existe herramienta MCP de escaneo de contenido. |
| Copilot code review | pendiente | Solicitar al abrir el draft, comprobando antes solicitudes/revisiones existentes. |
| GitHub Code Quality / Autofix | no disponible | La sesión no expone herramientas para verificar configuración, licencia o alertas; no se activan planes. |

## Pendientes

- Publicar el incremento revisado, comprobar su diff remoto y abrir el draft PR enlazado con #10; registrar número, URL y commit reales.
- Solicitar Copilot code review sin duplicar una revisión del mismo commit y consultar checks, status y comentarios accesibles.
- Medir cobertura combinada sin excluir código mantenido y conservar el reporte completo. No modificar el CI ajeno para conseguir verde.
- La base remota no contiene `.github/workflows/`; la configuración CI/security local es trabajo ajeno no publicado por esta tarea. Los controles remotos correspondientes pueden quedar pendientes hasta integrar ese trabajo.
- Verificar alertas, push protection, Dependabot, CodeQL, rulesets, Autofix y Code Quality cuando existan capacidades y acceso apropiados. La ausencia de herramientas no demuestra ausencia de alertas.
- Mantener el PR draft: aceptación humana pendiente, sin aprobación, auto-merge ni fusión.
