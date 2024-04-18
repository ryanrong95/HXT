<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentEdit.aspx.cs" Inherits="WebApp.Crm.Documents.DocumentEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var data = eval('(<%=this.Model.Data%>)');

        //页面加载时
        $(function () {
            $("#tdfile").hide();

            //数据初始化
            if (data != null) {
                $("#tdfile").show();

                $("#Title").textbox("setValue", data.Title);
                $("#Summary").textbox("setValue", escape2Html(data.Summary));
                document.getElementById('fileName').innerHTML = "<a href='" + data.Url + "' target='_blank' style='color:Blue'>文件名: " + data.Name + "</a>";
            }
        });

        //保存
        function Save() {
            var files = document.getElementById("fileUpload").files;
            var id = getQueryString("ID");
            if (files.length == 0 && id == "") {
                $.messager.alert('提示', '请选择上传文件！');
                return false;
            }
            return Valid();
        }

        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin-top: 10px; width: 100%">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr>
                <td class="lbl">标题</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="Title" name="Title" data-options="required:true,validType:'length[1,50]'" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">描述</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="Summary" name="Summary"
                        data-options="multiline:true,required:true,validType:'length[1,250]',tipPosition:'bottom'" style="width: 98%; height: 80px" />
                </td>
            </tr>
            <tr>
                <td class="lbl">附件上传</td>
                <td colspan="3">
                    <asp:FileUpload ID="fileUpload" runat="server" />
                </td>
                <td class="lbl" id="tdfile">已上传文件</td>
                <td>
                    <div style="word-break:break-all;word-wrap:break-word;width:90%">
                        <label id="fileName"></label>
                    </div>
                </td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <asp:Button ID="btnSave" Text="保存" runat="server" OnClientClick="return Save();" OnClick="btnSave_Click" />
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()" />
        </div>
    </form>
</body>
</html>
