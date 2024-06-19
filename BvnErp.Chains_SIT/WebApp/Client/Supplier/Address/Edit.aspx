<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.Supplier.Address.Edit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="http://fix.szhxd.net/My/Scripts/area.data.js"></script>
    <script src="http://fix.szhxd.net/My/Scripts/areacombo.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
    <style type="text/css">
        .irtbaddress tr td input {
            width: 180px;
        }
    </style>
    <script type="text/javascript">
        var ClientSupplierID = '<%=this.Model.ClientSupplierID%>';//供应商ID
        var AddressID = '<%=this.Model.AddressID%>';//供应商地址ID
        if (AddressID != '') {
            SupplierAddressData = eval('(<%=this.Model.SupplierAddressData != null ? this.Model.SupplierAddressData:""%>)');
        }
         var places = eval('(<%=this.Model.Places%>)');
        $(function () {
             $("#Place").combobox({
                 data: places,
                 onLoadSuccess : function(){
			    $('#Place').combobox('setValue','HKG');
			}

            });
            $("#Address").area("setValue", "香港");
            //$("#Address").next().find("input[index='0']").combobox('readonly', true);//固定香港

            if (AddressID != '') {
                $("#ContactName").textbox("setValue", SupplierAddressData.ContactName);
                $("#Mobile").textbox("setValue", SupplierAddressData.Mobile);
                $("#Address").area("setValue", SupplierAddressData.Address.replace(new RegExp("#39;", "g"), "'"));
                $("#Summary").textbox("setValue", SupplierAddressData.Summary);
                $('#IsDefault').prop('checked', SupplierAddressData.IsDefault);
                $("#Place").combobox("setValue",SupplierAddressData.Place);
            }

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                var address = $("#Address").area("getValue");
                if (address.split(' ').length < 3) {
                    $.messager.alert('消息', '请录入正确的地址', 'info');
                    return;
                }

                var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
                values['ClientSupplierID'] = ClientSupplierID;//会员供应商ID
                values['AddressID'] = AddressID;//供应商地址ID
                values['Address'] = $("#Address").area("getValue").replace("'", "#39;");
                values['Summary'] = $("#Summary").textbox("getValue").replace("\'", "#39;");
                //提交后台
                $.post('?action=SaveClientSupplierAddress', { Model: JSON.stringify(values) }, function (res) {
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {
                            closeWin();
                        }
                    });
                });
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });
        });

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">联系人：</td>
                    <td>
                        <input class="easyui-textbox" id="ContactName"
                            data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入联系人'" style="width: 500px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">联系人电话：</td>
                    <td>
                        <input class="easyui-textbox" id="Mobile"
                            data-options="required:true,tipPosition:'bottom',missingMessage:'请输入联系人电话'" style="width: 500px" />
                    </td>
                </tr>
                  <tr>
                    <td class="lbl">国家/地区</td>
                    <td>
                        <input class="easyui-combobox" style="width: 500px;" disabled="disabled" data-options="valueField:'Code',textField:'Name',limitToList:true,required:true,tipPosition:'bottom'" id="Place" name="Place" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">提货地址：</td>
                    <td>
                        <table class="irtbaddress">
                            <tr>
                                <td>
                                    <div class="easyui-area" data-options="country:'中国',newline:true,newlinewidth:500" id="Address" name="Address"></div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

              
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary"
                            data-options="validType:'length[1,400]',tipPosition:'bottom',multiline:true," style="width: 500px; height: 60px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl"></td>
                    <td style="text-align: left">
                        <input type="checkbox" id="IsDefault" name="IsDefault" checked="checked" /><label for="IsDefault" style="margin-right: 30px">默认提货地址</label>
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        <a id="btnReturn" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
