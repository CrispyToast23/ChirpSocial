namespace ChirpSocial.Data
{
    public class ChirpImage
    {
        public int Id { get; set; }
        public int ChirpId { get; set; }
        public Chirp Chirp { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
