# tarea-actual — 2B Pruebas: operaciones por lote

## Objetivo

Evaluar las operaciones por lote frente al código y a los tests reales. Comprobar persistencia diferida, colección nula, colección vacía y que Application no referencia EF Core. No se implementa ni se ejecuta nada en este paso.

## Rol

Pruebas (paso 2B). Análisis de código y de tests existentes. No se modificó código, tests ni configuración. No se ejecutaron comandos.

## Archivos realmente consultados

- `.github/copilot-instructions.md` (instrucción del espacio de trabajo).
- `.github/instructions/application.instructions.md`.
- `.github/instructions/infrastructure.instructions.md`.
- `.github/instructions/tests.instructions.md`.
- `docs/multiagente-copilot.md` (inicio).
- `docs/repositorio-generico.md`.
- `docs/arquitectura.md` (inicio).
- `docs/expedientes/BASE-002.md`.
- `docs/expedientes/BASE-001.md` (inicio).
- `src/AulaPedidos.Application/AulaPedidos.Application.csproj`.
- `src/AulaPedidos.Application/DependencyInjection.cs`.
- `src/AulaPedidos.Application/Abstractions/Persistence/IRepository.cs`.
- `src/AulaPedidos.Application/Abstractions/Persistence/IUnitOfWork.cs`.
- `src/AulaPedidos.Domain/AulaPedidos.Domain.csproj`.
- `src/AulaPedidos.Infrastructure/AulaPedidos.Infrastructure.csproj`.
- `src/AulaPedidos.Infrastructure/DependencyInjection.cs`.
- `src/AulaPedidos.Infrastructure/Persistence/Repository.cs`.
- `src/AulaPedidos.Infrastructure/Persistence/UnitOfWork.cs`.
- `src/AulaPedidos.Infrastructure/Persistence/AppDbContext.cs`.
- `tests/AulaPedidos.Infrastructure.Tests/RepositoryTests.cs`.
- `tests/AulaPedidos.Infrastructure.Tests/InfrastructureCompositionTests.cs`.
- `tests/AulaPedidos.Infrastructure.Tests/TestEntity.cs`.
- `tests/AulaPedidos.Infrastructure.Tests/AulaPedidos.Infrastructure.Tests.csproj`.
- `tests/AulaPedidos.ArchitectureTests/ArchitectureTests.cs` (secciones de fronteras y contrato).
- `tests/AulaPedidos.ArchitectureTests/CompositionTests.cs` (inicio).
- `tests/AulaPedidos.ArchitectureTests/AulaPedidos.ArchitectureTests.csproj`.
- `tests/AulaPedidos.Api.IntegrationTests/ApiTests.cs`.
- `docs/informes/BASE-002/2B-Pruebas.md` (solo para comprobar que es otro objetivo; no se usó para formar esta propuesta).
- `docs/informes/BASE-002/2A-Arquitectura.md` (inicio; otro objetivo; no se usó para formar esta propuesta).
- `docs/informes/BASE-002/3-implementracion.md` (solo como declaración ajena de una ejecución previa).

No localizados:

- `docs/paquetes/BASE-001-paquete-comun.md`.
- `docs/informes/tarea-actual/` antes de este informe.
- `coverlet.runsettings` en la raíz.
- `src/AulaPedidos.Application/obj/project.assets.json`.

## Conclusiones

Las operaciones por lote no existen en el contrato ni en la implementación ni en los tests. No hay `AddRange`, `UpdateRange`, `RemoveRange` ni `GetByIdsAsync`. `IRepository<TEntity>` solo declara escrituras unitarias: `AddAsync`, `Update` y `Remove`. `Repository<TEntity>` las implementa sobre una entidad y no llama a `SaveChanges`.

Por eso, los tres comportamientos de lote pedidos no están cubiertos:

- Persistencia diferida de un lote: no comprobable. El único caso real es unitario.
- Colección nula: no hay parámetro de colección de entidades. El rechazo de `null` cubre una entidad o un contexto, no un lote.
- Colección vacía: no hay operación de escritura por colección. Las listas vacías de los tests son resultados de lectura, no entradas de un lote.

Application no declara referencia a EF Core. Eso sí se observa en el archivo de proyecto. No se ejecutó la prueba de arquitectura que lo afirma, ni se inspeccionó el grafo restaurado: `project.assets.json` de Application no estaba en la ruta consultada.

`docs/repositorio-generico.md` describe `GetByIdsAsync` y una carga por lote. Es documentación de un objetivo, no el contrato vigente. El código actual no tiene ese método.

## Análisis del código

Contrato vigente en `IRepository<TEntity>`, métodos públicos:

`GetAsync`, `FirstOrDefaultAsync`, `AnyAsync`, `CountAsync`, `Query`, `GetPagedAsync`, `AddAsync`, `Update`, `Remove`.

La prueba `PersistenceContractsAreStorageIndependent` fija exactamente esa lista. Si se añaden métodos de lote sin actualizarla, esa aserción dejaría de coincidir con el contrato. Esto es lectura del test, no una ejecución.

Escrituras unitarias:

