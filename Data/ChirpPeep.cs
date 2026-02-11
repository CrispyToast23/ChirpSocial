namespace ChirpSocial.Data
{
    public class ChirpPeep
    {
        public int ChirpId { get; set; }
        public Chirp Chirp { get; set; } = null!;
        public int PeepId { get; set; }
        public Peep Peep { get; set; } = null!;
    }
}
