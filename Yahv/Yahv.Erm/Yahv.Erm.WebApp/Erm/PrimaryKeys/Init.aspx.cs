using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Yahv.Erm.WebApp.Erm.PrimaryKeys
{
    public partial class Init : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //更新PvCenter值
            Yahv.Services.CheckPrimaryKey.CheckPkey<Yahv.Underly.PKeyType>();       //根据枚举更新pvCenter的PK
        }
    }
}