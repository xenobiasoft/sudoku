namespace DepenMock.XUnit
{
	public abstract class BaseTestByType<TTestType> : BaseTest where TTestType : class
	{
		protected TTestType ResolveSut() => Container?.Resolve<TTestType>();
	}
}