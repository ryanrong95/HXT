<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Logistics.Carrier.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>承运商-系统配置</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var IDdata = eval('(<%=this.Model.IDdata%>)');
        var id = getQueryString("ID");
        $(function () {
            var CarrierTypeData = eval('(<%=this.Model.CarrierTypeData%>)');

            //初始化下拉框
            $('#CarrierType').combobox({
                data: CarrierTypeData,
            });
            var data = eval('(<%=this.Model.CarriersInfo%>)');
            $('#CarrierType').combobox('setValue', data['CarrierType']);
            $('#Code').textbox('setValue', data['Code']);
            $('#QueryMark').textbox('setValue', data['QueryMark']);
            $('#Name').textbox('setValue', data['Name']);
            $('#Summary').textbox('setValue', data['Summary']);
            $('#ContactName').textbox('setValue', data['ContactName']);
            $('#ContactMobile').textbox('setValue', data['ContactMobile']);
            $('#Fax').textbox('setValue', data['Fax']);
             $('#Address').textbox('setValue', data['Address']);
        });

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var code = $('#Code').textbox('getValue');
            var name = $('#Name').textbox('getValue');
            var id = getQueryString("ID");
            $.post('?action=IsExitCode', { ID: id, Code: code }, function (res) {
                if (!res) {
                    $.messager.alert('错误', "简称已存在");
                    return;
                }
                else {
                    $.post('?action=IsExitName', { ID: id, Name: name }, function (res) {
                        if (!res) {
                            $.messager.alert('错误', "名称已存在");
                            return;
                        } else {
                            SubmitDate();
                        }
                    });



                }
            });
        }

        function SubmitDate() {
            var data = new FormData($('#form1')[0]);
            data.append("ID", id)
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            $.myWindow.close();
                        });
                    }
                }
            }).done(function (res) {
            });


        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin-left: 20px">
                <tr>
                    <td class="lbl">承运商类型:</td>
                    <td>
                        <input class="easyui-textbox input" id="CarrierType" name="CarrierType" data-options="valueField:'TypeValue',textField:'TypeText',required:true,missingMessage:'请选择承运商类型'"  style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">简称:</td>
                    <td>
                        <input class="easyui-textbox input" id="Code" name="Code" data-options="validType:'length[1,50]',tipPosition:'right',required:true,missingMessage:'请输入简称'"  style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">查询标记:</td>
                    <td>
                        <input class="easyui-textbox input" id="QueryMark" name="QueryMark" data-options="validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入查询标记'"   style="width: 226px; height: 25px"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">名称:</td>
                    <td>
                        <input class="easyui-textbox input" id="Name" name="Name"
                            data-options="required:true,validType:'length[1,100]',tipPosition:'right',missingMessage:'请输入名称'"   style="width: 226px; height: 25px"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">联系人:</td>
                    <td>
                        <input class="easyui-textbox input" id="ContactName" name="ContactName"
                            data-options="validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入联系人'"   style="width: 226px; height: 25px"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">联系电话:</td>
                    <td>
                        <input class="easyui-textbox input" id="ContactMobile" name="ContactMobile"
                            data-options="tipPosition:'right',missingMessage:'请输入联系电话'"  style="width: 226px; height: 25px" />
                    </td>
                </tr>
                   <tr>
                    <td class="lbl">传真:</td>
                    <td>
                        <input class="easyui-textbox input" id="Fax" name="Fax"
                            data-options="tipPosition:'right'"  style="width: 226px; height: 25px" />
                    </td>
                </tr>
                   <tr>
                    <td class="lbl">承运商地址:</td>
                    <td>
                        <input class="easyui-textbox input" id="Address" name="Address"
                            data-options="tipPosition:'right' ,multiline:true,validType:'length[1,500]'" style="width: 300px; height: 40px" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl">备注:</td>
                    <td>
                        <input class="easyui-textbox input" id="Summary" name="Summary"
                            data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" style="width: 300px; height: 80px" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
