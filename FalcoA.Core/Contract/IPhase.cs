
namespace FalcoA.Core
{
    public interface IPhase
    {
        /// <summary>
        /// 运行流程，所有的参数Resolve都在运行的时候才解析
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        PhaseResult Run(Context context);
    }
}
