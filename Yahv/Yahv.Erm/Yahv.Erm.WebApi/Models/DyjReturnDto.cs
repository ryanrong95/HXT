namespace Yahv.Erm.WebApi.Models
{
    /// <summary>
    /// 大赢家返回结果
    /// </summary>
    public class DyjResultModel<T>
    {
        /// <summary>
        /// 编码
        /// </summary>
        public int Code { get; set; }

        public bool Success { get; set; }

        public string Msg { get; set; }

        public string User_Host_Address { get; set; }

        public int Page_Count { get; set; }

        public int Count { get; set; }

        public string Response_Time { get; set; }

        public T[] Data { get; set; }
    }
}