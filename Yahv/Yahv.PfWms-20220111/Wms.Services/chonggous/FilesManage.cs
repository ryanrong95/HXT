using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;

namespace Wms.Services.chonggous
{
    public class FilesManage
    {
        public void DeleteFile(string fileID)
        {
            string url = CenterFile.Web + "api/Upload/DeleteFile";
            Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { fileID = fileID });

            //var message = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { fileID = id });

            //var data = message.JsonTo<JMessage>();
            //return data;

        }

        //public void DeleteFiles(string[] fileIDs)
        //{
        //    string url = CenterFile.Web + "api/Upload/DeleteFiles";
        //    Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { fileIDs = fileIDs });

        //    //var message = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { fileID = id });

        //    //var data = message.JsonTo<JMessage>();
        //    //return data;

        //}

    }
}
