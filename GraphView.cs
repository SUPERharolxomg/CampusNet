using CampusNet.Models;
using CampusNet.Views;

namespace CampusNet.Controllers
{
    /// <summary>
    /// Controlador principal de CampusNet.
    /// Orquesta todos los casos de uso y comunica Model ↔ View.
    /// No contiene impresión directa a consola (delega a GraphView).
    /// </summary>
    public class GraphController
    {
        private readonly Graph _graph;

        public GraphController()
        {
            _graph = new Graph();
        }

        // ════════════════════════════════════════════════════════════════════
        //  CASO DE USO 1 — Construcción del grafo
        // ════════════════════════════════════════════════════════════════════

        public void BuildInitialGraph()
        {
            GraphView.PrintHeader("Construcción del Grafo Inicial — CampusNet");

            // ── 12 usuarios con roles variados ──────────────────────────────
            var users = new[]
            {
                new Vertex("U01", "Ana García",      "Estudiante"),
                new Vertex("U02", "Luis Martínez",   "Profesor"),
                new Vertex("U03", "Carlos Romero",   "Egresado"),
                new Vertex("U04", "María López",     "Estudiante"),
                new Vertex("U05", "Pedro Sánchez",   "Profesor"),
                new Vertex("U06", "Sofía Torres",    "Estudiante"),
                new Vertex("U07", "Diego Ramírez",   "Egresado"),
                new Vertex("U08", "Valeria Núñez",   "Estudiante"),
                new Vertex("U09", "Andrés Herrera",  "Profesor"),
                new Vertex("U10", "Camila Ríos",     "Egresado"),
                new Vertex("U11", "Tomás Vargas",    "Estudiante"),
                new Vertex("U12", "Elena Castro",    "Profesora"),
            };

            Console.WriteLine("\n  Registrando usuarios:");
            foreach (var u in users)
                AddUser(u);

            // ── 18 relaciones dirigidas ──────────────────────────────────────
            // Ciclo A→B→C→A: U01 → U02 → U03 → U01
            // U02 grado salida ≥ 4
            // U05 grado salida ≥ 4
            // U11 y U12 sin seguidores (grado entrada = 0)

            Console.WriteLine("\n  Registrando relaciones de seguimiento:");

            var edges = new[]
            {
                // Ciclo dirigido: U01 → U02 → U03 → U01
                ("U01","U02"), ("U02","U03"), ("U03","U01"),

                // U02 sigue a 4+ usuarios (grado salida ≥ 4)
                ("U02","U04"), ("U02","U05"), ("U02","U06"),

                // U05 sigue a 4+ usuarios (grado salida ≥ 4)
                ("U05","U01"), ("U05","U03"), ("U05","U07"), ("U05","U08"),

                // Relaciones variadas para cubrir ≥ 18 aristas
                ("U01","U04"), ("U04","U07"),
                ("U06","U09"), ("U07","U09"),
                ("U08","U10"), ("U09","U10"),
                ("U10","U03"), ("U03","U07"),
                // U11 y U12: sin seguidores (grado entrada 0), solo siguen
                ("U11","U02"), ("U12","U05"),
            };

            foreach (var (f, t) in edges)
                AddFollow(f, t);
        }

        // ════════════════════════════════════════════════════════════════════
        //  CASO DE USO 2 — Mostrar lista de adyacencia
        // ════════════════════════════════════════════════════════════════════

        public void ShowAdjacencyList()
        {
            GraphView.PrintAdjacencyList(
                _graph.GetVertices(),
                _graph.GetAdjacency());
        }

        // ════════════════════════════════════════════════════════════════════
        //  CASO DE USO 3 — Recorridos BFS desde 3 usuarios distintos
        // ════════════════════════════════════════════════════════════════════

