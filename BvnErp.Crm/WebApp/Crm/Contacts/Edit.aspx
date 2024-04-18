<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Contacts.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="http://fixed2.b1b.com/My/Scripts/area.data.js"></script>
    <script src="http://fixed2.b1b.com/My/Scripts/areacombo.js"></script>
    <script type="text/javascript">        
        var sex = eval('(<%=this.Model.Sex%>)');
        var contact = eval(<%=this.Model.Contact%>);
        var path = eval('(<%=this.Model.Path%>)');
        var files = eval('(<%=this.Model.Files%>)');

        //页面加载时
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

            if (contact != null) {
                $("#Name").textbox("setValue", contact["Name"]);
                $("#Sex").combobox("setValue", contact["Sex"]);
                $("#Position").textbox("setValue", contact["Position"]);
                $("#Department").textbox("setValue", contact["Department"]);
                $("#AuthorityRange").textbox("setValue", contact["AuthorityRange"]);
                $("#Mobile").numberbox("setValue", contact["Mobile"]);
                $("#Tel").textbox("setValue", contact["Tel"]);
                $("#Email").textbox("setValue", contact["Email"]);
                $("#Fax").textbox("setValue", contact["Fax"]);
                $("#QQ").textbox("setValue", contact["QQ"]);
                $("#WeChat").textbox("setValue", contact["WeChat"]);
                $("#MSN").textbox("setValue", contact["MSN"]);
                $("#SKYPE").textbox("setValue", contact["SKYPE"]);
                $("#Birthday").textbox("setValue", contact["Birthday"]);
                $("#NativePlace").textbox("setValue", contact["NativePlace"]);
                $("#Character").textbox("setValue", contact["Character"]);
                $("#Hobby").textbox("setValue", contact["Hobby"]);
                $("#Taboo").textbox("setValue", contact["Taboo"]);
                $("#Home").textbox("setValue", contact["Home"]);
                $("#Record").textbox("setValue", contact["Record"]);
                $("#Other").textbox("setValue", contact["Other"]);
                if (contact["Address"] != "") {
                    $("#Address").area('setValue', contact["Address"]);
                }

                for (var j = 0; j < files.length; j++) {
                    document.getElementById('fileName').innerHTML += "<span path='" + files[j].Url + "' name='" + files[j].Name +
                        "' style='color:Blue'>文件名: " + files[j].Name + "</span></br>";
                }
            }

            $("#Sex").combobox("textbox").bind("blur", function () {
                var value = $("#Sex").combobox("getValue");
                var data = $("#Sex").combobox("getData");
                var valuefiled = $("#Sex").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Sex").combobox("clear");
                }
            });
        });

        //关闭
        function closeWin() {
            $.myWindow.close();
        }

        //保存校验
        function Save() {
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

            $('#form1').form('submit', {
                url: window.location.pathname + '?' + $.param($.extend({ action: 'Save' }, {
                    fileNames: filename,
                    filePaths: filepath,
                    ID: getQueryString("ID"),
                    ClientID: getQueryString("ClientID"),
                })),
                success: function (text) {
                    $.myWindow.close();
                }
            });
            return false;
        }

        //名片上传校验
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
                var url = $(current).attr("path");
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '预览',
                    url: url,
                }).open();
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
            var type = [".png", ".jpg", ".pdf"];
            for (var i = 0; i < type.length; i++) {
                if (name.indexOf(type[i]) >= 0) {
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
    </script>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server" method="post" enctype="multipart/form-data">
            <table id="table1">
                <tr>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                </tr>
                <tr>
                    <td class="lbl">姓名</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name"
                            data-options="required:true,validType:'length[1,10]',tipPosition:'bottom'" style="width: 95%" />
                    </td>
                    <td class="lbl">性别</td>
                    <td>
                        <input class="easyui-combobox" id="Sex" name="Sex"
                            data-options="valueField:'value',textField:'text',required:true,data: sex," style="width: 95%" />
                    </td>
                    <td class="lbl">职位</td>
                    <td>
                        <input class="easyui-textbox" id="Position" name="Position"
                            data-options="required:true,validType:'length[1,20]'" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">部门</td>
                    <td>
                        <input class="easyui-textbox" id="Department" name="Department" data-options="validType:'length[1,20]'"
                            style="width: 95%" />
                    </td>
                    <td class="lbl">权力范围</td>
                    <td>
                        <input class="easyui-textbox" id="AuthorityRange" name="AuthorityRange" data-options="validType:'length[1,20]'"
                            style="width: 95%" />
                    </td>
                    <td class="lbl">生日</td>
                    <td>
                        <input class="easyui-datebox" id="Birthday" name="Birthday" style="width: 95%" data-options="editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">手机号码</td>
                    <td>
                        <input class="easyui-numberbox" id="Mobile" name="Mobile"
                            data-options="required:true,validType:'length[11,11]',invalidMessage:'请输入11位手机号码',tipPosition:'bottom'" style="width: 95%" />
                    </td>
                    <td class="lbl">电话号码</td>
                    <td>
                        <input class="easyui-textbox" id="Tel" name="Tel"
                            data-options="validType:'length[1,20]'" style="width: 95%" />
                    </td>
                    <td class="lbl">邮箱</td>
                    <td>
                        <input class="easyui-textbox" id="Email" name="Email"
                            data-options="validType:'email',validType:'length[1,20]'" style="width: 95%" />
                    </td>

                </tr>
                <tr>
                    <td class="lbl">微信</td>
                    <td>
                        <input class="easyui-textbox" id="WeChat" name="WeChat" data-options="validType:'length[1,20]',tipPosition:'bottom'"
                            style="width: 95%" />
                    </td>
                    <td class="lbl">QQ</td>
                    <td>
                        <input class="easyui-textbox" id="QQ" name="QQ" data-options="validType:'length[1,20]'"
                            style="width: 95%" />
                    </td>
                    <td class="lbl">传真</td>
                    <td>
                        <input class="easyui-textbox" id="Fax" name="Fax" data-options="validType:'length[1,20]'"
                            style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">MSN</td>
                    <td>
                        <input class="easyui-textbox" id="MSN" name="MSN" data-options="validType:'length[1,20]',tipPosition:'bottom'"
                            style="width: 95%" />
                    </td>
                    <td class="lbl">SKYPE</td>
                    <td>
                        <input class="easyui-textbox" id="SKYPE" name="SKYPE" data-options="validType:'length[1,20]'"
                            style="width: 95%" />
                    </td>
                    <td class="lbl">籍贯</td>
                    <td>
                        <input class="easyui-textbox" id="NativePlace" name="NativePlace" data-options="validType:'length[1,50]',tipPosition:'bottom'"
                            style="width: 95%" />
                    </td>

                </tr>
                <tr>

                    <td class="lbl">性格</td>
                    <td>
                        <input class="easyui-textbox" id="Character" name="Character" data-options="validType:'length[1,50]'"
                            style="width: 95%" />
                    </td>
                    <td class="lbl">兴趣</td>
                    <td>
                        <input class="easyui-textbox" id="Hobby" name="Hobby" data-options="validType:'length[1,50]'"
                            style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">忌讳</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Taboo" name="Taboo" data-options="validType:'length[1,100]',tipPosition:'bottom'"
                            style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">住址</td>
                    <td colspan="5">
                        <div class="easyui-area" data-options="country:'中国',validType:'length[1,50]'," id="Address" name="Address"></div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">家庭状况</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Home" name="Home" data-options="multiline:true,validType:'length[1,300]',tipPosition:'bottom'"
                            style="width: 95%; height: 80px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">人生履历</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Record" name="Record" data-options="multiline:true,validType:'length[1,300]',tipPosition:'bottom'"
                            style="width: 95%; height: 80px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">其他备注</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Other" name="Other" data-options="multiline:true,validType:'length[1,300]',tipPosition:'bottom'"
                            style="width: 95%; height: 80px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 80px">名片上传</td>
                    <td colspan="2">
                        <input class="easyui-filebox" id="fileImport" name="fileImport" style="width: 95%"
                            data-options="buttonText:'选择文件',accept:'.png,.jpg,.pdf',multiple:true," />
                    </td>
                    <td>
                        <a id="btnUpload" href="javascript:void(0);" class="easyui-linkbutton" onclick="UploadCheck()">上传</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 80px">名片预览</td>
                    <td>
                        <label id="fileName"></label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td><span style="color: red; width: 200px">请上传jpg,png,pdf格式的文件，并且文件大小不大于4M</span></td>
                </tr>
            </table>
            <div id="divSave" style="text-align: center">
                <asp:Button runat="server" ID="btnSave" Text="保存" OnClientClick="return Save();" />
                <asp:Button runat="server" ID="Button1" Text="取消" OnClientClick="closeWin()" />
            </div>
            <div id="mm" class="easyui-menu" style="width: 120px;" data-options="onClick:menuHandler">
                <div data-options="name:'Show',iconCls:'icon-search'">预览</div>
                <div class="menu-sep"></div>
                <div data-options="name:'Delete',iconCls:'icon-cancel'">删除</div>
            </div>
        </form>
    </div>
</body>
</html>
