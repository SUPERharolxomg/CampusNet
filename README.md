namespace CampusNet.Models
{

    public class Edge
    {
        public string FromId { get; private set; }
        public string ToId   { get; private set; }

        public Edge(string fromId, string toId)
        {
            FromId = fromId;
            ToId   = toId;
        }

        public override string ToString() =>
            $"{FromId} → {ToId}";
    }
}
