namespace ETR.Middleware {
	public class ExceptionHandlerMiddleware {
		private readonly RequestDelegate _next;

		public ExceptionHandlerMiddleware(RequestDelegate next) {
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context) {
			try {
				await _next(context);
			}
			catch (Exception ex) {
				throw;
			}
		}
	}
}
