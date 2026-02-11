namespace ChirpSocial.Data
{
    public class Peep
    {
        public int Id { get; set; }
        public string Tag { get; set; } = string.Empty;
        public ICollection<ChirpPeep> ChirpPeeps { get; set; } = new List<ChirpPeep>();
    }
}
