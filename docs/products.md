# CRUD de productos — TAREA-003

Contrato vigente: `id` (Guid generado por el servidor), `price` (decimal), `description` y `alternateDescription` (texto nullable). No existen `name` ni `sku` en este contrato.

| Operación | Ruta | Resultado |
|---|---|---|
| Crear | POST `/products` | 201, producto y Location consultable |
| Listar | GET `/products` | 200, array ordenado por Id; vacío `[]` |
| Consultar | GET `/products/{id}` | 200 o 404 |
| Actualizar completamente | PUT `/products/{id}` | 200 conservando Id, o 404 |
| Eliminar | DELETE `/products/{id}` | 204 sin cuerpo, o 404 |

Ejemplo de entrada POST/PUT:

```json
{"price": 12.50, "description": "Producto", "alternateDescription": null}
```

`price` debe estar presente y ser un decimal representable; JSON inválido, null como cuerpo o tipos incompatibles devuelven 400 sin escribir. Ambas descripciones pueden omitirse o ser null. PUT sustituye todos los valores editables: omitir una descripción la convierte en null. El Id se toma sólo de la ruta en PUT; no se puede elegir en POST ni cambiar en PUT. Guid mal formado devuelve 404 por la restricción de ruta `:guid`; Guid vacío o inexistente devuelve 404.

No se han aprobado reglas comerciales de precio no negativo ni obligatoriedad de textos. Se admiten precios negativos/cero, null, vacío y espacios, sin trim ni límites arbitrarios. No se añaden moneda, unicidad, filtros, paginación o autenticación. El CRUD sigue la exposición de la API existente; antes de exponerlo fuera del aula deben decidirse los controles de acceso adecuados. Errores técnicos se gestionan con ProblemDetails sin detalles de excepciones.

## SQLite y esquema

El precio usa TEXT según el proveedor SQLite de EF Core para conservar `decimal` sin convertirlo a double. No hay filtros/ordenaciones de precio en SQL. Repository consulta sin seguimiento; editar/borrar marcan explícitamente el estado y guardan mediante UnitOfWork con CancellationToken.

La migración `20261013235000_AddProducts` añade únicamente Products e historial EF; no recrea bases ni modifica tablas ajenas. El arranque HTTP **no migra**. Aplicación explícita del esquema, desde la raíz y después de revisar la conexión y respaldar datos:

```sh
dotnet run --project src/AulaPedidos.Api -- --migrate
dotnet run --project src/AulaPedidos.Api
```

Para una base aislada, proporcionar `--ConnectionStrings:Default="Data Source=RUTA_REVISADA"` al comando de migración y al arranque. Estos comandos operativos son instrucciones, no evidencia de ejecución sobre la base del usuario. La implementación se verifica con bases aisladas de pruebas.

La base de partida no tenía modelo de productos ni migraciones versionadas. Si un despliegue externo ya tiene una tabla Products (por ejemplo, Name/Sku del incremento histórico), **no aplicar sin analizar su esquema y datos**: esta migración inicial fallará ante la tabla existente, sin convertir ni borrar sus datos. La conversión de ese esquema ajeno no se presume. No ejecutar rollback en bases con datos que deban conservarse: Down elimina exclusivamente Products; sólo se prueba en SQLite efímero.

## Verificación

Pruebas HTTP con Mediator real y SQLite único por caso bajo TestResults; se verifica la conexión antes de migrar. Cubren ciclo CRUD, independencia de otros productos, errores, fronteras decimal, nulabilidad, identidad y cancelación. Las migraciones se comprueban aparte con datos ajenos preexistentes en SQLite en memoria. Resultados reales y controles remotos: `docs/informes/TAREA-003/05-implementacion.md` y PR #19.
