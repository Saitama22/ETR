namespace ETR.Helpers {
	public interface ISendMail {
		public Task Send(string subject, string body);
	}
}
