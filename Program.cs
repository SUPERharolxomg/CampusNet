namespace CampusNet.Models
{
    /// <summary>
    /// Representa un usuario académico dentro de la red CampusNet.
    /// </summary>
    public class Vertex
    {
        public string Id   { get; private set; }
        public string Name { get; set; }
        public string Role { get; set; }   // "Estudiante", "Profesor", "Egresado"

        public Vertex(string id, string name, string role)
        {
            Id   = id;
            Name = name;
            Role = role;
        }

        public override string ToString() =>
            $"[{Id}] {Name} ({Role})";
    }
}
