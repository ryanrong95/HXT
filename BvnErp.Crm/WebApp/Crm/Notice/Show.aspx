<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="WebApp.Crm.Notice.Show" %>

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

        //关闭弹出页面
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto; width: 100%">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr>
                <td class="lbl" style="text-align: center;">公告主题</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 90%" data-options="readonly:true" />
                </td>
                <td class="lbl" style="text-align: center;">发布时间</td>
                <td>
                    <input class="easyui-datetimebox" id="CreateDate" name="CreateDate" style="width: 90%" data-options="readonly:true" />
                </td>
                <td class="lbl" style="text-align: center;">发布人</td>
                <td>
                    <asp:Label ID="AdminName" runat="server" Text="Label" Style="width: 90%"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="lbl" style="text-align: center;">公告内容</td>
                <td colspan="5">
                    <input id="editorValue" type="hidden" />
                    <script id="editor" type="text/plain" style="width: 90%; height: 100%;"></script>
                </td>
            </tr>
            <tr id="IsShow">
                <td class="lbl" style="text-align: center;">附件查看</td>
                <td>
                    <label id="fileName"></label>
                </td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <asp:Button ID="btnClose" Text="关闭" runat="server" OnClientClick="Close()" />
        </div>
        <div id="mm" class="easyui-menu" style="width: 120px;" data-options="onClick:menuHandler">
            <div data-options="name:'Show',iconCls:'icon-search'">预览</div>
        </div>
    </form>
    <script type="text/javascript">
        //编辑器初始化
        var editor = UM.getEditor("editor");
        //编辑器只读
        UM.getEditor('editor').setDisabled('fullscreen');
        var AllData = eval('(<%=this.Model.AllData%>)');
        var files = eval('(<%=this.Model.Files%>)');

        $(function () {

            //加载右击菜单
            startMenu(document.getElementById("fileName"));

            $("#fileName").tooltip({
                position: 'right',
                content: '<span style="color:#fff">点击鼠标右键查看或者删除操作</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666',
                    });
                },
            });

            //初始化赋值
            if (AllData != null) {
                UM.getEditor('editor').setContent(escape2Html(AllData.Context));
                $("#Name").textbox("setValue", escape2Html(AllData.Name));
                $("#CreateDate").datetimebox("setValue", new Date(AllData.CreateDate).toDateTimeStr());
                if (files.length > 0) {
                    $("#IsShow").show();
                    for (var j = 0; j < files.length; j++) {
                        document.getElementById('fileName').innerHTML += "<span path='" + files[j].Url + "' name='" + files[j].Name +
                            "' style='color:Blue'>文件名: " + files[j].Name + "</span></br>";
                    }
                }
                else {
                    $("#IsShow").hide();
                }

            }
        });


        //加载菜单
        function startMenu(context) {
            $(context).bind('contextmenu', function (e) {
                e.preventDefault();
                $('#mm').data('current', $(e.target));
                $('#mm').menu('show', {
                    left: e.pageX,
                    top: e.pageY,
                });
            });
        }

        //按钮点击触发事件
        function menuHandler(item) {
            var current = $('#mm').data('current');
            if (item.name == "Show") {
                var name = $(current).attr("name");
                var url = $(current).attr("path");
                if (CheckShow(name)) {
                    top.$.myWindow({
                        iconCls: "",
                        noheader: false,
                        title: '预览',
                        url: url,
                    }).open();
                }
                else {
                    window.location.href = url;
                }
                return;
            }
        }

        //校验文件是否预览
        function CheckShow(name) {
            var isCheck = false;
            var type = [".png", ".jpg", ".pdf", ".txt"];
            for (var i = 0; i < type.length; i++) {
                if (name.toLowerCase().indexOf(type[i]) >= 0) {
                    isCheck = true;
                    break;
                }
            }
            return isCheck;
        }
    </script>
</body>
</html>
