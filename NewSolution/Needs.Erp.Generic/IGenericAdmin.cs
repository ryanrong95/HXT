namespace Needs.Erp.Generic
{
    public partial interface IGenericAdmin
    {
        string ID { get; }
        string RealName { get; }
        string UserName { get; }
        bool IsSa { get; }
    }
}
