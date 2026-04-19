using DepenMock.Moq;

namespace UnitTests;

public class MoqBaseTestByAbstraction<TTestType, TInterfaceType>() : BaseTestByAbstraction<TTestType, TInterfaceType>(new MoqMockFactory())
    where TTestType : class, TInterfaceType;