        public void RunBFSTraversals()
        {
            GraphView.PrintHeader("Recorridos BFS");

            string[] starts = { "U01", "U05", "U08" };
            foreach (var startId in starts)
            {
                var order = _graph.BFS(startId);
                GraphView.PrintBFS(startId, order, _graph.GetVertices());
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  CASO DE USO 4 — DFS completo
        // ════════════════════════════════════════════════════════════════════

        public void RunFullDFS()
        {
            GraphView.PrintHeader("Recorrido DFS Completo");

            var (order, cycleNodes) = _graph.DFSFull();
            GraphView.PrintDFS(order, cycleNodes, _graph.GetVertices());
        }

        // ════════════════════════════════════════════════════════════════════
        //  CASO DE USO 5 — Consultas sociales
        // ════════════════════════════════════════════════════════════════════

        public void RunSocialQueries()
        {
            GraphView.PrintHeader("Consultas Sociales");

            // 5a) Usuarios sin seguidores (grado entrada = 0)
            var noFollowers = _graph.GetVertices().Values
                .Where(v => _graph.InDegree(v.Id) == 0)
                .ToList();
            GraphView.PrintNoFollowers(noFollowers);

            // 5b) Usuarios influyentes (mayor grado de entrada)
            int maxIn = _graph.GetVertices().Values
                .Max(v => _graph.InDegree(v.Id));
            var influential = _graph.GetVertices().Values
                .Select(v => (User: v, InDeg: _graph.InDegree(v.Id)))
                .Where(x => x.InDeg == maxIn)
                .OrderByDescending(x => x.InDeg)
                .ToList();
            GraphView.PrintInfluentialUsers(influential);

            // 5c) Usuarios más activos (mayor grado de salida)
            int maxOut = _graph.GetVertices().Values
                .Max(v => _graph.OutDegree(v.Id));
            var active = _graph.GetVertices().Values
                .Select(v => (User: v, OutDeg: _graph.OutDegree(v.Id)))
                .Where(x => x.OutDeg == maxOut)
                .OrderByDescending(x => x.OutDeg)
                .ToList();
            GraphView.PrintActiveUsers(active);

            // 5d) Alcanzabilidad
            RunReachabilityCheck("U01", "U10");
            RunReachabilityCheck("U11", "U10");
            RunReachabilityCheck("U12", "U01");
        }

        private void RunReachabilityCheck(string from, string to)
        {
            bool reach = _graph.CanReach(from, to);
            GraphView.PrintReachability(from, to, reach);
        }

        // ════════════════════════════════════════════════════════════════════
        //  CASO DE USO 6 — Operaciones CRUD
        // ════════════════════════════════════════════════════════════════════

        public void RunCRUDOperations()
        {
            GraphView.PrintHeader("Operaciones CRUD");

            // ── 6a) Agregar nuevo usuario ────────────────────────────────────
            GraphView.PrintSubHeader("Agregar usuario U13");
            AddUser(new Vertex("U13", "Roberto Díaz", "Estudiante"));
            AddFollow("U13", "U02");
            AddFollow("U13", "U05");
            ShowAdjacencyList();

            // ── 6b) Intentar agregar duplicado ───────────────────────────────
            GraphView.PrintSubHeader("Intentar agregar usuario duplicado U01");
            AddUser(new Vertex("U01", "Duplicado", "Estudiante"));

            // ── 6c) Agregar relación y luego eliminarla ──────────────────────
            GraphView.PrintSubHeader("Agregar y eliminar relación U08 → U02");
            AddFollow("U08", "U02");
            RemoveFollow("U08", "U02");
            ShowAdjacencyList();

            // ── 6d) Actualizar usuario ───────────────────────────────────────
            GraphView.PrintSubHeader("Actualizar U04: nuevo nombre y rol");
            UpdateUser("U04", "María López Vega", "Egresada");
            ShowAdjacencyList();

            // ── 6e) Eliminar usuario ─────────────────────────────────────────
            GraphView.PrintSubHeader("Eliminar usuario U13");
            RemoveUser("U13");
            ShowAdjacencyList();

            // ── 6f) Intentar agregar arista duplicada ────────────────────────
            GraphView.PrintSubHeader("Intentar agregar arista duplicada U01 → U02");
            AddFollow("U01", "U02");
        }

        // ════════════════════════════════════════════════════════════════════
        //  MÉTODOS AUXILIARES DE CRUD (comunican Model ↔ View)
        // ════════════════════════════════════════════════════════════════════

        private void AddUser(Vertex v)
        {
            if (_graph.AddVertex(v))
                GraphView.PrintVertexAdded(v);
            else
                GraphView.PrintVertexDuplicate(v.Id);
        }

        private void RemoveUser(string id)
        {
            if (_graph.RemoveVertex(id))
                GraphView.PrintVertexRemoved(id);
            else
                GraphView.PrintVertexNotFound(id);
        }

        private void UpdateUser(string id, string? newName, string? newRole)
        {
            if (_graph.UpdateVertex(id, newName, newRole))
                GraphView.PrintVertexUpdated(_graph.GetVertex(id)!);
            else
                GraphView.PrintVertexNotFound(id);
        }

        private void AddFollow(string from, string to)
        {
            bool ok = _graph.AddEdge(from, to);
            if (ok)
                GraphView.PrintEdgeAdded(from, to);
            else if (_graph.GetVertex(from) == null || _graph.GetVertex(to) == null)
                GraphView.PrintEdgeError(from, to);
            else
                GraphView.PrintEdgeDuplicate(from, to);
        }

        private void RemoveFollow(string from, string to)
        {
            if (_graph.RemoveEdge(from, to))
                GraphView.PrintEdgeRemoved(from, to);
            else
                Console.WriteLine($"  [!] No existe la relación {from} → {to}");
        }

        // ════════════════════════════════════════════════════════════════════
        //  PUNTO DE ENTRADA — el Controller orquesta todo
        // ════════════════════════════════════════════════════════════════════

        public void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║         CampusNet — Red Social Académica            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");

            BuildInitialGraph();       // Caso de uso 1
            ShowAdjacencyList();       // Caso de uso 2
            RunBFSTraversals();        // Caso de uso 3
            RunFullDFS();              // Caso de uso 4
            RunSocialQueries();        // Caso de uso 5
            RunCRUDOperations();       // Caso de uso 6

            GraphView.PrintHeader("Fin de la Simulación CampusNet");
            Console.WriteLine();
        }
    }
}
