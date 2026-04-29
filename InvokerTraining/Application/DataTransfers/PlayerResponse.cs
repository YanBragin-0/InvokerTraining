namespace InvokerTraining.Application.DataTransfers
{
    public record PlayerResponse(string PhoneOrEmail,int GameCount,TimeSpan? playerRecord);
}
