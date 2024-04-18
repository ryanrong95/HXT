using Needs.Underly;

namespace Needs.Overall.Models
{
    public interface IExchangeRate
    {
        District District { get; }
        Currency From { get; }
        Currency To { get; }
        decimal Value { get; }
    }
}
