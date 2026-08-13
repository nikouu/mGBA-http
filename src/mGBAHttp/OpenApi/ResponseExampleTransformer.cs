using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;

namespace mGBAHttp.OpenApi
{
    /// <summary>Carries an endpoint's response example as metadata because every endpoint returns a bare <c>string</c>, which has no per-endpoint schema for an XML <c>&lt;example&gt;</c> to attach to.</summary>
    public sealed record ResponseExample(string Value);

    /// <summary>Writes each endpoint's <see cref="ResponseExample"/> into its 200 text/plain response.</summary>
    public sealed class ResponseExampleTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var example = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<ResponseExample>()
                .FirstOrDefault();

            if (example is not null
                && operation.Responses is not null
                && operation.Responses.TryGetValue("200", out var response)
                && response.Content is not null
                && response.Content.TryGetValue("text/plain", out var media))
            {
                media.Example = JsonValue.Create(example.Value);
            }

            return Task.CompletedTask;
        }
    }
}
