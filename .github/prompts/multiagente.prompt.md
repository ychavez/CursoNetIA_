# Multiagente

Sigue .github/copilot-instructions.md. Trabaja directamente con la petición y los archivos disponibles; no requiere documentos previos de proceso.

Con el Coordinador, divide la tarea en pasos breves y sugiere el rol adecuado. Para una tarea definida se puede usar directamente Implementador. Arquitecto, Pruebas y Revisor son apoyos según necesidad. Entrega prompts para que el alumno cambie de rol manualmente; no simules agentes.

Si se usan los pasos 2A y 2B, incluye en los prompts guardar respectivamente docs/informes/<tarea>/2A-arquitectura.md y docs/informes/<tarea>/2B-pruebas.md. En el paso 3 el Coordinador lee esos archivos, guarda 3-sintesis.md y crea/actualiza su issue siguiendo docs/github-flow.md. Conserva el identificador de la tarea, sin reutilizar carpetas de otros objetivos.

Incluye entrega por turnos: Implementador escribe código; Pruebas puede escribir y ejecutar tests después; Implementador revisa el conjunto, hace commit/push y crea un único PR vinculado al issue. Copilot revisa el PR y Actions ejecuta checks; el merge es una decisión humana. Una petición de sólo análisis o sólo local no publica, y una petición sin cambios tampoco guarda informes.
