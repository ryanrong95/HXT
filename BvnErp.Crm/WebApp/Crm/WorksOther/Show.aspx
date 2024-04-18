<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="WebApp.Crm.WorksOther.Show" ValidateRequest="false" %>

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
    <script type="text/javascript">
        function Close() {
            $.myWindow.close();
        }
        //打开点评页面
        function Add() {
            var url = location.pathname.replace(/WorksOther/ig, 'WorksWeekly');
            url = url.replace(/Show.aspx/ig, 'WeeklyComment.aspx') + "?ID=" + getQueryString("ID") + "&Type=50";
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '我的点评',
                width: "420px",
                height: "220px",
                url: url,
                onClose: function () {
                    window.location.reload(); //刷新当前页面
                }
            }).open();
        }

        //校验文件是否预览
        function CheckShow(name) {
            var isCheck = false;
            var type = [".png", ".jpg", ".pdf"];
            for (var i = 0; i < type.length; i++) {
                if (name.toLowerCase().indexOf(type[i]) >= 0) {
                    isCheck = true;
                    break;
                }
            }
            return isCheck;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto; width: 100%">
            <tr>
                <td class="lbl" style="width: 80px">开始时间</td>
                <td>
                    <input  class="easyui-datetimebox" id="StartDate" name="StartDate" style="width: 180px" 
                        data-options="readonly:true"/>
                </td>
                <td class="lbl" style="width: 80px">计划主题</td>
                <td>
                    <input  class="easyui-textbox" id="Subject" name="Subject" style="width: 180px"
                        data-options="readonly:true"/>
                </td>
                <td class="lbl" style="width: 80px">计划人</td>
                <td>
                    <asp:Label ID="AdminName" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">计划内容</td>
                <td colspan="5">
                    <input id="editorValue" type="hidden" />
                    <script id="editor" type="text/plain" style="width: 95%; height: 70%;"></script>
                </td>
            </tr>
            <tr id="check">
                <td class="lbl" style="text-align: center;">附件查看</td>
                <td>
                    <label id="fileName"></label>
                </td>
            </tr>
        </table>
        <div id="DivComment" style="width: 95%">
            <div class="easyui-panel" title="主管点评" style="width: 100%;" data-options="border:false"></div>
            <table id="myComment" style="margin: 0 auto; width: 95%; height: 95%">
            </table>
        </div>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <a id="Comment" href="javascript:void(0);" class="easyui-linkbutton" onclick="Add()">点评</a>
        </div>
    </form>
    <script type="text/javascript">
        //编辑器
        var editor = UM.getEditor("editor");
        var editorComment = UM.getEditor("editorComment");
        UM.getEditor('editor').setDisabled('fullscreen');
        var AllData = eval('(<%=this.Model.AllData%>)');

        //页面加载时
        $(function () {
            //初始化赋值
            if (AllData != null) {
                UM.getEditor('editor').setContent(escape2Html(AllData["Context"]));
                var startDateStr = new Date(AllData["StartDate"]).toDateTimeStr();
                $("#StartDate").datetimebox("setValue", startDateStr);
                $("#Subject").textbox("setValue", escape2Html(AllData["Subject"]));
                if (AllData["files"].length > 0) {
                    $("#check").show();
                    for (var j = 0; j < AllData["files"].length; j++) {
                        document.getElementById('fileName').innerHTML += "<a href='" + AllData["files"][j].URL + "' target='_blank' style='color:Blue'>文件名: "
                            + AllData["files"][j].Name + "</a></br>";
                    }
                }
                else {
                    $("#check").hide();
                }
                $("#DivComment").hide();
                var reply = AllData["Reply"];  //点评数组
                var strhtml = "";  //
                if (AllData.IsOwner) {
                    $("#Comment").hide();  //点评按钮隐藏
                }            
                if (reply.length > 0) {
                    for (var i = 0; i < reply.length; i++) {
                        strhtml += "<tr >";
                        strhtml += "<td class='lbl' style='padding-top:20px'><strong>点评人员</strong></td>";
                        strhtml += "<td style='padding-top:20px'> ";
                        strhtml += reply[i].RealName;
                        strhtml += "</td>";
                        strhtml += "<td class='lbl' style='width: 80px;padding-top:20px'><strong>点评时间</strong></td>";
                        strhtml += "<td style='padding-top:20px'>";
                        strhtml += reply[i].UpdateDate.replace(/T/, ' ');
                        strhtml += "</td>";
                        strhtml += " </tr>";
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
            }
        });
    </script>
</body>
</html>
