# Vivero con Producción Propia

Proyecto de Programación Orientada a Objetos (IUE, IF2002, grupo 604, 2026-2).
API web en C# con .NET 8 para un vivero que siembra sus propias plantas y vende plantas, insumos y herramientas.

## Integrantes
- Ana Sofía Geovo Jaramillo
- (integrante 2)
- (integrante 3)

## Cómo ejecutarlo
1. Instalar el SDK de .NET 8.
2. En una terminal, entrar a la carpeta `ViveroWebApi`.
3. Ejecutar `dotnet run`.
4. Abrir Swagger en `http://localhost:5166/swagger`.

## Estructura (SOLID)
| Carpeta | Qué contiene |
| --- | --- |
| `Dominio/Entidades` | Clases del negocio: Producto, Insumo, Herramienta, Especie, Lote, Labor, Cliente, Pedido, LineaPedido |
| `Dominio/Interfaces` | `ITipoCliente` y los tipos Particular, Jardinero y Empresa |
| `Dominio/Excepciones` | `DatoInvalidoException`, `PedidoInvalidoException` |
| `DTOs` | Request (lo que llega) y Response (lo que se devuelve), como `record` |
| `Mappers` | Convierten entre DTO y dominio |
| `Repositorios` | Interfaces de lectura y de escritura + implementación en memoria |
| `Servicios` | Lógica de la aplicación a través de interfaces |
| `Controllers` | Endpoints de la API |
| `Excepciones` | `RecursoNoEncontradoException` y el manejador global de errores |

## Endpoints
| Recurso | Endpoints |
| --- | --- |
| Insumos | `GET/POST /api/insumos`, `GET/PUT/DELETE /api/insumos/{codigo}` |
| Herramientas | `GET/POST /api/herramientas`, `GET/PUT/DELETE /api/herramientas/{codigo}` |
| Especies | `GET/POST /api/especies`, `GET/PUT/DELETE /api/especies/{codigo}` |
| Lotes | `POST /api/especies/{codigo}/lotes`, `.../lotes/{idLote}/bajas`, `.../lotes/{idLote}/listo`, `.../lotes/{idLote}/labores`, `.../lotes/{idLote}/trasplante` |
| Clientes | `GET/POST /api/clientes`, `GET/PUT/DELETE /api/clientes/{documento}` |
| Pedidos | `GET/POST /api/pedidos`, `GET /api/pedidos/{numero}`, `POST /api/pedidos/{numero}/lineas`, `/reservar`, `/despachar`, `/cancelar` |

## Estado
- [x] Clases del dominio con encapsulamiento, constructores, validaciones y enumeraciones
- [x] Descuento según tipo de cliente y tipo de producto, vencimiento de insumos, despacho parcial y cupo de crédito
- [x] Interfaz `ITipoCliente` y 2 excepciones propias
- [x] API por capas: controladores, DTOs, mappers, repositorios (en memoria) y servicios
- [ ] Base de datos SQLite
- [ ] Patrón de diseño
- [ ] Vistas web
- [ ] Documentación final
