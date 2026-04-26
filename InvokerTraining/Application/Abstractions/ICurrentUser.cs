namespace InvokerTraining.Application.Abstractions
{
    public interface ICurrentUser
    {
        Guid? CurrentUserID { get; }
    }
}
