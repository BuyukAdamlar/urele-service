namespace urele.Service.Model
{
    public class GoShortLink
    {
        public long waitTime { get; set; }
        public string url { get; set; } //Uzun link
        public string title { get; set; } //Link başlığı
        public string description { get; set; } //Link mesajı
        public string creator { get; set; } //Oluşturan kişi
        public DateTime expiresOn { get; set; } //Son geçerlilik tarihi

    }
}
