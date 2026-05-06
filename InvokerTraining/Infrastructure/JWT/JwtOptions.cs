namespace InvokerTraining.Infrastructure.JWT
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public int Expires {  get; set; }
        public int Refresh { get; set; }
    }
}
