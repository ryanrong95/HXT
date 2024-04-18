using Needs.Linq;

namespace Needs.Interpreter.Models
{
    /// <summary>
    /// 语言包
    /// </summary>
    public interface ITopObject : IUnique, IPersist, IPersistence, IEnterSuccess, IFulSuccess
    {
        string Name { get; }
        string Language { get; }
        string Value { set; get; }
        string Project { get; }
        string[] Path {  get; }
    }
}
