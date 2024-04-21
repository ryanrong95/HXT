using System.Collections.Generic;

namespace Yahv.Erm.Services.Models.Origins
{
    public class Business
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string LogoUrl { get; set; }
        public string FirstUrl { get; set; }

        //public FirstMenu[] Menu { get; set; }

        public List<FirstMenu> Menu { get; set; }
    }

    public class FirstMenu
    {
        public string text { get; set; }
        public string state { get; set; }
        //public ChildMenu[] children { get; set; }

        public List<ChildMenu> children { get; set; }
    }

    public class ChildMenu
    {
        public string text { get; set; }
        public string url { get; set; }
        //public string iconCls { get; set; }
    }
}