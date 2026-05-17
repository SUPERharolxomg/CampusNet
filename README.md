# CampusNet — Red Social Académica Dirigida

Módulo interno que simula una **red social dirigida** de usuarios académicos usando un grafo con **lista de adyacencia** implementado en **C# (.NET 8)**, siguiendo arquitectura **MVC estricta**.

---

## Integrantes y Responsabilidades

| Integrante | Rol en el proyecto |
|---|---|
| Harold David Garces Casas | Tech Leader — Modelos: `Vertex.cs`, `Edge.cs`, `Graph.cs` (estructura del grafo, BFS, DFS, CRUD) |
| Luisa Fernanda Gallego Serna | Full Stack Developer — Vista: `GraphView.cs`, Controlador: `GraphController.cs`, `Program.cs`, integración y casos de uso |

---

## Estructura del Proyecto

```
CampusNet/
├── CampusNet.csproj
├── Program.cs                    <- solo invoca al Controller
├── Models/
│   ├── Vertex.cs                 <- usuario (id, nombre, rol)
│   ├── Edge.cs                   <- arista dirigida (from -> to)
│   └── Graph.cs                  <- grafo con lista de adyacencia
├── Views/
│   └── GraphView.cs              <- impresion en consola e input de usuario
└── Controllers/
    └── GraphController.cs        <- casos de uso y orquestacion
```

---

## Arquitectura MVC

| Capa | Clase | Responsabilidad |
|---|---|---|
| **Model** | `Vertex`, `Edge`, `Graph` | Estructura de datos, algoritmos (BFS/DFS/CRUD). Sin `Console`. |
| **View** | `GraphView` | Impresion en consola y lectura de input del usuario. Sin logica de negocio. |
| **Controller** | `GraphController` | Orquesta todos los casos de uso. Comunica Model con View. |

---

## Casos de Uso Implementados

| # | Caso de Uso | Descripcion |
|---|---|---|
| 1 | **Construccion** | 12 usuarios + 20 relaciones dirigidas (automatico al iniciar) |
| 2 | **Lista de adyacencia** | Impresion completa del grafo |
| 3 | **BFS x3** | Desde U01, U05 y U08 |
| 4 | **DFS completo** | Deteccion de ciclos con back-edges |
| 5 | **Consultas sociales** | Sin seguidores, influyentes, activos, alcanzabilidad |
| 6 | **CRUD interactivo** | Menu en consola para agregar, eliminar y actualizar usuarios y relaciones |

---

## Requisitos del Grafo Cumplidos

- [x] >= 12 vertices (usuarios)
- [x] >= 18 aristas (relaciones dirigidas) — el grafo inicial tiene 20
- [x] Al menos 2 usuarios con grado de salida >= 4 (U02 y U05)
- [x] Al menos 1 ciclo dirigido: **U01 -> U02 -> U03 -> U01**
- [x] Al menos 2 nodos sin seguidores (grado entrada = 0): **U11 y U12**

---

## Decision de Diseño: UUID interno + Codigo de Estudiante

Cada usuario maneja dos identificadores con responsabilidades distintas:

| Campo | Tipo | Proposito |
|---|---|---|
| `Id` | UUID (`Guid`) | Clave interna del grafo. Auto-generado, nunca visible al usuario. Garantiza unicidad absoluta independiente de cualquier dato externo. |
| `CodigoEstudiante` | string | Identificador legible (ej. `EST001`). Es el campo que el usuario ingresa en el CRUD y el que aparece en la lista de adyacencia y los recorridos. |

**Por que esta separacion?**
En sistemas reales, los codigos estudiantiles son asignados por reglas institucionales que pueden cambiar (cambio de carrera, reingreso, migracion entre sistemas). Si el grafo usara el codigo como clave primaria, cualquier cambio en el codigo romperia todas las relaciones almacenadas. Al desacoplar la identidad interna (UUID) de la identidad de negocio (codigo), el grafo permanece intacto aunque el codigo del estudiante cambie — solo se actualiza el campo, no la estructura.

---

## Como Compilar y Ejecutar

### Requisitos

- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0

### Pasos

```bash
# Clonar el repositorio
git clone <URL-del-repositorio>
cd CampusNet

# Compilar
dotnet build

# Ejecutar
dotnet run
```

Al ejecutar, el programa construye el grafo inicial, muestra recorridos y consultas de forma automatica, y luego despliega el **menu interactivo de CRUD** donde el usuario puede ingresar datos en tiempo real.

---

## Penalizaciones Evitadas

| Penalizacion | Estado |
|---|---|
| Logica de negocio en `Main` | OK — `Program.cs` solo llama a `controller.Run()` |
| `Console` en el Model | OK — Ninguna referencia a `Console` en `Models/` |
| Logica en la View | OK — `GraphView` solo tiene metodos de impresion e input |
| Duplicados aceptados sin control | OK — `Graph.AddVertex` y `Graph.AddEdge` los rechazan |
| Repositorio privado o que no compila | OK — Asegurar repo publico y compilacion limpia |

---

## Checklist de Pruebas Minimas

- [x] Grafo dirigido con >= 12 vertices y >= 18 aristas
- [x] Impresion de lista de adyacencia
- [x] 3 BFS desde distintos nodos
- [x] 1 DFS completo con deteccion de ciclos
- [x] Consultas: influyentes, activos, sin seguidores
- [x] Alcanzabilidad (CanReach)
- [x] >= 3 operaciones CRUD funcionales (agregar, eliminar, actualizar) mediante menu interactivo

---

## Evidencia de Ejecucion

Al ejecutar `dotnet run` la salida comienza asi:

```
╔══════════════════════════════════════════════════════╗
║         CampusNet — Red Social Academica            ║
╚══════════════════════════════════════════════════════╝

  Registrando usuarios:
  [+] Usuario agregado:   [U01] Ana Garcia (Full Stack)
  [+] Usuario agregado:   [U02] Luis Martinez (Tech Leader)
  ...

  ╔══════════════════════════════════╗
  ║       CRUD INTERACTIVO           ║
  ╠══════════════════════════════════╣
  ║  1. Agregar usuario              ║
  ║  2. Eliminar usuario             ║
  ║  3. Actualizar usuario           ║
  ║  4. Agregar relacion             ║
  ║  5. Eliminar relacion            ║
  ║  6. Ver lista de adyacencia      ║
  ║  0. Salir del CRUD               ║
  ╚══════════════════════════════════╝
  Opcion: _
```
