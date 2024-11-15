
namespace ETR.Handlers.Interface
{
	public interface IMailHandler {
		Task SendMessageEmailConfirm(string email, string? confirmUrl);
	}
}
