using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace mGBAHttp.OpenApi
{
    public sealed class DocumentInfoTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Info.Title = "mGBA-http";
            document.Info.Description = "An HTTP interface for mGBA scripting.";

            var version = typeof(DocumentInfoTransformer).Assembly.GetName().Version;
            document.Info.Version = version is null ? "1.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";

            document.Info.Contact = new OpenApiContact
            {
                Name = "mGBA-http GitHub Repository",
                Url = new Uri("https://github.com/nikouu/mGBA-http/")
            };
            document.Info.License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://github.com/nikouu/mGBA-http/blob/main/LICENSE")
            };
            document.ExternalDocs = new OpenApiExternalDocs
            {
                Description = "mGBA scripting API docs",
                Url = new Uri("https://mgba.io/docs/scripting.html")
            };

            document.Servers?.Clear();

            var tagDescriptions = new Dictionary<string, string>
            {
                ["Core"] = "Endpoints for the mGBA Core scripting API.",
                ["CoreAdapter"] = "Endpoints for the mGBA CoreAdapter scripting API.",
                ["MemoryDomain"] = "Endpoints for the mGBA MemoryDomain scripting API.",
                ["Console"] = "Endpoints for the mGBA scripting console.",
                ["Button"] = "mGBA-http convenience layer using button names instead of key bitmasks.",
                ["Extension"] = "mGBA-http convenience layer for miscellaneous helpers."
            };

            if (document.Tags is not null)
            {
                foreach (var tag in document.Tags)
                {
                    if (tag.Name is not null && tagDescriptions.TryGetValue(tag.Name, out var description))
                    {
                        tag.Description = description;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
