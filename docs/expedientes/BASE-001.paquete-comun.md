# Paquete común BASE-001 — Revisión de la base de AulaPedidos

> Estado: borrador no congelado. No iniciar las consultas A/B hasta completar el estado exacto y declarar una versión congelada.

## Objetivo

Examinar la base técnica de AulaPedidos antes de implementar funciones de negocio. Determinar qué existe realmente en arquitectura, ADR, configuración, documentación, proyectos y pruebas; qué falta; y qué decisiones y evidencias se necesitan antes de proponer una implementación.

## Repositorio y estado

- Directorio: `C:\Users\Yael\Curso\CursoNetIA`
- Rama declarada por el entorno: `master`
- Remoto declarado: `origin` (`https://github.com/ychavez/CursoNetIA_`)
- Commit: pendiente.
- Diff local: pendiente.
- Archivos nuevos: pendiente.
- Compilación y pruebas: no aportadas.
- Premisa humana: todavía no se han implementado funciones de negocio.
- Comprobación de la premisa contra el código: pendiente.

## Contexto comprobado por el Coordinador

Se revisaron:

- `.github/copilot-instructions.md`
- `docs/multiagente-copilot.md`
- `docs/arquitectura.md`
- `docs/seguridad.md`
- `docs/expedientes/README.md`

No se ha realizado una búsqueda ni un inventario completo del repositorio.

## Arquitectura documentada

La documentación describe:

- API modular desplegada como una unidad.
- Domain sin dependencias de EF, HTTP ni Infrastructure.
- Application para casos de uso, DTO, puertos, mediador y `Result`.
- Infrastructure para persistencia, adaptadores, caché y outbox.
- Api como composición y transporte mediante Minimal APIs.
- Productos y pedidos con propiedad por usuario, precio histórico, concurrencia y borrado lógico.
- SQL Server para contenedores y SQLite para desarrollo local.
- JWT, permisos y autorización sobre el recurso.

Estas descripciones no prueban por sí mismas que los componentes estén implementados.

## Reglas y restricciones

1. No editar archivos ni ejecutar comandos.
2. No proponer MediatR ni sustituir el mediador y `Result` existentes sin una necesidad demostrada.
3. No añadir paquetes, capas, servicios externos o abstracciones sin justificación concreta.
4. No introducir dependencias de EF, HTTP o Infrastructure en Domain.
5. Diferenciar autenticación, permiso y pertenencia del recurso.
6. No exponer secretos, tokens, claves ni datos personales.
7. Marcar cada afirmación como observada, documentada, inferida o pendiente.
8. No tratar la compilación, si posteriormente se aporta, como prueba suficiente de seguridad o comportamiento.
9. No autorizar implementación ni cierre.

## Alcance

### Incluido

- Arquitectura y referencias entre proyectos.
- ADR existentes y cobertura de decisiones.
- Configuración de aplicación, compilación, paquetes, contenedores y CI.
- Coherencia entre documentación y código.
- Infraestructura de pruebas y criterios previos al desarrollo.
- Riesgos de seguridad y reproducibilidad.

### Excluido

- Edición de código, pruebas o documentación.
- Implementación de catálogo o pedidos.
- Ejecución de build, test, migraciones o despliegues.
- Aprobación final de arquitectura o liberación.

## Evidencia pendiente antes de congelar

- Commit y diff sanitizado.
- Inventario completo de documentación y ADR.
- Inventario de proyectos y archivos de configuración.
- Solución y archivos de proyecto.
- Código relevante de composición y fronteras.
- Inventario de pruebas.
- Versiones relevantes de SDK y paquetes.
- Estado de CI, contenedores y persistencia.

## Criterios de aceptación

- Inventario respaldado por rutas concretas.
- Tabla de existente, parcial, ausente y no comprobado.
- Contradicciones y ADR faltantes identificados.
- Separación entre infraestructura y funciones de negocio.
- Riesgos priorizados con evidencia necesaria.
- Alternativas explícitas y condiciones que las invalidarían.
- Matriz de pruebas derivada de requisitos.
- Preguntas que requieran decisión humana.

## Consulta A — Arquitecto

Analiza exclusivamente este paquete congelado y los archivos de contexto autorizados. No edites ni ejecutes comandos.

Contrasta código, ADR, configuración y documentación. Separa observado directamente, documentado, inferido y pendiente.

Determina:

1. Qué estructura técnica existe realmente.
2. Qué decisiones están respaldadas por ADR.
3. Qué decisiones documentadas no están implementadas.
4. Qué elementos implementados carecen de decisión o documentación.
5. Qué contradicciones, ADR ausentes u obsoletos existen.
6. Si las dependencias respetan las fronteras de las capas.
7. Qué infraestructura mínima falta antes de implementar negocio.
8. Qué componentes serían prematuros y deberían posponerse.

Entrega:

- Evidencia con rutas y referencias.
- Supuestos.
- Alternativas y costes.
- Riesgos priorizados.
- Preguntas para decisión humana.
- Secuencia recomendada.
- Condiciones que invalidarían la recomendación.

No autorices implementación.

## Consulta B — Pruebas

Analiza exclusivamente este paquete congelado y los archivos de contexto autorizados. No edites, no ejecutes comandos y no consultes el informe A.

Separa observado directamente, documentado, inferido y pendiente.

Determina:

1. Qué infraestructura de pruebas existe realmente.
2. Qué requisitos son comprobables actualmente.
3. Qué criterios de aceptación faltan antes del desarrollo.
4. Qué riesgos de seguridad, configuración y aislamiento deben probarse.
5. Qué pruebas de arquitectura, composición, autenticación, autorización, persistencia y errores son necesarias.
6. Qué casos positivos, negativos y de borde deben definirse para catálogo y pedidos.
7. Qué configuración o dependencias dificultan pruebas reproducibles.

Entrega una matriz con:

- Caso.
- Origen del requisito.
- Precondición.
- Acción.
- Resultado esperado.
- Tipo de prueba.
- Evidencia requerida.
- Estado: posible ahora, bloqueado o pendiente.

No afirmes haber ejecutado pruebas y no derives expectativas únicamente del código existente.

## Entrega de informes

Cada rol entregará su informe original en una conversación separada. Ningún informe se añadirá a este paquete. El Coordinador sólo los incorporará al expediente después de terminar ambas consultas y tras confirmación humana de un nuevo guardado.