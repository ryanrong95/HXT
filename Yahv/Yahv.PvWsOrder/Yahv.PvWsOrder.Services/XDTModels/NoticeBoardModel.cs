using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    public class NoticeBoardModel : IUnique
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public DateTime CreateDate { get; set; }

        public string Content { get; set; }
    }
}
