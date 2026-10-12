---
applyTo: "src/AulaPedidos.Application/**"
---

# Application: implementación y revisión

- Depende de Domain y define contratos para coordinar casos de uso. No referencies Infrastructure, Api, EF Core, proveedores SQL ni `HttpContext`.
- Usa `Mediator.Abstractions` de martinothamar para mensajes y handlers cuando existan casos de uso. El generador y `AddMediator` viven en Api; no crear un mediador propio ni añadir MediatR.
- Conserva `IRepository<TEntity>` e `IUnitOfWork` en `Abstractions/Persistence`. En BASE-002 se aceptó `IQueryable` para `Query` y la composición de `GetPagedAsync`; esa excepción no autoriza importar extensiones de EF Core en esta capa.
- Las lecturas del repositorio son sin seguimiento. Las escrituras solo se confirman con `IUnitOfWork.SaveChangesAsync`; revisa que el caso de uso respete esa unidad de trabajo y propague `CancellationToken`.
- No cambies silenciosamente semántica de ausencia, validación, orden, persistencia o cancelación de los contratos. `GetPagedAsync` devuelve una lista, acepta orden opcional y no garantiza orden estable si se omite.
- Cuando se incorporen recursos por usuario, decide acceso con una identidad confiable y comprueba pertenencia; un identificador del body no acredita propiedad. No añadas autenticación, Result, outbox o entidades ajenas al alcance del issue.
- En la revisión pide pruebas de comportamiento para decisiones y errores introducidos: entradas inválidas, ausencia, cancelación y permisos cuando apliquen. Vincula el hallazgo a código cambiado, consecuencia y criterio del issue; no uses el porcentaje de cobertura como prueba de corrección.
