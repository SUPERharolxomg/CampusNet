namespace CampusNet.Models
{
    public class Vertex
    {
        public string Id               { get; private set; }  // UUID interno — clave del grafo
        public string CodigoEstudiante { get; private set; }  // identificador legible por humanos
        public string Name             { get; set; }
        public string Role             { get; set; }          // "Tech Leader", "Full Stack"

        public Vertex(string codigoEstudiante, string name, string role)
        {
            Id               = Guid.NewGuid().ToString();
            CodigoEstudiante = codigoEstudiante;
            Name             = name;
            Role             = role;
        }

        public override string ToString() =>
            $"[{CodigoEstudiante}] {Name} ({Role})";
    }
}
