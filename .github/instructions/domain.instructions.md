---
applyTo: "src/AulaPedidos.Domain/**"
---

# Domain: implementación y revisión

- No dependas de Application, Infrastructure, Api, HTTP, EF Core, proveedores SQL, DI ni Mediator. Las reglas deben poder probarse sin host ni base de datos.
- Añade entidades, eventos o interfaces solo cuando la tarea los requiera. Domain está vacío en la base actual; los ejemplos de productos y pedidos en documentación no son implementaciones existentes.
- Cuando se incorporen entidades, protege invariantes al construirlas y modificarlas; una entrada inválida no debe dejar un estado parcialmente actualizado. No hagas I/O desde métodos del dominio.
- Usa `decimal` para dinero y conserva el precio histórico cuando un caso de uso introduzca líneas de pedido. No inventes reglas de negocio: deriva límites y transiciones de criterios de aceptación.
- En revisión, comprueba pruebas unitarias de invariantes y transiciones modificadas: límites, valores inválidos y conservación de estado ante errores. Revisa referencias de proyecto y paquetes para detectar dependencias que crucen las capas.
