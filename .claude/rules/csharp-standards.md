# C# Coding Standards & Conventions

## Naming Conventions
- PascalCase for public members, classes, and methods
- camelCase for private fields and local variables
- UPPER_CASE for constants
- Prefix private fields with underscore: `_fieldName`

## File Organization
- One public class per file
- File name should match class name
- Group related classes in appropriate namespaces

## Code Style
- Use expression-bodied members when appropriate
- Prefer `var` for local variables when type is obvious
- Use `readonly` for immutable fields
- Use primary constructors when possible
- Always use curly braces for code blocks

## Dependency Injection
Register services via extension methods:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSudokuServices(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, CosmosDbGameRepository>();
        services.AddScoped<ICommandHandler<CreateGameCommand>, CreateGameCommandHandler>();
        services.AddScoped<IGameApplicationService, GameApplicationService>();
        return services;
    }
}
```

## Anti-Patterns to Avoid
- **Anemic Domain Models**: don't create entities that are just data containers
- **Tight Coupling**: avoid direct dependencies between layers
- **God Objects**: don't create classes with too many responsibilities
- **Primitive Obsession**: use value objects instead of primitives for domain concepts
- **Magic Numbers**: use constants or enums
- **Long Methods**: keep methods focused and under 20 lines when possible
- **Deep Nesting**: avoid deeply nested conditionals and loops
