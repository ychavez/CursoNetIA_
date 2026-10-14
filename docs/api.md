# Contrato HTTP vigente — TAREA-003

La API usa Minimal APIs de ASP.NET Core y publica productos bajo `/products`, sin prefijo `/api/v1`. El contrato completo de productos y sus decisiones está en [CRUD de productos](products.md).

## Preparación y arranque

Desde la raíz, revisar la conexión SQLite y respaldar los datos antes de aplicar el esquema:

```sh
dotnet run --project src/AulaPedidos.Api -- --migrate
dotnet run --project src/AulaPedidos.Api
```

El primer comando aplica la migración aditiva y termina; repetirlo no recrea la base. El arranque HTTP no migra automáticamente. Para una base aislada, proporcionar la misma opción `--ConnectionStrings:Default="Data Source=RUTA_REVISADA"` a ambos comandos. Si ya existe una tabla Products externa, detenerse y analizar su compatibilidad antes de migrar. No ejecutar rollback sobre datos que deban conservarse.

## Rutas implementadas

| Método y ruta | Entrada | Éxito |
|---|---|---|
| POST `/products` | price, description, alternateDescription | 201, producto y Location |
| GET `/products` | Sin parámetros | 200, array por Id; vacío [] |
| GET `/products/{id}` | Guid | 200 producto |
| PUT `/products/{id}` | Guid de ruta y mismo cuerpo que POST | 200 producto; conserva Id |
| DELETE `/products/{id}` | Guid | 204 sin cuerpo |
| GET `/health` | Sin cuerpo | 200 Healthy, liveness; no comprueba SQLite |
| GET `/openapi/v1.json` | Sin cuerpo, sólo Development | 200 documento OpenAPI |

```json
{"price":12.50,"description":"Producto","alternateDescription":null}
```

La salida añade `id` Guid generado en servidor. El cuerpo no permite elegir ni reemplazar esa identidad. `price` es decimal requerido: ausencia, null, tipo incompatible, fuera de rango o JSON inválido devuelven 400 sin guardar. Ambas descripciones pueden omitirse o ser null. PUT sustituye todos los valores editables; omitir un texto lo deja null.

Sin aprobación de restricciones comerciales, se conservan precios negativos/cero, textos vacíos/espacios y descripciones nulas; no hay trim, redondeo, unicidad ni longitudes arbitrarias. Id inexistente o vacío devuelve 404; Guid mal formado devuelve 404 por la restricción `:guid`. Errores técnicos producen ProblemDetails sin detalles de excepciones.

## Límites del estado actual

No están implementados `/api/v1`, SKU, Name, Version, paginación, pedidos, soft delete, JWT, permisos, Swagger UI, rate limiting ni DevTools. La API conserva su exposición existente sin autenticación; antes de exponerla fuera del aula deben decidirse controles de acceso. La documentación histórica de la aplicación completa no acredita estas capacidades.

[Informe de implementación y evidencia](informes/TAREA-003/05-implementacion.md). Los comandos anteriores son instrucciones operativas, no evidencia de ejecución sobre la base del usuario.
