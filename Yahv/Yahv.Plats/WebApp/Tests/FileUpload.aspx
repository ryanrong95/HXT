<%@ Page Language="C#" %>

<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Yahv.Utils.Serializers" %>
<%@ Import Namespace="Yahv" %>

<%--线上上传的另外提供，不要混在一起开发--%>

<script language="C#" runat="server">
    void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json";

        //验证
        string token = Request.Form["token"];
        if (string.IsNullOrWhiteSpace(token) || Erp.Current == null)
        {
            //还应验证token是否一致，这里暂时省略
            //Response.Write(new
            //{
            //    Status = 500,
            //    Message = "validation failed!",
            //}.Json());
            //Response.End();
        }

        //后台开发暂时不限制长度
        string type = Request.Form["type"] ?? "defaults";
        string mainID = Request.Form["mainID"] ?? "unknown";


        var vpath = $"/{string.Join("/", "fh", Erp.Current.ID, type, mainID)}/";
        var path = Server.MapPath(vpath);

        //检查文件是否存在，不存在则创建
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        HttpFileCollection hfCollection = Request.Files;
        List<string> files = new List<string>(hfCollection.Count);

        for (int i = 0; i <= hfCollection.Count - 1; i++)
        {
            HttpPostedFile hPostedFile = hfCollection[i];
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(Path.GetFileName(hPostedFile.FileName));
            if (hPostedFile.ContentLength > 0)
            {
                hPostedFile.SaveAs(path + fileName);
                Uri uri;
                if (Uri.TryCreate(Request.Url, vpath + fileName, out uri))
                {
                    files.Add(uri.OriginalString);
                }
            }
        }

        Response.Write(files.Json());
        Response.End();
    }
</script>
