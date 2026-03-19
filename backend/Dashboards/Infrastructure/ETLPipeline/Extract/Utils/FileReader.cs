using System.Reflection;

namespace Infrastructure.ETLPipeline.Extract.Utils
{
    public class FileReader
    {
        public async Task<string> ReadRequestBodyFromFileAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Infrastructure.ETLPipeline.Extract.Student.Resources.ContingentBody.json";

            using var stream = assembly.GetManifestResourceStream( resourceName );
            if ( stream == null )
                throw new FileNotFoundException( $"Resource {resourceName} not found" );

            using var reader = new StreamReader( stream );
            var jsonBody = await reader.ReadToEndAsync();

            return jsonBody;
        }
    }
}
