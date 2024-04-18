<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Finance.Account.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>金库账户编辑</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        var CurrData = eval('(<%=this.Model.CurrData%>)');
        var Admins = eval('(<%=this.Model.AllAdmin%>)');      
        var AccountType = eval('(<%=this.Model.AccountType%>)');
        var Countries = eval('(<%=this.Model.Countries%>)');
        var isCommitted = false;//表单是否已经提交标识，默认为false
        $(function () {
            //初始化Combobox
            $('#FinanceVault').combobox({
                data: FinanceVaultData,
            })
            $('#Currency').combobox({
                data: CurrData,
            })
            //$('#AccountType').combobox({
            //     data: AccountType,
            //})
            $('#Owner').combobox({
                data: Admins,
            })
             $('#Region').combobox({
                data: Countries,
             })

            $('#AccountType').combobox({
                data: AccountType,
                panelHeight: 'auto',//自适应
                multiple: true,
                formatter: function (row) {
                    var opts = $(this).combobox('options');
                    return '<input type="checkbox" class="combobox-checkbox" id="' + row[opts.valueField] + '">' + row[opts.textField]
                },

                onShowPanel: function () {
                    var opts = $(this).combobox('options');
                    var target = this;
                    var values = $(target).combobox('getValues');
                    $.map(values, function (value) {
                        var el = opts.finder.getEl(target, value);
                        el.find('input.combobox-checkbox')._propAttr('checked', true);
                    })
                },
                onLoadSuccess: function () {
                    var opts = $(this).combobox('options');
                    var target = this;
                    var values = $(target).combobox('getValues');
                    $.map(values, function (value) {
                        var el = opts.finder.getEl(target, value);
                        el.find('input.combobox-checkbox')._propAttr('checked', true);
                    })
                },
                onSelect: function (row) {
                    var opts = $(this).combobox('options');
                    var el = opts.finder.getEl(this, row[opts.valueField]);
                    el.find('input.combobox-checkbox')._propAttr('checked', true);
                },
                onUnselect: function (row) {
                    var opts = $(this).combobox('options');
                    var el = opts.finder.getEl(this, row[opts.valueField]);
                    el.find('input.combobox-checkbox')._propAttr('checked', false);
                }
            });
           
            //初始化赋值
            if (AllData != null && AllData != "") {
                $("#FinanceVault").combobox("setValue", AllData["FinanceVault"]);
                $("#AccountName").textbox("setValue", AllData["AccountName"]);
                $("#BankName").textbox("setValue", AllData["BankName"]);
                $("#BankAddress").textbox("setValue", AllData["BankAddress"]);
                $("#BankAccount").textbox("setValue", AllData["BankAccount"]);
                $("#BankAccount").textbox('disable');
                $("#SwiftCode").textbox("setValue", AllData["SwiftCode"]);
                $("#CustomizedCode").textbox("setValue", AllData["CustomizedCode"]);
                $("#Currency").combobox("setValue", AllData["Currency"]);
                $("#Balance").textbox("setValue", AllData["Balance"]);
                $("#Summary").textbox("setValue", AllData["Summary"]);
                $('#Balance').textbox('textbox').attr('readonly', true);
                $("#Currency").combobox('readonly', true);
                $("#AccountType").combobox("setValues", AllData["AccountType"]);
                $("#Company").combobox("setValue", AllData["CompanyName"]);
                $("#Owner").combobox("setValue", AllData["AdminInchargeID"]);
                $("#Region").combobox("setValue", AllData["Region"]);
            }
        });
        //关闭弹出页面
        function Close() {
            $.myWindow.close();
        }
        //校验重复提交
        function CheckSubmit() {
            if (isCommitted == false) {
                isCommitted = true;//提交表单后，将表单是否已经提交标识设置为true
                return true;//返回true让表单正常提交
            } else {
                return false;//返回false那么表单将不提交
            }
        }
        //保存校验
        function Save() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            var data = new FormData($('#form1')[0]);           
            if (AllData != "") {
                data.append('ID', AllData["ID"])
                data.append('BankAccount', AllData["BankAccount"])
            }
            MaskUtil.mask();
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success) {
                        $.messager.alert('消息', res.message, 'info', function () {
                            Close();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">金库：</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceVault" name="FinanceVault" data-options="required:true,height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'" />
                    </td>
                    <td class="lbl">账户名称：</td>
                    <td>
                        <input class="easyui-textbox" id="AccountName" name="AccountName" data-options="required:true,height:28,width:250,validType:'length[1,100]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行名称：</td>
                    <td>
                        <input class="easyui-textbox" id="BankName" name="BankName" data-options="required:true,height:28,width:250,validType:'length[1,150]'" />
                    </td>
                    <td class="lbl">银行地址：</td>
                    <td>
                        <input class="easyui-textbox" id="BankAddress" name="BankAddress" data-options="required:true,height:28,width:250,validType:'length[1,250]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行账号：</td>
                    <td>
                        <input class="easyui-textbox" id="BankAccount" name="BankAccount" data-options="required:true,height:28,width:250,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">银行代码：</td>
                    <td>
                        <input class="easyui-textbox" id="SwiftCode" name="SwiftCode" data-options="height:28,width:250,validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">币种：</td>
                    <td>
                        <input class="easyui-combobox" id="Currency" name="Currency" data-options="required:true,height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'" />
                    </td>
                    <td class="lbl">余额：</td>
                    <td>
                        <input class="easyui-textbox" id="Balance" name="Balance" data-options="required:true,height:28,width:250,validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">所属公司：</td>
                    <td>
                        <select class="easyui-combobox" id="Company" name="Company" style="width:250px;height:28px">
                            <option value="深圳市芯达通供应链管理有限公司" selected="selected">深圳市芯达通供应链管理有限公司</option>
                            <option value="香港万路通国际贸易有限公司">香港万路通国际贸易有限公司</option>
                        </select>
                    </td>
                    <td class="lbl">账号类型：</td>
                    <td>
                        <input class="easyui-combobox" id="AccountType" name="AccountType" data-options="required:true,height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'"  />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">管理人：</td>
                    <td>
                        <input class="easyui-combobox" id="Owner" name="Owner" data-options="required:true,height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'" />
                    </td>
                   <td class="lbl">国家地区：</td>
                    <td>
                        <input class="easyui-combobox" id="Region" name="Region" data-options="height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">自定义代码：</td>
                    <td>
                        <input class="easyui-textbox" id="CustomizedCode" name="CustomizedCode" data-options="height:28,width:250,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary" data-options="required:false,height:28,width:250,validType:'length[1,500]'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
