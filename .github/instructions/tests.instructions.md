---
applyTo: "tests/**"
---

# Pruebas: implementación y revisión

- Prueba comportamiento observable con Arrange/Act/Assert y resultados derivados del issue. Prefiere casos de límites, errores y regresión a pruebas que solo repiten la implementación.
- Usa unitarias para reglas aisladas, `WebApplicationFactory<Program>` para HTTP y SQLite en memoria con conexión abierta para persistencia relacional. Aísla datos por prueba y libera conexiones/contextos; no uses credenciales ni servicios de producción.
- En persistencia verifica lecturas sin seguimiento, filtros, paginación, cancelación y que las escrituras no se persistan antes de `SaveChangesAsync`, observando desde otro contexto cuando corresponda.
- Mantén ArchitectureTests sobre dirección de dependencias y composición. Las listas exactas de tipos, proyectos, rutas y paquetes describen la base actual: al introducir una funcionalidad autorizada, actualiza esas expectativas justificadamente y conserva las prohibiciones de dependencias entre capas.
- Para funcionalidades con identidad incluye sin token, sin permiso y recurso ajeno; para invariantes comprueba que el fallo conserva estado. No inventes esos endpoints mientras no formen parte de la tarea.
- En revisión comprueba que quitar pruebas, ignorarlas o ampliar exclusiones de cobertura no oculte regresiones. La cobertura de líneas/ramas complementa la calidad de las aserciones; no acredita por sí sola cumplimiento o seguridad.
- Sigue la configuración y umbrales de cobertura versionados, sin duplicar cifras en instrucciones. Justifica cualquier cambio de umbral o exclusión; no rebajes controles para obtener un check verde.
- Reporta comando, resultado real, pruebas fallidas/omitidas y artefacto de cobertura. Un test no ejecutado queda pendiente; no lo declares aprobado. Copilot revisa el diff y la evidencia disponible, mientras Actions ejecuta las comprobaciones configuradas.
