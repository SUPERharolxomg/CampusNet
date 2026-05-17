using CampusNet.Models;
using CampusNet.Views;

namespace CampusNet.Controllers
{
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

            var users = new[]
            {
                new Vertex("EST001", "Ana García",      "Full Stack"),
                new Vertex("EST002", "Luis Martínez",   "Tech Leader"),
                new Vertex("EST003", "Carlos Romero",   "Full Stack"),
                new Vertex("EST004", "María López",     "Full Stack"),
                new Vertex("EST005", "Pedro Sánchez",   "Tech Leader"),
                new Vertex("EST006", "Sofía Torres",    "Full Stack"),
                new Vertex("EST007", "Diego Ramírez",   "Full Stack"),
                new Vertex("EST008", "Valeria Núñez",   "Full Stack"),
                new Vertex("EST009", "Andrés Herrera",  "Tech Leader"),
                new Vertex("EST010", "Camila Ríos",     "Full Stack"),
                new Vertex("EST011", "Tomás Vargas",    "Full Stack"),
                new Vertex("EST012", "Elena Castro",    "Tech Leader"),
            };

            GraphView.PrintSectionLabel("Registrando usuarios:");
            foreach (var u in users)
                AddUser(u);

            GraphView.PrintSectionLabel("Registrando relaciones de seguimiento:");

            var edges = new[]
            {
                // Ciclo dirigido: EST001 → EST002 → EST003 → EST001
                ("EST001","EST002"), ("EST002","EST003"), ("EST003","EST001"),

                // EST002 grado salida >= 4
                ("EST002","EST004"), ("EST002","EST005"), ("EST002","EST006"),

                // EST005 grado salida >= 4
                ("EST005","EST001"), ("EST005","EST003"), ("EST005","EST007"), ("EST005","EST008"),

                // Relaciones variadas para cubrir >= 18 aristas
                ("EST001","EST004"), ("EST004","EST007"),
                ("EST006","EST009"), ("EST007","EST009"),
                ("EST008","EST010"), ("EST009","EST010"),
                ("EST010","EST003"), ("EST003","EST007"),

                // EST011 y EST012: sin seguidores (grado entrada 0), solo siguen
                ("EST011","EST002"), ("EST012","EST005"),
            };

            foreach (var (f, t) in edges)
                AddFollowByCodigo(f, t);
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

            string[] startCodigos = { "EST001", "EST005", "EST008" };
            foreach (var codigo in startCodigos)
            {
                var vertex = _graph.GetVertexByCodigo(codigo);
                if (vertex == null) continue;
                var order = _graph.BFS(vertex.Id);
                GraphView.PrintBFS(vertex.CodigoEstudiante, order, _graph.GetVertices());
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

            var noFollowers = _graph.GetVertices().Values
                .Where(v => _graph.InDegree(v.Id) == 0)
                .ToList();
            GraphView.PrintNoFollowers(noFollowers);

            int maxIn = _graph.GetVertices().Values.Max(v => _graph.InDegree(v.Id));
            var influential = _graph.GetVertices().Values
                .Select(v => (User: v, InDeg: _graph.InDegree(v.Id)))
                .Where(x => x.InDeg == maxIn)
                .OrderByDescending(x => x.InDeg)
                .ToList();
            GraphView.PrintInfluentialUsers(influential);

            int maxOut = _graph.GetVertices().Values.Max(v => _graph.OutDegree(v.Id));
            var active = _graph.GetVertices().Values
                .Select(v => (User: v, OutDeg: _graph.OutDegree(v.Id)))
                .Where(x => x.OutDeg == maxOut)
                .OrderByDescending(x => x.OutDeg)
                .ToList();
            GraphView.PrintActiveUsers(active);

            RunReachabilityCheck("EST001", "EST010");
            RunReachabilityCheck("EST011", "EST010");
            RunReachabilityCheck("EST012", "EST001");
        }

        private void RunReachabilityCheck(string fromCodigo, string toCodigo)
        {
            var fromV = _graph.GetVertexByCodigo(fromCodigo);
            var toV   = _graph.GetVertexByCodigo(toCodigo);
            if (fromV == null || toV == null) return;
            bool reach = _graph.CanReach(fromV.Id, toV.Id);
            GraphView.PrintReachability(fromCodigo, toCodigo, reach);
        }

        // ════════════════════════════════════════════════════════════════════
        //  CASO DE USO 6 — CRUD Interactivo
        // ════════════════════════════════════════════════════════════════════

