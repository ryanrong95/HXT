<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Notice.Edit" %>

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
        var path = eval('(<%=this.Model.Path%>)');
        var isSave = false;

        //页面加载时
        $(function () {
            //编辑器
            var editor = UM.getEditor("editor");
            $("#CreateDate").textbox('setValue', new Date().toDateTimeStr());
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
        });

        //保存数据校验
        function Save() {
            //重复保存校验
            if (isSave == true) {
                alert("不要重复保存！");
                return false;
            }
            var content = UM.getEditor('editor').getContent();

            if ($("#Name").textbox('getValue') == "") {
                alert('请输入公告标题！');
                return false;
            }
            if (content == "") {
                alert('请输入公告内容！');
                return false;
            }
            $("#editorValue").val(content);

            isSave = true;
            //获取保存的文件
            var files = document.getElementById("fileName").getElementsByTagName("span");
            var filename = "", filepath = "";
            if (files.length > 0) {
                for (var i = 0; i < files.length; i++) {
                    filename += $(files[i]).attr("name") + ";";
                    filepath += $(files[i]).attr("path") + ";";
                }
            }
            $('#form1').form('submit', {
                url: window.location.pathname + '?' + $.param($.extend({ action: 'Save' }, {
                    fileNames: filename,
                    filePaths: filepath,
                })),
                success: function (text) {
                    $.myWindow.close();
                }
            });
            return false;
        }

        //文件上传
        function Upload() {
            var files = document.getElementById("filebox_file_id_1").files;
            var fileNames = "";
            if (files.length == 0) {
                alert("请选择上传的文件!");
                return false;
            }
            for (var i = 0; i < files.length; i++) {
                if (files[i].size > 4096 * 1024) {
                    alert("上传的文件不得大于4M!");
                    return false;
                }
                if (!CheckType(files[0].name)) {
                    alert("请上传符合格式要求的文件！");
                    return false;
                }
                var index = files[i].name.lastIndexOf('.');
                var name = new Date().Format("yyyyMMddhhmmssS") + i + files[i].name.substr(index);
                fileNames += name + ";";
                var url = path + "/UploadFiles/" + name;
                document.getElementById('fileName').innerHTML += "<span path='" + url + "' name='" + files[i].name +
                    "' style='color:Blue'>文件名: " + files[i].name + "</span></br>";

            }
            $('#form1').form('submit', {
                url: window.location.pathname + '?action=Upload&fileNames=' + fileNames,
                success: function (text) {
                    var filebox = document.getElementById("fileImport");
                    $(filebox).textbox('clear');
                    $.messager.alert("提示", "上传成功！");
                }
            });
        }

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
            if (item.name == "Delete") {
                current.remove();
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

        //校验文件格式
        function CheckType(name) {
            var isCheck = false;
            var type = [".docx", ".png", ".jpg", ".pdf", ".xls", ".doc", ".xlsx",".ppt",".pptx"];
            for (var i = 0; i < type.length; i++) {
                if (name.toLowerCase().indexOf(type[i]) >= 0) {
                    isCheck = true;
                    break;
                }
            }
            return isCheck;
        }

        //时间格式化
        Date.prototype.Format = function (fmt) { //author: meizz 
            var o = {
                "M+": this.getMonth() + 1, //月份 
                "d+": this.getDate(), //日 
                "h+": this.getHours(), //小时 
                "m+": this.getMinutes(), //分 
                "s+": this.getSeconds(), //秒 
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
                "S": this.getMilliseconds() //毫秒 
            };
            if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        }

        //关闭弹出页面
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post" enctype="multipart/form-data">
        <input type="hidden" runat="server" id="hidName" />
        <input type="hidden" runat="server" id="hidPath" />
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
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 90%" data-options="validType:'length[1,150]'" />
                </td>
                <td class="lbl" style="text-align: center;">发布时间</td>
                <td>
                    <input class="easyui-textbox" id="CreateDate" name="CreateDate" style="width: 90%" data-options="readonly:true" />
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
            <tr>
                <td class="lbl" style="text-align: center;">附件上传</td>
                <td colspan="2">
                    <input class="easyui-filebox" id="fileImport" name="fileImport" style="width: 95%"
                        data-options="buttonText:'选择文件',accept:'.docx,.png,.jpg,.pdf,.xls,.doc,.xlsx,.ppt',multiple:true," />
                </td>
                <td>
                    <a id="btnUpload" href="javascript:void(0);" class="easyui-linkbutton" onclick="Upload()">上传</a>
                </td>
            </tr>
            <tr>
                <td class="lbl" style="text-align: center;">附件预览</td>
                <td>
                    <label id="fileName"></label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td><span style="color: red; width: 200px">请上传xls,doc,docx,xlsx,jpg,png,pdf.ppt,.pptx格式的文件，并且文件大小不大于4M</span></td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 10px">
            <asp:Button ID="btnSave" Text="保存" runat="server" OnClientClick="return Save();"/>
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()" />
        </div>
        <div id="mm" class="easyui-menu" style="width: 120px;" data-options="onClick:menuHandler">
            <div data-options="name:'Show',iconCls:'icon-search'">预览</div>
            <div class="menu-sep"></div>
            <div data-options="name:'Delete',iconCls:'icon-cancel'">删除</div>
        </div>
    </form>
</body>
</html>
