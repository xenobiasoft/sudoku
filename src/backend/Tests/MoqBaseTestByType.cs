using DepenMock.Moq;

namespace UnitTests;

public class MoqBaseTestByType<TTestType>() : BaseTestByType<TTestType>(new MoqMockFactory()) where TTestType : class;