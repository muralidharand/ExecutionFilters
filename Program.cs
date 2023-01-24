var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/{name?}", (string? name) => "Hello World!")
    .AddEndpointFilter<AfterEndpointExecution>(); 
app.Run();

/* Code for After Endpoint Execution Filter */
public class AfterEndpointExecution : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context); //first the actual code executed
        if (result is string temp &&
                context.HttpContext.GetEndpoint() is { } e)
        {
            return "Welcome" + result.ToString();
        }
        return result;
    }
}
/* Code for ShortCircuit Endpoint Execution Filter */
public class ShortCircuit : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        return new ValueTask<object?>(Results.Json(new { Name = "hello" }));
    }
}


/* Code of Before Endpoint Execution Filter */
public class BeforeEndpointExecution : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.GetRouteValue("name") is string name)
        {
            return Results.Ok($"Hi {name}, this is from the before endpoint execution filter!");
        }
        return await next(context);
    }
}
