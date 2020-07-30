
using System.Threading.Tasks;

namespace ProjectPlanner.API.Services.EmailConfirmation
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
