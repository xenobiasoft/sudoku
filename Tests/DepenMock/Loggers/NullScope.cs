namespace DepenMock.Loggers
{
	/// <summary>
	/// Provides a Nullable pattern for testing logging
	/// </summary>
	public class NullScope : IDisposable
	{
		private NullScope()
		{ }


		/// <summary>
		/// Singleton Instance property to get a reference to <see cref="NullScope"/>
		/// </summary>
		public static NullScope Instance { get; } = new();


		/// <inheritdoc />
		public void Dispose() { }
	}
}