- `AddAsync`, `Update` y `Remove` lanzan `ArgumentNullException` si la entidad es nula.
- No llaman a `SaveChanges`. La persistencia queda en `UnitOfWork.SaveChangesAsync`, que delega en `AppDbContext.SaveChangesAsync`.
- El comentario del contrato dice que las escrituras no se persisten hasta `IUnitOfWork.SaveChangesAsync`.

Único manejo de una colección nula observado, y no es un lote de entidades: `GetPagedAsync` hace `includeStrings ?? []`. No hay un test que pase `includeStrings` nulo. No sustituye el caso de colección de entidades nula.

`IRepository` usa `System.Linq.Expressions` e `IQueryable`. Eso no es una referencia de paquete a EF Core. Sí expone `IQueryable`, que es un detalle de consulta; queda fuera del chequeo de referencia pedido y no se propone cambiarlo aquí.

## Tests reales leídos

`RepositoryTests` comparte una conexión SQLite en memoria (`Data Source=:memory:`, abierta en `InitializeAsync`) y crea contextos independientes con `UseSqlite(_connection)`. El caso `WritesOnlyPersistAfterSaveChanges` afirma, para una sola entidad:

- tras `AddAsync` y antes de guardar, otro contexto cuenta 5;
- `SaveChangesAsync` devuelve 1 y otro contexto cuenta 6;
- tras `Update` y guardar, `Remove` aún no borra en otro contexto;
- tras el último `SaveChangesAsync`, otro contexto vuelve a contar 5.

Ese diseño sirve de patrón para el lote, pero no lo cubre: no recibe una colección, no prueba colección nula ni vacía, y la entidad actualizada ya estaba rastreada por el `AddAsync` del mismo test.

`NullEntitiesAreRejected` rechaza entidad nula en `AddAsync`, `Update` y `Remove`, y contexto nulo en los constructores. No hay colección.

Lecturas vacías existentes, distintas de una colección de escritura vacía:

- `GetAsync` con predicado sin coincidencias afirma lista vacía.
- `GetPagedAsync` de una página fuera de rango afirma lista vacía.

`ApiTests` resuelve `IRepository<>` e `IUnitOfWork` por scope. No ejercita escrituras ni lotes.

Recuento por lectura de fuente, no por ejecución: en `RepositoryTests` hay 8 `[Fact]` y un `[Theory]` con 3 `[InlineData]`; en `InfrastructureCompositionTests` hay 2 `[Fact]`. Ninguno es de lote.

## Evidencia ejecutada por otros

`docs/informes/BASE-002/3-implementracion.md` declara `dotnet build` correcto y `dotnet test` con 45 pruebas superadas (11 Api, 21 Architecture, 13 Infrastructure). No lo ejecuté ni lo verifiqué. Ese informe no menciona operaciones por lote. Aunque esa ejecución hubiera sido correcta, los tests que leí no contienen los casos de lote, así que no acreditaría persistencia diferida de colección, colección nula ni colección vacía.

No hay salidas de prueba en esta sesión.

## Tabla de casos

Los esperados de lote se derivan del contrato ya escrito para la escritura unitaria («no persiste hasta `SaveChangesAsync`») y del rechazo de `null` ya implementado. No existe un expediente de lote que los haya aprobado. La colección vacía y el elemento nulo dentro del lote son propuesta, no decisión humana.

| Requisito | Caso | Entrada | Resultado esperado | Comprobación |
|---|---|---|---|---|
| Persistencia diferida de alta por lote | Añadir varias entidades y leer con otro contexto antes de guardar | 2 `TestEntity` nuevas sobre las 5 semillas; conexión SQLite en memoria compartida | El otro contexto sigue contando 5. Detecta un `SaveChanges` oculto dentro del alta por lote | Ausente. El patrón unitario está en `WritesOnlyPersistAfterSaveChanges`; no lo ejecuté |
| Persistencia diferida de alta por lote tras guardar | Alta por lote y `SaveChangesAsync` | Las mismas 2 entidades y un guardado sin otros cambios pendientes | `SaveChangesAsync` devuelve 2; otro contexto cuenta 7 | Ausente |
| Persistencia diferida de actualización por lote | Modificar varias entidades ya persistidas, sin guardar | Cambiar `Name` de dos semillas y llamar a la actualización por lote | Otro contexto conserva los nombres originales. Detecta persistencia inmediata o que solo se marque la primera | Ausente. El test unitario no parte de entidades leídas sin seguimiento |
| Persistencia diferida de eliminación por lote | Eliminar varias entidades, sin guardar | Dos semillas existentes | Otro contexto sigue encontrándolas. Tras guardar, ya no están y el conteo baja en 2 | Ausente |
| Colección nula | Alta, actualización y eliminación por lote con colección nula | `null` | `ArgumentNullException` y ningún cambio rastreado. Detecta un `NullReferenceException` tardío o un lote ignorado en silencio | Ausente. Analogía con `NullEntitiesAreRejected`; no es un requisito escrito de lote |
| Colección vacía | Alta, actualización y eliminación por lote con colección vacía | colección vacía, no nula | No lanza; no marca entidades; otro contexto no cambia; si no hay otros pendientes, `SaveChangesAsync` devuelve 0. Detecta una excepción o un efecto colateral | Ausente. Comportamiento propuesto, no decidido |
| Elemento nulo dentro del lote | Colección con una entidad y un `null` | por ejemplo una entidad válida y un `null` | Propuesta: `ArgumentNullException` y ninguna entidad de ese lote queda marcada. Detecta un alta parcial | No pedido como requisito confirmado; caso adicional propuesto |
| Application sin EF Core | El contrato de lote, si se añade, y el proyecto Application | `AulaPedidos.Application.csproj` y los tipos de los parámetros | Sin `PackageReference` ni `ProjectReference` a EF Core o SQLite. La colección debe ser un tipo de BCL (`IReadOnlyCollection<TEntity>` o `IEnumerable<TEntity>`), no un tipo de EF. Detecta que el lote arrastre `DbSet`, `EntityEntry` o el paquete | El csproj actual cumple: solo referencia Domain y los paquetes `Mediator.Abstractions` y `Microsoft.Extensions.DependencyInjection.Abstractions`. No ejecuté `ArchitectureTests` ni leí `project.assets.json` |

