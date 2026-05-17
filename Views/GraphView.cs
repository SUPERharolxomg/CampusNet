using CampusNet.Models;

namespace CampusNet.Views
{
    /// <summary>
    /// Capa de presentación: únicamente imprime en consola.
    /// No contiene lógica de negocio.
    /// </summary>
    public static class GraphView
    {
        // ─────────────────────── ENCABEZADOS ────────────────────────────────

        public static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.WriteLine(new string('═', 60));
            Console.WriteLine($"  {title.ToUpper()}");
            Console.WriteLine(new string('═', 60));
        }

        public static void PrintSubHeader(string subtitle)
        {
            Console.WriteLine();
            Console.WriteLine($"  ── {subtitle} ──");
        }

        public static void PrintSeparator() =>
            Console.WriteLine(new string('─', 60));

        // ─────────────────────── GRAFO ──────────────────────────────────────

        /// <summary>Muestra la lista de adyacencia completa del grafo.</summary>
        public static void PrintAdjacencyList(
            IReadOnlyDictionary<string, Vertex> vertices,
            IReadOnlyDictionary<string, List<string>> adjacency)
        {
            PrintHeader("Lista de Adyacencia — CampusNet");

            foreach (var kv in adjacency)
            {
                var user = vertices[kv.Key];
                string neighbors = kv.Value.Count > 0
                    ? string.Join(", ", kv.Value.Select(id =>
                        vertices.TryGetValue(id, out var v) ? v.CodigoEstudiante : id))
                    : "(ninguno)";

                Console.WriteLine($"  {user} → [ {neighbors} ]");
            }
        }

        // ─────────────────────── VÉRTICES ───────────────────────────────────

        public static void PrintVertexAdded(Vertex v) =>
            Console.WriteLine($"  [+] Usuario agregado:   {v}");

        public static void PrintVertexRemoved(string id) =>
            Console.WriteLine($"  [-] Usuario eliminado:  Id={id}");

        public static void PrintVertexUpdated(Vertex v) =>
            Console.WriteLine($"  [~] Usuario actualizado: {v}");

        public static void PrintVertexNotFound(string id) =>
            Console.WriteLine($"  [!] Usuario no encontrado: Id={id}");

        public static void PrintVertexDuplicate(string codigo) =>
            Console.WriteLine($"  [!] Duplicado rechazado: ya existe CodigoEstudiante={codigo}");

        // ─────────────────────── ARISTAS ────────────────────────────────────

        public static void PrintEdgeAdded(string from, string to) =>
            Console.WriteLine($"  [+] Relación agregada:   {from} → {to}");

        public static void PrintEdgeRemoved(string from, string to) =>
            Console.WriteLine($"  [-] Relación eliminada:  {from} → {to}");

        public static void PrintEdgeDuplicate(string from, string to) =>
            Console.WriteLine($"  [!] Arista duplicada rechazada: {from} → {to}");

        public static void PrintEdgeError(string from, string to) =>
            Console.WriteLine($"  [!] No se pudo agregar arista {from} → {to} (vértice inexistente)");

        // ─────────────────────── RECORRIDOS ─────────────────────────────────

        public static void PrintBFS(string startCodigo, List<string> order,
            IReadOnlyDictionary<string, Vertex> vertices)
        {
            PrintSubHeader($"BFS desde [{startCodigo}]");
            Console.Write("  Orden de visita: ");
            Console.WriteLine(string.Join(" → ", order.Select(id =>
                vertices.TryGetValue(id, out var v) ? $"[{v.CodigoEstudiante}]{v.Name}" : id)));
            Console.WriteLine($"  Vértices alcanzados: {order.Count}");
        }

        public static void PrintDFS(List<string> order, List<string> cycleNodes,
            IReadOnlyDictionary<string, Vertex> vertices)
        {
            PrintSubHeader("DFS Completo");
            Console.Write("  Orden de descubrimiento: ");
            Console.WriteLine(string.Join(" → ", order.Select(id =>
                vertices.TryGetValue(id, out var v) ? $"[{v.CodigoEstudiante}]{v.Name}" : id)));

            if (cycleNodes.Count > 0)
            {
                var codigos = cycleNodes.Select(id =>
                    vertices.TryGetValue(id, out var v) ? v.CodigoEstudiante : id);
                Console.WriteLine($"  Ciclos detectados (back-edges hacia): " +
                    string.Join(", ", codigos));
            }
            else
            {
                Console.WriteLine("  Sin ciclos detectados.");
            }
        }

