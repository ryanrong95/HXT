<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.WorksOther.Edit" ValidateRequest="false" %>

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
        var AllData = eval('(<%=this.Model.AllData%>)');
        var editcontent = "";
        var Istempsave = false;
        var isCommitted = false;//表单是否已经提交标识，默认为false
        var path = eval('(<%=this.Model.Path%>)');
        var files = eval('(<%=this.Model.Files%>)');
        var id = getQueryString("ID");

        //页面加载时
        $(function () {

            //编辑器
            var editor = UM.getEditor("editor");
            UM.getEditor('editor').addListener('blur', function (e) {
                if (editcontent != editor.getContent()) {
                    Istempsave = true;
                } else {
                    Istempsave = false;
                }
            });

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
                if (AllData["Context"] != null) {
                    UM.getEditor('editor').setContent(escape2Html(AllData["Context"]));
                    editcontent = escape2Html(AllData["Context"]);
                }
                if (AllData["StartDate"] != null && AllData["StartDate"] != "") {
                    var startDateStr = new Date(AllData["StartDate"]).toDateTimeStr();
                    $("#StartDate").datetimebox("setValue", startDateStr);
                }
                $("#Subject").textbox("setValue", escape2Html(AllData["Subject"]));
                //初始化文件
                for (var j = 0; j < files.length; j++) {
                    document.getElementById('fileName').innerHTML += "<span path='" + files[j].Url + "' name='" + files[j].Name +
                        "' style='color:Blue'>文件名: " + files[j].Name + "</span></br>";
                }
            }

            win = $("#win").window({
                collapsible: false,
                minimizable: false,
                maximizable: false,
                closed: true,
            });

            //定时器,每隔1分钟暂存一次数据
            //window.t1 = window.setInterval(function () {
            //    var data = GetData();
            //    $.post('?action=SaveTemp', data, function (result) {
            //        window.id = result;
            //    });
            //}, 60000);
        });

        //保存数据校验
        function Save() {
            if (isCommitted == true) {
                alert("不要重复保存！");
                return;
            }
            var content = UM.getEditor('editor').getContent();
            if (content == "") {
                $.messager.alert('提示', '请输入计划内容！');
                return false;
            }
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
            $("#editorValue").val(content);

            isCommitted = true;
            Istempsave = false;
            //定时器清除
            //window.clearInterval(t1);

            //获取保存的文件
            var files = document.getElementById("fileName").getElementsByTagName("span");
            var filename = "", filepath = "";
            if (files.length > 0) {
                for (var i = 0; i < files.length; i++) {
                    filename += $(files[i]).attr("name") + ";";
                    filepath += $(files[i]).attr("path") + ";";
                }
            }

            var params = {
                action: 'Save',
                editorValue: content,
                fileNames: filename,
                filePaths: filepath,
                ID: window.id,
                StartDate: $("#StartDate").datetimebox("getValue"),
                Subject: $("#Subject").textbox("getValue"),
            };
            $.post(window.location.pathname, params, function () {
                $.messager.alert('提示', '保存成功！', 'info', function () {
                    $.myWindow.close();
                });
            });
            //$('#form1').form('submit', {
            //    url: window.location.pathname + '?' + $.param($.extend({ action: 'Save' }, {
            //        fileNames: filename,
            //        filePaths: filepath,
            //        ID: window.id,
            //    })),
            //    success: function (text) {
            //        $.myWindow.close();
            //    }
            //});
            return false;
        }

        //校验附件大小
        function UploadCheck() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
            //获取上传文件
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
                var fileurl = path + "/UploadFiles/" + name;
                document.getElementById('fileName').innerHTML += "<span path='" + fileurl + "' name='" + files[i].name +
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

        //校验文件格式
        function CheckType(name) {
            var isCheck = false;
            var type = [".docx", ".png", ".jpg", ".pdf", ".xls", ".doc", ".xlsx"];
            for (var i = 0; i < type.length; i++) {
                if (name.toLowerCase().indexOf(type[i]) >= 0) {
                    isCheck = true;
                    break;
                }
            }
            return isCheck;
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

        //关闭弹出页面
        function Close() {
            //定时器清除
            //window.clearInterval(t1);
            Istempsave = false;
            $.myWindow.close();
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
    </script>

    <script type="text/javascript">
        //暂存
        function SaveTemp() {
            //定时器清除
            //window.clearInterval(t1);

            var data = GetData();
            $.post('?action=SaveTemp', data, function () {
                $.messager.alert('提示', '数据暂存成功！', 'info', function () {
                    win.window('close');
                    Istempsave = false;
                    $.myWindow.close();
                });
            });
        }

        //获取暂存数据
        function GetData() {
            var data = {};
            data.ID = window.id;
            data.StartDate = $("#StartDate").datetimebox("getValue");
            data.Subject = $("#Subject").textbox("getValue");
            data.Context = UM.getEditor('editor').getContent();
            var files = window.document.getElementById("fileName").getElementsByTagName("span");
            data.filename = "", data.filepath = "";
            if (files.length > 0) {
                for (var i = 0; i < files.length; i++) {
                    data.filename += $(files[i]).attr("name") + ";";
                    data.filepath += $(files[i]).attr("path") + ";";
                }
            }
            return data;
        }

        //关闭窗口
        function CloseWin() {
            win.window('close');
            Close();
        }
        //继续编辑
        function Continue() {
            win.window('close');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post" enctype="multipart/form-data">
        <table id="table1" style="margin: 0 auto; width: 100%">
            <tr>
                <td class="lbl" style="width: 80px">开始时间</td>
                <td>
                    <input class="easyui-datetimebox" id="StartDate" name="StartDate" style="width: 180px" data-options="required:true,editable:false," />
                </td>
                <td class="lbl" style="width: 80px">计划主题</td>
                <td>
                    <input class="easyui-textbox" id="Subject" name="Subject" style="width: 180px" data-options="required:true,validType:'length[1,200]'" />
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
                    <script id="editor" type="text/plain" style="width: 95%; height: 400px;"></script>
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">附件上传</td>
                <td colspan="2">
                    <input class="easyui-filebox" id="fileImport" name="fileImport" style="width: 95%"
                        data-options="buttonText:'选择文件',accept:'.docx,.png,.jpg,.pdf,.xls,.doc,.xlsx',multiple:true," />
                </td>
                <td>
                    <a id="btnUpload" href="javascript:void(0);" class="easyui-linkbutton" onclick="UploadCheck()">上传</a>
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">附件预览</td>
                <td>
                    <label id="fileName"></label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td><span style="color: red; width: 200px">请上传xls,doc,docx,xlsx,jpg,png,pdf格式的文件，并且文件大小不大于4M</span></td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <asp:Button ID="btnSave" Text="保存" runat="server" OnClientClick="return Save();" />
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()" />
        </div>
        <div id="mm" class="easyui-menu" style="width: 120px;" data-options="onClick:menuHandler">
            <div data-options="name:'Show',iconCls:'icon-search'">预览</div>
            <div class="menu-sep"></div>
            <div data-options="name:'Delete',iconCls:'icon-cancel'">删除</div>
        </div>
        <div id="win" class="easyui-window" title="关闭窗口提示" style="width: 320px; height: 220px">
            <table id="table2" style="width: 280px; height: 180px; margin-left: 20px">
                <tr style="text-align: left;">
                    <td colspan="5">
                        <button id="btnSubmit" onclick="Save()" style="text-align: center">保存</button>
                        正常保存
                    </td>
                </tr>
                <tr style="text-align: left; margin-left: 40px">
                    <td colspan="5">
                        <button id="btnSaveTemp" onclick="SaveTemp()" style="text-align: center">暂存</button>
                        暂时保存，暂存数据不算考核计数
                    </td>
                </tr>
                <tr style="text-align: left; margin-left: 40px">
                    <td colspan="5">
                        <button id="btnCloseWin" onclick="CloseWin()" style="text-align: center">不暂存</button>
                        放弃新增或者编辑内容，数据不会保存
                    </td>
                </tr>
                <tr style="text-align: left; margin-left: 20px">
                    <td colspan="5">
                        <button id="btnContinue" onclick="Continue()" style="text-align: center">继续编辑</button>
                        关闭此提示窗口，继续编辑
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
