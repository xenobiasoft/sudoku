using DepenMock.Loggers;
using Microsoft.Extensions.Logging;

namespace DepenMock.XUnit
{
	public abstract class BaseTest
	{
		protected BaseTest()
		{
			Container = new Container();
			Logger = new ListLogger();
			Container
				.ResolveMock<ILoggerFactory>()
				.Setup(x => x.CreateLogger(It.IsAny<string>()))
				.Returns(Logger);
		}

		public Container Container { get; }

		protected virtual ListLogger Logger { get; }
	}
}
