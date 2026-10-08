# Expediente BASE-001 — Revisión de la base de AulaPedidos

## Identificación

- Repositorio: `C:\Users\Yael\Curso\CursoNetIA`
- Rama declarada por el entorno: `master`
- Remoto declarado: `origin` (`https://github.com/ychavez/CursoNetIA_`)
- Fase actual: E — preparación del expediente
- Modalidad: consultas manuales separadas en Microsoft Visual Studio
- Coordinación: humana; este expediente no representa invocaciones automáticas
- Implementación: no autorizada
- Cierre: pendiente

## Objetivo

Revisar la base técnica de AulaPedidos antes de implementar funciones de negocio. Identificar qué arquitectura, ADR, configuración, documentación, proyectos y pruebas existen realmente; qué falta; qué está descrito pero no comprobado; y qué decisiones deben resolverse antes de autorizar una implementación.

## Premisa aportada por la persona responsable

La persona responsable declara que todavía no se han implementado funciones de negocio.

Esta premisa no se ha comprobado contra el código. `docs/arquitectura.md` describe en presente endpoints, agregados, repositorios, handlers, persistencia y otros componentes, por lo que debe distinguirse entre arquitectura objetivo y estado implementado.

## Documentos revisados por el Coordinador

- `.github/copilot-instructions.md`
- `docs/multiagente-copilot.md`
- `docs/arquitectura.md`
- `docs/seguridad.md`
- `docs/expedientes/README.md`

No se afirma haber revisado el resto del repositorio.

## Estado exacto

Pendiente de completar antes de congelar el paquete común:

- Commit base: pendiente.
- Diff de cambios locales: pendiente.
- Archivos nuevos no registrados: pendiente.
- Inventario completo de `docs/`: pendiente.
- Inventario y contenido de `docs/adr/`: pendiente.
- Inventario de `src/`, `tests/` y `tools/`: pendiente.
- Solución y archivos de proyecto: pendiente.
- Configuración de aplicación, compilación, paquetes, contenedores y CI: pendiente.
- Estado real de implementación: pendiente.
- Comandos ejecutados: ninguno por el Coordinador.
- Resultados de compilación o pruebas: no aportados.

La ausencia de estos datos impide considerar exacto el estado base.

## Hallazgos preliminares

### Observados en documentación

1. Se define una arquitectura modular con Domain, Application, Infrastructure y Api.
2. Domain no debe depender de EF, HTTP ni Infrastructure.
3. Se prevén productos, pedidos, pertenencia por usuario, precios históricos, concurrencia y borrado lógico.
4. Se documentan mediador propio, `Result`, repositorios, outbox, caché y adaptador de notificaciones.
5. Se documentan Minimal APIs, JWT, permisos y autorización sobre el recurso.
6. Se contemplan SQL Server para contenedores y SQLite para desarrollo local.
7. El protocolo exige consultas A/B separadas, decisión humana y un único implementador.

### Pendientes de comprobación

1. Que los proyectos y referencias descritos existan.
2. Que las funciones de negocio no estén implementadas.
3. Que existan los ADR mencionados y estén vigentes.
4. Que `Program.cs`, endpoints, entidades, repositorios, migraciones y pruebas coincidan con la documentación.
5. Que la configuración no exponga secretos ni habilite modos locales en producción.
6. Que exista una base de pruebas reproducible.

## Alcance de la ronda

Incluye:

- Inventario de arquitectura, ADR, configuración y documentación.
- Contraste entre documentación y estado real.
- Identificación de decisiones ausentes, contradictorias u obsoletas.
- Evaluación de la preparación para comenzar funciones de negocio.
- Definición de una matriz inicial de pruebas y evidencia.

Excluye:

- Implementar o modificar funciones de negocio.
- Modificar código, pruebas, ADR, configuración o documentación técnica.
- Ejecutar comandos, compilaciones, pruebas o migraciones.
- Aprobar arquitectura, implementación o liberación.
- Introducir paquetes, servicios externos o nuevas abstracciones.

