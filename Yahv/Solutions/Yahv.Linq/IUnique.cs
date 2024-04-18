namespace Yahv.Linq
{
    /// <summary>
    /// 唯一性接口
    /// </summary>
    public interface IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        string ID { get; }

    }


    /// <summary>
    /// 唯一性接口
    /// </summary>
    public interface IUnique<T>
    {
        /// <summary>
        /// ID
        /// </summary>
        T ID { get; }
    }



    /// <summary>
    /// 名册化接口
    /// </summary>
    public interface IRoll
    {

    }

    /// <summary>
    /// 名册化接口
    /// </summary>
    public interface IEntity
    {

    }

    /// <summary>
    /// 名册化接口
    /// </summary>
    public interface IDataEntity
    {

    }

}
