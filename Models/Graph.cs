namespace CampusNet.Models
{

    public class Graph
    {
        private readonly Dictionary<string, Vertex> _vertices;
        private readonly Dictionary<string, List<string>> _adjacency;
        private readonly List<Edge> _edges;

        public Graph()
        {
            _vertices  = new Dictionary<string, Vertex>();
            _adjacency = new Dictionary<string, List<string>>();
            _edges     = new List<Edge>();
        }

        // ───────────────────────────── VERTICES ──────────────────────────────

        /// <summary>Agrega un vértice si su UUID e CodigoEstudiante no existen ya.</summary>
        public bool AddVertex(Vertex v)
        {
            if (_vertices.ContainsKey(v.Id))
                return false;
            if (_vertices.Values.Any(u => u.CodigoEstudiante == v.CodigoEstudiante))
                return false;

            _vertices[v.Id]  = v;
            _adjacency[v.Id] = new List<string>();
            return true;
        }

        /// <summary>Elimina un vértice y todas sus aristas incidentes.</summary>
        public bool RemoveVertex(string id)
        {
            if (!_vertices.ContainsKey(id))
                return false;

            _adjacency.Remove(id);
            foreach (var list in _adjacency.Values)
                list.Remove(id);

            _edges.RemoveAll(e => e.FromId == id || e.ToId == id);
            _vertices.Remove(id);
            return true;
        }

        /// <summary>Actualiza nombre y/o rol de un vértice existente.</summary>
        public bool UpdateVertex(string id, string? newName, string? newRole)
        {
            if (!_vertices.TryGetValue(id, out var v))
                return false;

            if (newName != null) v.Name = newName;
            if (newRole != null) v.Role = newRole;
            return true;
        }

        // ────────────────────────────── EDGES ────────────────────────────────

        /// <summary>Agrega una arista dirigida fromId → toId si no existe ya.</summary>
        public bool AddEdge(string fromId, string toId)
        {
            if (!_vertices.ContainsKey(fromId) || !_vertices.ContainsKey(toId))
                return false;   // alguno de los extremos no existe

            if (_adjacency[fromId].Contains(toId))
                return false;   // arista duplicada rechazada

            _adjacency[fromId].Add(toId);
            _edges.Add(new Edge(fromId, toId));
            return true;
        }

        /// <summary>Elimina la arista dirigida fromId → toId.</summary>
        public bool RemoveEdge(string fromId, string toId)
        {
            if (!_adjacency.ContainsKey(fromId))
                return false;

            bool removed = _adjacency[fromId].Remove(toId);
            if (removed)
                _edges.RemoveAll(e => e.FromId == fromId && e.ToId == toId);
            return removed;
        }

        // ────────────────────────── CONSULTAS ────────────────────────────────

        public IReadOnlyDictionary<string, Vertex> GetVertices() => _vertices;

        public IReadOnlyDictionary<string, List<string>> GetAdjacency() => _adjacency;

        public IReadOnlyList<Edge> GetEdges() => _edges;

        /// <summary>Busca un vértice por su CodigoEstudiante (identificador legible).</summary>
        public Vertex? GetVertexByCodigo(string codigo) =>
            _vertices.Values.FirstOrDefault(v => v.CodigoEstudiante == codigo);

        public Vertex? GetVertex(string id) =>
            _vertices.TryGetValue(id, out var v) ? v : null;

        /// <summary>Grado de salida: cuántos usuarios sigue.</summary>
        public int OutDegree(string id) =>
            _adjacency.TryGetValue(id, out var list) ? list.Count : 0;

        /// <summary>Grado de entrada: cuántos usuarios lo siguen.</summary>
        public int InDegree(string id)
        {
            int count = 0;
            foreach (var list in _adjacency.Values)
                if (list.Contains(id)) count++;
            return count;
        }

        /// <summary>Devuelve los vecinos (lista de adyacencia) de un nodo.</summary>
        public List<string> Neighbors(string id) =>
            _adjacency.TryGetValue(id, out var list) ? list : new List<string>();

        // ─────────────────────────── RECORRIDOS ──────────────────────────────

        /// <summary>
        /// BFS desde un nodo origen.
        /// Devuelve el orden de visita de los Ids alcanzados.
        /// </summary>
        public List<string> BFS(string startId)
        {
            var visited = new HashSet<string>();
            var order   = new List<string>();
            var queue   = new Queue<string>();

            if (!_vertices.ContainsKey(startId))
                return order;

            queue.Enqueue(startId);
            visited.Add(startId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                order.Add(current);

                foreach (var neighbor in _adjacency[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return order;
        }

        /// <summary>
        /// DFS completo sobre todos los vértices (como si fuera un bosque).
        /// Devuelve el orden de descubrimiento y los IDs donde se detectó
        /// una arista de retroceso (back-edge → ciclo).
        /// </summary>
        public (List<string> Order, List<string> CycleNodes) DFSFull()
        {
            var visited    = new HashSet<string>();
            var inStack    = new HashSet<string>();
            var order      = new List<string>();
            var cycleNodes = new List<string>();

            foreach (var id in _vertices.Keys)
            {
                if (!visited.Contains(id))
                    DFSVisit(id, visited, inStack, order, cycleNodes);
            }

            return (order, cycleNodes);
        }

        private void DFSVisit(
            string id,
            HashSet<string> visited,
            HashSet<string> inStack,
            List<string>    order,
            List<string>    cycleNodes)
        {
            visited.Add(id);
            inStack.Add(id);
            order.Add(id);

            foreach (var neighbor in _adjacency[id])
            {
                if (!visited.Contains(neighbor))
                {
                    DFSVisit(neighbor, visited, inStack, order, cycleNodes);
                }
                else if (inStack.Contains(neighbor))
                {
                    // back-edge: hay un ciclo
                    if (!cycleNodes.Contains(neighbor))
                        cycleNodes.Add(neighbor);
                }
            }

            inStack.Remove(id);
        }

        /// <summary>
        /// Verifica si existe un camino dirigido de fromId a toId (BFS).
        /// </summary>
        public bool CanReach(string fromId, string toId)
        {
            if (!_vertices.ContainsKey(fromId) || !_vertices.ContainsKey(toId))
                return false;

            var visited = new HashSet<string>();
            var queue   = new Queue<string>();

            queue.Enqueue(fromId);
            visited.Add(fromId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == toId) return true;

                foreach (var neighbor in _adjacency[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return false;
        }
    }
}
