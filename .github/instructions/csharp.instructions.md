---
applyTo: "**/*.cs"
---

# C#: implementación y revisión

- Usa nombres de código en inglés y explicaciones en español. Respeta PascalCase en tipos y miembros públicos, camelCase en variables y parámetros, y `_camelCase` en campos privados.
- Mantén el formato existente y namespaces file-scoped. Respeta `.editorconfig` si se incorpora.
- Aplica SOLID según el problema; no exijas una interfaz por clase ni patrones sin un caso de uso. Domain protege invariantes, Application coordina, Infrastructure implementa detalles y Api adapta HTTP.
- El repositorio usa Mediator de martinothamar, Repository y Unit of Work. No sustituirlos por MediatR o un mediador propio ni asumir que Result ya existe.
- Usa `decimal` para montos cuando se incorporen entidades. Valida antes de modificar estado y conserva invariantes también ante fallos.
- Respeta nulabilidad, propaga `CancellationToken` en I/O y evita `.Result`/`.Wait()`. Revisa disposal, tiempos de vida de DI y dependencias scoped capturadas por singletons.
- Revisa inyección, acceso indebido a recursos, entradas sin límites, secretos y datos sensibles en logs cuando el cambio toque esas superficies. Señala la ruta concreta del fallo; no declares seguridad completa por una revisión favorable.
- Centra la revisión del PR en defectos y regresiones del diff: cita archivo, condición que los provoca, consecuencia y prueba faltante o corrección propuesta. Distingue defectos de sugerencias de estilo; evita exigir capacidades futuras o repetir hallazgos de otra capa.
- Contrasta arquitectura y patrones con código, contratos y el alcance vigente del issue. Los documentos históricos no demuestran capacidades implementadas. No inventes ejecución de tests, cobertura ni análisis de seguridad: cita los resultados disponibles de CI y declara lo pendiente.
