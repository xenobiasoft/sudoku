using AutoFixture.AutoMoq;
using AutoFixture.Dsl;

namespace DepenMock
{
	/// <summary>
	/// Provides object creation functionality, as well as acting as a pseudo dependency injection container
	/// </summary>
	public class Container
	{
		private readonly IFixture _fixture;

		/// <summary>
		/// Initializes a new instance of <see cref="Container"/>
		/// </summary>
		public Container()
		{
			_fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

			_fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
				.ToList()
				.ForEach(x => _fixture.Behaviors.Remove(x));
		}

		/// <summary>
		/// Creates a new instance of the specified type. This acts as a data generator for
		/// primitive types and strings. For object types, it will generate a new instance
		/// of the class with all properties set with generated data.
		/// </summary>
		/// <typeparam name="TType">Any primitive type, string, or object</typeparam>
		/// <returns>A new instance of the requested type.</returns>
		public TType Create<TType>()
		{
			return _fixture.Create<TType>();
		}

		/// <summary>
		/// Creates a list of specified type. This acts as a data generator for
		/// primitive types and strings. For object types, it will generate a list of instances
		/// of the specified type with all properties set with generated data.
		/// </summary>
		/// <typeparam name="TType">Any primitive type, string, or object</typeparam>
		/// <param name="count">The number of instances to create</param>
		/// <returns>A list of instances of the requested type.</returns>
		public IEnumerable<TType> CreateMany<TType>(int? count = 3)
		{
			return _fixture.CreateMany<TType>(count.GetValueOrDefault());
		}

		/// <summary>
		/// Customizes the creation process for a single object.
		/// </summary>
		/// <typeparam name="TType">Any object type</typeparam>
		/// <returns>
		/// A <see cref="T:AutoFixture.Dsl.ICustomizationComposer`1" /> that can be used to customize the creation
		/// algorithm before creating the object.
		/// </returns>
		public ICustomizationComposer<TType> Build<TType>()
		{
			return _fixture.Build<TType>();
		}

		/// <summary>
		/// Creates a new instance of the requested type and registers it in the Fixture.
		/// The fixture always returns the same instance whenever the instance of the type is requested
		/// either directly, or indirectly as a nested value of other types.
		/// </summary>
		/// <typeparam name="TType">Any object type</typeparam>
		/// <returns>A new instance of the requested type.</returns>
		public TType Resolve<TType>() where TType : class
		{
			return _fixture.Freeze<TType>();
		}

		/// <summary>
		/// Creates a mock of the requested type and registers it in the Fixture. As a default, all properties and methods
		/// are automatically mocked, but can be overridden using a setup on the returned mocked object.
		/// The fixture always returns the same instance of the mock whenever the instance of the type is requested
		/// either directly, or indirectly as a nested value of other types.
		/// </summary>
		/// <typeparam name="TType">Any object type</typeparam>
		/// <returns>A mock instance of the requested type</returns>
		public Mock<TType> ResolveMock<TType>() where TType : class
		{
			return _fixture.Freeze<Mock<TType>>();
		}

		/// <summary>
		/// Registers an instance of TInstanceType in the Fixture. The fixture always returns this same instance when the instance
		/// of the type is requested either directly, or indirectly as a nested value of other types.
		/// </summary>
		/// <typeparam name="TInstanceType">The interface type</typeparam>
		/// <param name="instance">The instance of the type that will be registered with the Fixture</param>
		public void Register<TInstanceType>(TInstanceType instance) where TInstanceType : class
		{
			_fixture.Register(() => instance);
		}
	}
}
