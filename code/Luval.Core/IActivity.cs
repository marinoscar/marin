using System.Threading.Tasks;

namespace Luval.Core
{
    public interface IActivity
    {
        IActivityName Name { get; }
        void AddInputArgument(string name, object value);
        Task<IActivityResult> Execute();
    }
}