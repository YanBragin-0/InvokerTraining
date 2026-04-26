using System.ComponentModel.DataAnnotations;

namespace InvokerTraining.Application.DataTransfers
{
    public record RegisterationRequest(
        [Required] string EmailOrPhoneNumber,
        [Required] string password,
        [Required] string confirmPassword);
    public record LoginRequest(
        [Required] string EmailOrPhoneNumber,
        [Required] string password);
}
