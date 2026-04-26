namespace InvokerTraining.Application.Abstractions
{
    public interface IHasher
    {
        string Hash(string password);
        bool Verify(string hash,string password);
    }
}
