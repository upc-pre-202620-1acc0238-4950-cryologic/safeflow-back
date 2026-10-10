<div align="center">
        <img src="https://upload.wikimedia.org/wikipedia/commons/f/fc/UPC_logo_transparente.png"><br>
</div>
<h3 align="center"> Universidad Peruana de Ciencias Aplicadas </h3>

<h3 align="center">Carrera de Ingeniería de Software </h3>

<h3 align="center">1ASI0730 </h3>
<h3 align="center">Aplicaciones Web </h3>
<h3 align="center"> NRC </h3>
<h3 align="center"> 20177 </h3>

<h3 align="center"> Informe de Trabajo Final</h3>


<h3 align="center"> Docente</h3>
<h3 align="center"> Jose Miguel, Flores Ingaruca </h3>


<h3 align="center"> Equipo </h3>
<h3 align="center"> Cryologic Systems </h3>

<h3 align="center"> Proyecto</h3>
<h3 align="center"> SafeFlow </h3>

<h3 align="center"> Integrantes </h3>

<div align = "center">
 
| Code       |      Member |
| :---:      |     :--- |
| U202218531 | Andy Alejandro Mio Mejia |
| U202116018 | Mel Andree Orellana Rodriguez |
| U202114701 | Angel Guillermo Berrospi Marin |
| U20221C803 | Anhelo Rodrigo Rocca Leon |
| U202316049 | Jefferson Bayron Morales Yapuchura |


</div>

<h3 align="center">Periodo 202610</h3>
<h3 align="center">Junio 2026</h3>


<div style="page-break-after: always;"></div>



# SafeFlow Platform — Backend API

> Plataforma tecnológica para el monitoreo y control en tiempo real de la cadena de frío en almacenamiento y transporte logístico. Backend REST desarrollado por **Cryologic Systems**.

---

## Descripción

API REST de SafeFlow para la supervisión térmica y la prevención de pérdidas en la cadena de frío.
Implementa el alcance de la aplicación móvil Android según `../requirements_specification.md`
(v0.10.0, TS-01 a TS-31 en Sprint 2). Android ya consume la API con Retrofit/OkHttp y JWT Bearer;
el backend conserva la autoridad de reglas, permisos y persistencia. La app emuladora
(FR-023 a FR-027) queda fuera de alcance.

| Aspecto | Detalle |
|---------|---------|
| Framework | ASP.NET Core Web API, .NET 10 (`net10.0`), namespace `SafeFlow.API` |
| Persistencia | EF Core 9 sobre MySQL (`MySql.EntityFrameworkCore`) |
| Seguridad | JWT Bearer (30 min, revocable), BCrypt, permisos por operación |
| Puerto local | `http://localhost:5115`, Swagger en `/swagger` (Development) |

## Ejecución local

- .NET 10 SDK y MySQL accesible.
- Cadena de conexión en `appsettings.Development.json` (no versionado):

```json
{ "ConnectionStrings": { "DefaultConnection": "server=127.0.0.1;port=3306;user=<usuario>;password=<clave>;database=safeflow_db;sslmode=None;AllowPublicKeyRetrieval=True" } }
```

```sh
dotnet build
dotnet run --launch-profile http
```

Al arrancar se crean la base y el esquema, y se insertan solo las cuentas de prueba y sus permisos:
`operador.test1@gmail.com`, `supervisor.test1@gmail.com` y `admin.test1@gmail.com`. Las listas
operativas empiezan vacías.

La APK Android debug usa `http://10.0.2.2:5115/api/v1/` para acceder a esta API desde el emulador.
Solo debug permite HTTP para `10.0.2.2`; release usa un placeholder HTTPS que debe reemplazarse
antes de desplegar. Android conserva token, vencimiento, cuenta y permisos en DataStore y refresca
sus colecciones en memoria desde la API después de las mutaciones, sin Room ni seeds locales.
Verificar la integración requiere mantener la API y MySQL activos; las pruebas con fixtures
operativos Android fueron retiradas. Los flujos térmicos requieren datos de sensores en el backend.

## Arquitectura

Monolito modular con Domain-Driven Design: un bounded context por carpeta (Iam, Inventory,
Monitoring, Alerts, Logistics, Reporting) más el kernel `Shared`. Cada contexto tiene las capas
Domain, Application (CQRS ligero), Infrastructure (EF Core) e Interfaces (REST y fachadas ACL). La
fila hija guarda el id del padre con índice; las claves foráneas reales solo existen dentro de un
mismo contexto. Detalle en [ARCHITECTURE.md](ARCHITECTURE.md).

## Endpoints (`/api/v1`, `Authorization: Bearer <jwt>`)

| Contexto | Endpoint | Acceso |
|----------|----------|--------|
| IAM | `POST auth/sign-up`, `POST auth/sign-in`, `POST recovery-requests` | Anónimo |
| IAM | `POST auth/sign-out`, `GET users/me` | Sesión válida |
| IAM | `GET role-permissions`, `PUT role-permissions/{role}` | ADMIN |
| Inventory | `POST products`, `POST products/{id}/lots`, `POST lots/{id}/incomes` | WRITE_INVENTORY |
| Inventory | `GET products?condition=`, `GET products/{id}` | READ_INVENTORY, WRITE_INVENTORY, CREATE_SHIPMENT, RISKS |
| Inventory | `GET products/{id}/lots`, `GET lots/{id}` | READ_INVENTORY, WRITE_INVENTORY, CREATE_SHIPMENT |
| Monitoring | `GET sensors`, `GET readings`, `GET rejected-readings`, `GET risks` | Según operación (inventario, despachos o RISKS) |
| Alerts | `GET incidents`, `GET incidents/{id}` | ALERTS o RESOLVE |
| Alerts | `POST incidents/{id}/resolution` | RESOLVE |
| Logistics | `POST shipments` | CREATE_SHIPMENT |
| Logistics | `GET shipments?status=`, `GET shipments/{id}` | READ_SHIPMENTS, DELIVER, CREATE_SHIPMENT |
| Logistics | `GET shipments/{id}/validation`, `POST shipments/{id}/departure` | READ_SHIPMENTS |
| Logistics | `POST shipments/{id}/delivery` | DELIVER |
| Reporting | `GET operational-summary` | SUMMARY |

`POST auth/sign-up` devuelve 201; `POST auth/sign-in` devuelve 200;
`POST auth/sign-out` y `POST recovery-requests` devuelven 204.
Swagger agrupa las operaciones en seis etiquetas: Iam, Inventory, Monitoring, Alerts, Logistics y
Reporting, cada una con una descripción breve en español.

Los errores responden `{ "error": "<código>", "message": "<mensaje>" }`; sin token, 401; sin
permiso, 403.

## Despliegue (CI/CD)

`.github/workflows/main_safeflow-api.yml` compila y publica en Azure Web App `safeflow-api`. En
producción, `ConnectionStrings:DefaultConnection` y `TokenSettings:Secret` se configuran fuera del
repositorio.
