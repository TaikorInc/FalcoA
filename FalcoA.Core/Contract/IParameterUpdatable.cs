
namespace FalcoA.Core
{
    public interface IParameterUpdatable<T>
    {
        void Update(Context context, T val);
    }
}
