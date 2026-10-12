---
applyTo: "src/AulaPedidos.Api/**"
---

# API: implementación y revisión

- Usa Minimal APIs. Los endpoints traducen contratos HTTP y delegan los casos de uso a Application; no acceden directamente a `DbContext` ni implementan reglas del dominio.
- La referencia a Infrastructure se usa para composición en `Program.cs`. Registra dependencias y middleware con tiempos de vida compatibles.
- Conserva Mediator de martinothamar: `Mediator.SourceGenerator` pertenece a Api, con `PrivateAssets="all"`; `AddMediator` registra handlers de Application con lifetime scoped. No introducir MediatR ni un mediador propio.
- Comprueba rutas, DTOs, validación, códigos HTTP y OpenAPI contra el contrato del issue. Usa ProblemDetails para los errores cuando se incorporen endpoints que lo requieran; no exponer excepciones internas ni secretos.
- Cuando el cambio introduzca datos protegidos, valida identidad, permisos y acceso al recurso. No confíes en owner, rol o importe recibidos del cliente; añade límites de entrada y paginación donde sean necesarios.
- No amplíes CORS ni omitas validación de tokens para solucionar pruebas. Revisa que logs y configuración no expongan credenciales o datos personales.
- La base actual publica `/health` en todos los entornos y OpenAPI solo en Development; no exige autenticación de negocio todavía. No declares fallos por funcionalidades futuras descritas en documentos de la aplicación completa.
- Para cambios HTTP revisa pruebas de integración del contrato y sus casos negativos. Los resultados de tests, cobertura y seguridad se acreditan con los checks de Actions; la revisión de Copilot no los ejecuta por sí sola.