        public void RunInteractiveMenu()
        {
            GraphView.PrintHeader("Operaciones CRUD — Interactivo");

            bool exit = false;
            while (!exit)
            {
                GraphView.PrintMainMenu();
                string option = Console.ReadLine()?.Trim() ?? "";

                switch (option)
                {
                    case "1": InteractiveAddUser();      break;
                    case "2": InteractiveRemoveUser();   break;
                    case "3": InteractiveUpdateUser();   break;
                    case "4": InteractiveAddFollow();    break;
                    case "5": InteractiveRemoveFollow(); break;
                    case "6": ShowAdjacencyList();       break;
                    case "0": exit = true;               break;
                    default:  GraphView.PrintInvalidOption(); break;
                }
            }
        }

        private void InteractiveAddUser()
        {
            GraphView.PrintSubHeader("Agregar usuario");
            string codigo = GraphView.ReadLine("Codigo de estudiante (ej. EST013)");
            string name   = GraphView.ReadLine("Nombre completo");
            string role   = GraphView.ReadLine("Rol (Tech Leader / Full Stack)");

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role))
            {
                GraphView.PrintFieldRequired();
                return;
            }

            AddUser(new Vertex(codigo.ToUpper(), name, role));
            ShowAdjacencyList();
        }

        private void InteractiveRemoveUser()
        {
            GraphView.PrintSubHeader("Eliminar usuario");
            string codigo = GraphView.ReadLine("Codigo de estudiante");

            if (string.IsNullOrWhiteSpace(codigo)) { GraphView.PrintFieldRequired(); return; }

            var vertex = _graph.GetVertexByCodigo(codigo.ToUpper());
            if (vertex == null) { GraphView.PrintVertexNotFound(codigo.ToUpper()); return; }

            RemoveUser(vertex.Id);
            ShowAdjacencyList();
        }

        private void InteractiveUpdateUser()
        {
            GraphView.PrintSubHeader("Actualizar usuario");
            string codigo = GraphView.ReadLine("Codigo de estudiante");

            if (string.IsNullOrWhiteSpace(codigo)) { GraphView.PrintFieldRequired(); return; }

            var vertex = _graph.GetVertexByCodigo(codigo.ToUpper());
            if (vertex == null) { GraphView.PrintVertexNotFound(codigo.ToUpper()); return; }

            string name = GraphView.ReadLine("Nuevo nombre (Enter para no cambiar)");
            string role = GraphView.ReadLine("Nuevo rol   (Enter para no cambiar)");

            UpdateUser(
                vertex.Id,
                string.IsNullOrWhiteSpace(name) ? null : name,
                string.IsNullOrWhiteSpace(role) ? null : role);
            ShowAdjacencyList();
        }

        private void InteractiveAddFollow()
        {
            GraphView.PrintSubHeader("Agregar relacion");
            string fromCodigo = GraphView.ReadLine("Codigo de estudiante origen");
            string toCodigo   = GraphView.ReadLine("Codigo de estudiante destino");

            if (string.IsNullOrWhiteSpace(fromCodigo) || string.IsNullOrWhiteSpace(toCodigo))
            {
                GraphView.PrintFieldRequired();
                return;
            }

            var fromV = _graph.GetVertexByCodigo(fromCodigo.ToUpper());
            var toV   = _graph.GetVertexByCodigo(toCodigo.ToUpper());

            if (fromV == null) { GraphView.PrintVertexNotFound(fromCodigo.ToUpper()); return; }
            if (toV   == null) { GraphView.PrintVertexNotFound(toCodigo.ToUpper());   return; }

            AddFollow(fromV.Id, toV.Id);
            ShowAdjacencyList();
        }

        private void InteractiveRemoveFollow()
        {
            GraphView.PrintSubHeader("Eliminar relacion");
            string fromCodigo = GraphView.ReadLine("Codigo de estudiante origen");
            string toCodigo   = GraphView.ReadLine("Codigo de estudiante destino");

            if (string.IsNullOrWhiteSpace(fromCodigo) || string.IsNullOrWhiteSpace(toCodigo))
            {
                GraphView.PrintFieldRequired();
                return;
            }

            var fromV = _graph.GetVertexByCodigo(fromCodigo.ToUpper());
            var toV   = _graph.GetVertexByCodigo(toCodigo.ToUpper());

            if (fromV == null) { GraphView.PrintVertexNotFound(fromCodigo.ToUpper()); return; }
            if (toV   == null) { GraphView.PrintVertexNotFound(toCodigo.ToUpper());   return; }

            RemoveFollow(fromV.Id, toV.Id);
            ShowAdjacencyList();
        }

        // ════════════════════════════════════════════════════════════════════
        //  MÉTODOS AUXILIARES DE CRUD (comunican Model ↔ View)
        // ════════════════════════════════════════════════════════════════════

        private void AddUser(Vertex v)
        {
            if (_graph.AddVertex(v))
                GraphView.PrintVertexAdded(v);
            else
                GraphView.PrintVertexDuplicate(v.CodigoEstudiante);
        }

        private void RemoveUser(string uuid)
        {
            if (_graph.RemoveVertex(uuid))
                GraphView.PrintVertexRemoved(uuid);
            else
                GraphView.PrintVertexNotFound(uuid);
        }

        private void UpdateUser(string uuid, string? newName, string? newRole)
        {
            if (_graph.UpdateVertex(uuid, newName, newRole))
                GraphView.PrintVertexUpdated(_graph.GetVertex(uuid)!);
            else
                GraphView.PrintVertexNotFound(uuid);
        }

        private void AddFollow(string fromUuid, string toUuid)
        {
            var fromV = _graph.GetVertex(fromUuid);
            var toV   = _graph.GetVertex(toUuid);
            string fromDisplay = fromV?.CodigoEstudiante ?? fromUuid;
            string toDisplay   = toV?.CodigoEstudiante   ?? toUuid;

            bool ok = _graph.AddEdge(fromUuid, toUuid);
            if (ok)
                GraphView.PrintEdgeAdded(fromDisplay, toDisplay);
            else if (fromV == null || toV == null)
                GraphView.PrintEdgeError(fromDisplay, toDisplay);
            else
                GraphView.PrintEdgeDuplicate(fromDisplay, toDisplay);
        }

        private void RemoveFollow(string fromUuid, string toUuid)
        {
            var fromV = _graph.GetVertex(fromUuid);
            var toV   = _graph.GetVertex(toUuid);
            string fromDisplay = fromV?.CodigoEstudiante ?? fromUuid;
            string toDisplay   = toV?.CodigoEstudiante   ?? toUuid;

            if (_graph.RemoveEdge(fromUuid, toUuid))
                GraphView.PrintEdgeRemoved(fromDisplay, toDisplay);
            else
                GraphView.PrintEdgeNotFound(fromDisplay, toDisplay);
        }

        // Resuelve CodigoEstudiante a UUID antes de agregar una arista
        private void AddFollowByCodigo(string fromCodigo, string toCodigo)
        {
            var fromV = _graph.GetVertexByCodigo(fromCodigo);
            var toV   = _graph.GetVertexByCodigo(toCodigo);
            if (fromV == null || toV == null)
            {
                GraphView.PrintEdgeError(fromCodigo, toCodigo);
                return;
            }
            AddFollow(fromV.Id, toV.Id);
        }

        // ════════════════════════════════════════════════════════════════════
        //  MÉTRICAS FINALES
        // ════════════════════════════════════════════════════════════════════

        public void ShowFinalMetrics()
        {
            var vertices = _graph.GetVertices();
            int V = vertices.Count;
            int E = _graph.GetEdges().Count;
            double density = V > 1 ? (double)E / (V * (V - 1)) : 0;
            double avgIn   = V > 0 ? vertices.Values.Average(v => _graph.InDegree(v.Id))  : 0;
            double avgOut  = V > 0 ? vertices.Values.Average(v => _graph.OutDegree(v.Id)) : 0;
            GraphView.PrintFinalMetrics(V, E, density, avgIn, avgOut);
        }

        // ════════════════════════════════════════════════════════════════════
        //  PUNTO DE ENTRADA
        // ════════════════════════════════════════════════════════════════════

        public void Run()
        {
            GraphView.PrintBanner();
            BuildInitialGraph();       // Caso de uso 1
            ShowAdjacencyList();       // Caso de uso 2
            RunBFSTraversals();        // Caso de uso 3
            RunFullDFS();              // Caso de uso 4
            RunSocialQueries();        // Caso de uso 5
            ShowFinalMetrics();        // Métricas y análisis final
            RunInteractiveMenu();      // Caso de uso 6 — CRUD interactivo
            GraphView.PrintHeader("Fin de la Simulación CampusNet");
            Console.WriteLine();
        }
    }
}
