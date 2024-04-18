<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NAdd.aspx.cs" Inherits="WebApp.Crm.Trace.NAdd" ValidateRequest="false" %>

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
        var TypeData = eval('(<%=this.Model.TypeData%>)');
        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var admins = eval('(<%=this.Model.Admins%>)');
        var path = eval('(<%=this.Model.Path%>)');
        var isCommitted = false;

        //页面加载时
        $(function () {
            //编辑器
            var editor = UM.getEditor("editor");
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

            $("#Date").datebox({
                onSelect: function (data) {
                    var currentdate = new Date().toDateStr();
                    var date1 = new Date(data).toDateStr();
                    if (date1 > currentdate) {
                        alert("跟进日期应该不晚于当天！");
                        $(this).datebox('clear');
                    }
                },
            });

            $("#NextDate").datebox({
                onSelect: function (data) {
                    var currentdate = new Date().toDateStr();
                    var date1 = new Date(data).toDateStr();
                    if (currentdate >= date1) {
                        alert("下次跟进日期应该晚于当天！");
                        $(this).datebox('clear');
                    }
                },
            });

            //校验输入框内容
            $("#Type").combobox("textbox").bind("blur", function () {
                var value = $("#Type").combobox("getValue");
                var data = $("#Type").combobox("getData");
                var valuefiled = $("#Type").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Type").combobox("clear");
                }
            });
            $("#ClientID").combobox("textbox").bind("blur", function () {
                var value = $("#ClientID").combobox("getValue");
                var data = $("#ClientID").combobox("getData");
                var valuefiled = $("#ClientID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ClientID").combobox("clear");
                }
            });
            $("#Reader").combobox("textbox").bind("blur", function () {
                var values = [];
                $.map($("#Reader").combobox("getValues"), function (value) {
                    var data = $("#Reader").combobox("getData");
                    var valuefiled = $("#Reader").combobox("options").valueField;
                    var index = $.easyui.indexOfArray(data, valuefiled, value);
                    if (index >= 0) {
                        values.push(value);
                    }
                });
                $("#Reader").combobox("setValues", values);
            });
        });

        //保存校验
        function Save() {
            isCommitted = true;
            var content = UM.getEditor('editor').getContent();
            $("#editorValue").val(content);
            if (content == "") {
                $.messager.alert('提示', '请输入跟进内容！');
                return false;
            }
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
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
                Type: $("#Type").combobox("getValue"),
                ClientID: $("#ClientID").combobox("getValue"),
                Date: $("#Date").datebox("getValue"),
                NextDate: $("#NextDate").datebox("getValue"),
                OriginalStaffs: $('#OriginalStaffs').textbox("getValue"),
                Reader: $("#Reader").combobox("getValues"),
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
            //    })),
            //    success: function (text) {
            //        $.myWindow.close();
            //    }
            //});
            return false;
        }

        //附件上传校验
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

        //关闭当前弹框
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post" enctype="multipart/form-data">
        <table id="table1" style="margin: 0 auto; width: 800px">
            <tr>
                <td class="lbl" style="width: 80px">跟进日期</td>
                <td colspan="1">
                    <input class="easyui-datebox" id="Date" name="Date" style="width: 180px"
                        data-options="editable:false,required:true,currentText:'',closeText:''," />
                </td>
                <td class="lbl" style="width: 80px">跟进方式</td>
                <td>
                    <input class="easyui-combobox" id="Type" name="Type" style="width: 180px"
                        data-options="valueField:'value',textField:'text',required:true,data:TypeData, panelMaxHeight:'100px'," />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">客户</td>
                <td>
                    <input class="easyui-combobox" id="ClientID" name="ClientID" style="width: 180px"
                        data-options="valueField:'ID',textField:'Name',required:true,data: ClientData, panelMaxHeight:'150px'," />
                </td>
                <td class="lbl" style="width: 80px">下次跟进日期</td>
                <td>
                    <input class="easyui-datebox" id="NextDate" name="NextDate" type="text" style="width: 180px"
                        data-options="editable:false,required:true,currentText:'',closeText:''," />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">原厂陪同人员</td>
                <td>
                    <input class="easyui-textbox" id="OriginalStaffs" name="OriginalStaffs" data-options="validType:'length[1,300]'" style="width: 180px" />
                </td>
                <td class="lbl" style="width: 80px">指定阅读人</td>
                <td>
                    <input class="easyui-combobox" id="Reader" name="Reader" style="width: 180px"
                        data-options="valueField:'ID',textField:'RealName',data: admins,multiple:true," />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">跟进内容</td>
                <td colspan="4">
                    <input id="editorValue" type="hidden" />
                    <script id="editor" type="text/plain" style="width: 600px; height: 200px;"></script>
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
            <asp:Button ID="btnSumit" Text="保存" runat="server" OnClientClick="return Save()" />
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