## Propuestas para el Implementador

No añadir métodos de lote hasta que el humano confirme el contrato. Si se autoriza, pasos concretos:

1. Mantener las firmas en Application, sin tipos ni paquetes de EF Core. Nombres propuestos, no decididos: `AddRangeAsync`, `UpdateRange` y `RemoveRange`. Deben describir la acción y aceptar una colección de BCL.
2. Rechazar la colección nula con `ArgumentNullException` antes de marcar entidades. No llamar a `SaveChanges` dentro del repositorio.
3. Tratar la colección vacía como operación sin efecto, salvo que el humano decida otro comportamiento. Confirmar también el elemento nulo dentro de la colección: la propuesta es rechazar el lote completo, sin marcar una parte.
4. Añadir tests en `tests/AulaPedidos.Infrastructure.Tests`, reutilizando la conexión SQLite en memoria y un segundo contexto, como `WritesOnlyPersistAfterSaveChanges`. Cubrir alta, actualización y eliminación: invisible antes de `SaveChangesAsync`, visible después, con el número de filas devuelto.
5. Añadir los casos de colección nula y vacía. No sustituirlos por el test unitario de entidad nula ni por una consulta que devuelve lista vacía.
6. Si se añaden métodos, actualizar la lista esperada de `PersistenceContractsAreStorageIndependent`. Si no se actualiza, el test de arquitectura dejará de coincidir con el contrato.
7. No añadir la referencia de EF Core a Application. El csproj ya la excluye; el cambio de lote no debe introducirla.
8. Ejecutar `dotnet build CursoNETIA.slnx` y `dotnet test CursoNETIA.slnx`. Esta sesión no los ejecutó.

Fuera de este informe: no cambiar `docs/informes/BASE-002/2B-Pruebas.md`. Su objetivo es Repository y Unit of Work, no el lote.

## Supuestos

- «Operaciones por lote» significa escrituras que reciben una colección de entidades, porque se pidió persistencia diferida. Una consulta `GetByIdsAsync` no cumpliría ese chequeo y, además, no está implementada.
- El rechazo de la colección nula se propone por analogía con `ArgumentNullException` de la entidad nula. No hay una regla de lote aprobada en un expediente.
- La colección vacía no está especificada. El esperado «sin efecto y sin excepción» es propuesta.
- No se usaron los informes 2A ni el 2B de BASE-002 para definir estos esperados.
- La ausencia de `project.assets.json` no demuestra que el grafo restaurado esté limpio ni que esté sucio.

## Comprobaciones

Realizadas en esta sesión, solo por lectura:

- Contrato, implementación y tests de persistencia.
- `AulaPedidos.Application.csproj` no referencia EF Core ni SQLite.
- No existe informe previo en `docs/informes/tarea-actual/`.
- `docs/informes/BASE-002/2B-Pruebas.md` pertenece a otro objetivo y se conservó.

No realizadas:

- `dotnet build CursoNETIA.slnx`.
- `dotnet test CursoNETIA.slnx`.
- Inspección de ensamblados compilados o de `project.assets.json`.
- Cualquier caso de lote: no hay código ni test que ejecutar.

## Revisión de nombres

No hay nombres de operaciones por lote que revisar. Los nombres vigentes del contrato (`AddAsync`, `Update`, `Remove`, `SaveChangesAsync`, `IRepository`, `IUnitOfWork`) describen la acción unitaria y no se proponen cambios estéticos. `includeStrings` arrastra un detalle de inclusión por cadena; no forma parte de este objetivo y no se pide renombrarlo aquí.

## Siguiente paso

El humano decide si autoriza el contrato de lote, incluido el comportamiento de colección vacía y de elemento nulo. Hasta esa decisión, el Implementador no debe añadir métodos. Si se autoriza, implementa los casos de la tabla y ejecuta build y test. Este informe no acredita una ejecución ni una aceptación.

Ruta guardada: `docs/informes/tarea-actual/2B-pruebas.md`.
