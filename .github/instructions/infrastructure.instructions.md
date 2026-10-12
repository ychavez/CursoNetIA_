---
applyTo: "src/AulaPedidos.Infrastructure/**"
---

# Infrastructure: implementación y revisión

- Implementa contratos de Application y mantén aquí EF Core, SQLite y adaptadores externos. No dependas de Api ni decidas permisos a partir de valores no confiables del cliente.
- Ya existen `AppDbContext`, `Repository<TEntity>` e `UnitOfWork` sobre SQLite. Regístralos como scoped y comprueba que repositorio y unidad de trabajo compartan el mismo contexto. No captures el contexto en singletons ni lo uses simultáneamente.
- `Query` y lecturas mantienen `AsNoTracking`; `AddAsync`, `Update` y `Remove` no llaman a `SaveChanges`. `SaveChangesAsync` confirma los cambios del scope y propaga la cancelación.
- `GetPagedAsync` valida número/tamaño de página positivos, evita desbordamiento del offset y aplica filtros, includes y orden antes de paginar. El orden sigue siendo opcional; quien necesite páginas estables debe proporcionarlo. No introduzcas un máximo o un total sin requisito.
- Usa consultas parametrizadas; no concatenes entradas en SQL. Las rutas de include y expresiones de orden deben definirse por código o listas permitidas, no trasladarse sin validar desde HTTP.
- Revisa errores de configuración sin imprimir cadenas con credenciales. No añadas migraciones destructivas, caché, outbox, reintentos o proveedores nuevos fuera del alcance; si se solicitan, prueba sus fallos y consistencia.
- Para cambios de persistencia revisa evidencia con SQLite relacional: lecturas sin seguimiento, escrituras visibles tras confirmar, cancelación y aislamiento por scope. SQLite no prueba traducción ni concurrencia de SQL Server. No reemplaces esas verificaciones por mocks que repitan la implementación.
