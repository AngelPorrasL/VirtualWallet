namespace VirtualWallet.Models
{
    public class Card
    {
        public string? Id { get; set; }
        public string? Bank { get; set; }
        public string? Issuer { get; set; }
        public string? Owner { get; set; }
        public string? CardNumber { get; set; }
        public string? CVV { get; set; }
        public string? ExpirationDate { get; set; }
    }
}
