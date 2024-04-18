using Layers.Data;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Extends
{
    public static class CenterFileExtend
    {
        public static void Update(this CenterFileDescription[] files,LinqReponsitory rep)
        {
            foreach (var file in files)
            {

                rep.Command($"update FilesDescriptionTopView set WsOrderID='{file.WsOrderID ?? ""}',[Type]={file.Type},WaybillID='{file.WaybillID??""}',NoticeID='{file.NoticeID??""}',StorageID='{file.StorageID??""}',InputID='{file.InputID??""}',AdminID='{file.AdminID??""}',ClientID='{file.ClientID??""}',CustomName='{file.CustomName??""}',PayID='{file.PayID??""}' where ID='{file.ID}'");
                rep.Submit();
            }
        }
    }
}
