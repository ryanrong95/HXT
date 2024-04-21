

namespace Yahv.Erm.Fingerprints.Services
{
    /// <summary>
    /// 获取服务器的面单接结果
    /// </summary>
    public class FaceSheetResult
    {
        public string ID { get; set; }
        public string LogisticCode { get; set; }
        public string ShipperCode { get; set; }

        public string Html { get; set; }
    }
}
