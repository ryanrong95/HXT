<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowList.aspx.cs" Inherits="WebApp.Crm.Trace.ShowList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="http://fixed2.b1b.com/Scripts/umeditor/themes/default/css/umeditor.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="http://fixed2.b1b.com/Scripts/umeditor/third-party/jquery.min.js"></script>
    <script src="http://fixed2.b1b.com/Scripts/umeditor/umeditor.config.js"></script>
    <script src="http://fixed2.b1b.com/Scripts/umeditor/umeditor.min.js"></script>
    <script type="text/javascript" src="http://fixed2.b1b.com/Scripts/umeditor/lang/zh-cn/zh-cn.js"></script>
    <uc:EasyUI runat="server" />
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto; width: 100%; height: 100%">
            <tr>
                <td class="lbl" style="width: 80px">跟进日期</td>
                <td>
                    <input class="easyui-datebox" id="Date" name="Date" style="width: 180px" data-options="readonly:true" />
                </td>
                <td class="lbl" style="width: 80px">跟进方式</td>
                <td>
                    <input class="easyui-textbox" id="TypeName" name="TypeName" style="width: 180px" data-options="readonly:true" />
                </td>
                <td class="lbl" style="width: 80px">客户</td>
                <td>
                    <input class="easyui-textbox" id="ClientName" name="ClientName" style="width: 180px" data-options="readonly:true" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">原厂陪同人员</td>
                <td>
                    <input class="easyui-textbox" id="OriginalStaffs" name="OriginalStaffs" data-options="readonly:true" style="width: 180px" />
                </td>
                <td class="lbl" style="width: 80px">下次跟进日期</td>
                <td>
                    <input class="easyui-datebox" id="NextDate" name="NextDate" style="width: 180px" data-options="readonly:true" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">跟进内容</td>
                <td colspan="7">
                    <input id="editorValue" type="hidden" />
                    <script id="editor" type="text/plain" style="width: 96%; height: 300px;"></script>
                </td>
            </tr>
        </table>
    </form>
    <div id="DivComment" style="width: 95%">
        <div class="easyui-panel" title="主管点评" style="width: 100%;" data-options="border:false"></div>
        <table id="myComment" style="margin: 0 auto; width: 95%; height: 95%">
        </table>
    </div>
    <script type="text/javascript">
        //编辑器
        var editor = UM.getEditor("editor");
        var editorComment = UM.getEditor("editorComment");
        var AllData = eval('(<%=this.Model.AllData%>)');

        //页面加载时
        $(function () {

            //初始化赋值
            if (AllData != null) {
                if (AllData["Content"] != null) {
                    UM.getEditor('editor').setContent(escape2Html(AllData["Content"]));
                }
                $("#ClientName").textbox("setValue", AllData["ClientName"]);
                $("#TypeName").textbox("setValue", AllData["TypeName"]);
                $("#Date").datebox("setValue", AllData["Date"]);
                $("#NextDate").datebox("setValue", AllData["NextDate"]);
                $("#OriginalStaffs").textbox("setValue", AllData["OriginalStaffs"]);
                var reply = AllData["Reply"];  //点评数组
                var strhtml = "";  //

                if (reply.length > 0) {
                    for (var i = 0; i < reply.length; i++) {
                        strhtml += "<tr>";
                        strhtml += "<td class='lbl' style='padding-top:20px'><strong>点评人员</strong></td>";
                        strhtml += "<td style='padding-top:20px'> ";
                        strhtml += reply[i].RealName;
                        strhtml += "</td>";
                        strhtml += "<td class='lbl' style='width: 80px;padding-top:20px'><strong>点评时间</strong></td>";
                        strhtml += "<td style='padding-top:20px'>";
                        strhtml += reply[i].UpdateDate.replace(/T/, ' ');
                        strhtml += "</td>";
                        strhtml += "</tr>";
                        strhtml += "<tr>";
                        strhtml += "<td class='lbl' style='width: 80px'><strong>点评内容</strong></td>";
                        strhtml += "<td colspan='3'>";
                        strhtml += reply[i].Context;
                        strhtml += "</td>";
                        strhtml += "</tr>";
                    }
                    $("#myComment").append(strhtml);
                    $("#DivComment").show(); //显示点评内容
                }
                UM.getEditor('editor').setDisabled('fullscreen');
            }
        });
    </script>
</body>
</html>