        // ─────────────────────── CONSULTAS SOCIALES ─────────────────────────

        public static void PrintNoFollowers(List<Vertex> users)
        {
            PrintSubHeader("Usuarios sin seguidores (grado entrada = 0)");
            if (users.Count == 0)
                Console.WriteLine("  (ninguno)");
            else
                users.ForEach(u => Console.WriteLine($"  • {u}"));
        }

        public static void PrintInfluentialUsers(List<(Vertex User, int InDeg)> list)
        {
            PrintSubHeader("Usuarios más influyentes (mayor grado de entrada)");
            list.ForEach(x => Console.WriteLine($"  • {x.User}  ← seguidores: {x.InDeg}"));
        }

        public static void PrintActiveUsers(List<(Vertex User, int OutDeg)> list)
        {
            PrintSubHeader("Usuarios más activos (mayor grado de salida)");
            list.ForEach(x => Console.WriteLine($"  • {x.User}  → siguiendo: {x.OutDeg}"));
        }

        public static void PrintReachability(string fromId, string toId, bool canReach)
        {
            PrintSubHeader($"Alcanzabilidad: [{fromId}] → [{toId}]");
            Console.WriteLine(canReach
                ? $"  ✓ [{fromId}] SÍ puede llegar a [{toId}]"
                : $"  ✗ [{fromId}] NO puede llegar a [{toId}]");
        }

        // ─────────────────────── INPUT DE USUARIO ───────────────────────────

        public static string ReadLine(string prompt)
        {
            Console.Write($"  {prompt}: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        public static void PrintMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════╗");
            Console.WriteLine("  ║       CRUD INTERACTIVO           ║");
            Console.WriteLine("  ╠══════════════════════════════════╣");
            Console.WriteLine("  ║  1. Agregar usuario              ║");
            Console.WriteLine("  ║  2. Eliminar usuario             ║");
            Console.WriteLine("  ║  3. Actualizar usuario           ║");
            Console.WriteLine("  ║  4. Agregar relacion             ║");
            Console.WriteLine("  ║  5. Eliminar relacion            ║");
            Console.WriteLine("  ║  6. Ver lista de adyacencia      ║");
            Console.WriteLine("  ║  0. Salir del CRUD               ║");
            Console.WriteLine("  ╚══════════════════════════════════╝");
            Console.Write("  Opcion: ");
        }

        public static void PrintInvalidOption() =>
            Console.WriteLine("  [!] Opcion no valida. Intente de nuevo.");

        public static void PrintFieldRequired() =>
            Console.WriteLine("  [!] Todos los campos son obligatorios.");

        // ─────────────────────── BANNER Y ETIQUETAS ─────────────────────────

        public static void PrintBanner()
        {
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║         CampusNet — Red Social Académica            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        }

        public static void PrintSectionLabel(string label) =>
            Console.WriteLine($"\n  {label}");

        public static void PrintEdgeNotFound(string from, string to) =>
            Console.WriteLine($"  [!] No existe la relación {from} → {to}");

        // ─────────────────────── MÉTRICAS FINALES ───────────────────────────

        public static void PrintFinalMetrics(int V, int E, double density, double avgIn, double avgOut)
        {
            PrintHeader("Métricas y Análisis Final");
            Console.WriteLine($"  Total de vértices         : {V}");
            Console.WriteLine($"  Total de aristas          : {E}");
            Console.WriteLine($"  Densidad del grafo        : {density:F4}  (E / V×(V-1))");
            Console.WriteLine($"  Grado de entrada promedio : {avgIn:F2}");
            Console.WriteLine($"  Grado de salida promedio  : {avgOut:F2}");
            Console.WriteLine();
            Console.WriteLine("  Análisis:");
            Console.WriteLine("  La red CampusNet presenta una densidad baja, típica de redes sociales");
            Console.WriteLine("  reales donde cada usuario sigue solo a un subconjunto del total.");
            Console.WriteLine("  La existencia de un ciclo dirigido (U01→U02→U03→U01) indica");
            Console.WriteLine("  colaboración mutua entre esos miembros. Los nodos con mayor grado");
            Console.WriteLine("  de entrada representan referentes con mayor visibilidad en la red.");
        }
    }
}
