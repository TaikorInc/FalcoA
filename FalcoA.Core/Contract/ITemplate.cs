using System.Threading.Tasks;

namespace FalcoA.Core
{
    public interface ITemplate
    {
        TemplateCrawlResult Run(IDataProvider provider);

        Task<TemplateCrawlResult> RunAsync(IDataProvider provider);
    }
}