## Archivos permitidos para lectura

Una vez aportados y registrados en el paquete común:

- Solución y archivos `*.csproj`.
- `.github/copilot-instructions.md`.
- Documentación bajo `docs/`, incluidos todos los ADR.
- Configuración no secreta de la raíz y los proyectos.
- Código bajo `src/`.
- Pruebas bajo `tests/`.
- Documentación y proyectos relevantes bajo `tools/`.
- Diff y listado de archivos nuevos sanitizados.

No deben adjuntarse claves, tokens, contraseñas ni archivos locales con secretos.

## Asignación A/B

- Informe A: Arquitecto.
- Informe B: Pruebas.
- Independencia: chats nuevos y separados.
- Entrada: la misma versión congelada de `BASE-001.paquete-comun.md` y los archivos allí enumerados.
- Restricción: ninguno recibe el informe del otro.
- Escritura y comandos: prohibidos.
- Presupuesto: una consulta inicial por rol y, como máximo, una aclaración.
- Tiempo de referencia: 10 minutos para análisis y síntesis, ajustable por la persona responsable.

## Criterios de aceptación de la revisión

1. Existe un inventario rastreable de proyectos, ADR, documentación y configuración relevante.
2. Cada afirmación distingue observado, documentado, inferido y pendiente.
3. Se identifican contradicciones entre documentación, ADR, configuración y código.
4. Se separa infraestructura existente de funciones de negocio implementadas.
5. Se identifican decisiones necesarias antes de implementar.
6. Se incluye evidencia mediante rutas y referencias concretas.
7. Se propone una matriz de pruebas con casos positivos, negativos y de borde.
8. Se conservan desacuerdos entre A y B sin decidir por mayoría.
9. No se afirma haber ejecutado comandos que no se hayan ejecutado.
10. La decisión de implementar permanece pendiente de la persona responsable.

## Riesgos

- Tomar documentación aspiracional como evidencia de implementación.
- Declarar ausencias sin disponer de un inventario completo.
- Diseñar pruebas a partir del código en lugar de los requisitos.
- Omitir ADR o configuración no aportados.
- Exponer secretos al compartir configuración.
- Ampliar prematuramente la arquitectura antes de conocer la necesidad.
- Confundir la confirmación documental con autorización para implementar.

## Casos negativos y condiciones de parada

- Si falta el estado exacto, el paquete no se congela.
- Si A o B no se realiza, su informe permanece pendiente y no se inventa.
- Si cambia el commit, diff o una premisa relevante, se crea una nueva versión del paquete y se repiten los análisis afectados.
- Si aparecen secretos, se detiene su incorporación y se solicita una versión sanitizada.
- Si se agota el presupuesto, se registran pendientes y se devuelve la decisión a la persona responsable.

## Informes originales

- Informe A — Arquitecto: pendiente.
- Informe B — Pruebas: pendiente.

Los originales sólo se incorporarán después de finalizar ambas consultas y tras una nueva confirmación humana de guardado.

## Síntesis

Pendiente de ambos informes.

La síntesis deberá conservar procedencia y comparar: punto, A, B, evidencia, coincidencia o desacuerdo y comprobación necesaria.

## Decisión humana

Pendiente. No existe autorización para implementar.

## Evidencia de implementación y verificación

No aplica todavía:

- Implementador designado: pendiente.
- Diff autorizado: ninguno.
- Comandos ejecutados: ninguno aportado.
- Resultados de pruebas: ninguno aportado.
- Revisión final: pendiente.
- Cierre humano: pendiente.

## Confirmación documental

Al guardarse este contenido después de la confirmación explícita en la conversación activa, quedará registrado que la persona responsable autorizó exclusivamente la creación de:

- `docs/expedientes/BASE-001.md`
- `docs/expedientes/BASE-001.paquete-comun.md`

La confirmación no autoriza implementación, consultas con un estado incompleto, aceptación ni cierre.