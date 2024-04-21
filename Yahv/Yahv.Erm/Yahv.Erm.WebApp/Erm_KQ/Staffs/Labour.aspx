<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Labour.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.Labour" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var StaffID = getQueryString("ID");
        $(function () {
            //表格初始化
            if (model.WageItemData != null) {
                $("#tb").css('display', 'block');//显示
                $("#salary").css('display', 'block');//显示
                fetchData(model.WageItemData);
            }
            //所属公司（社保公司）
            $("#EntryCompany").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.CompaniesData,
            })
            //银行名称
            $("#Bank").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.BankTypeData,
            })
            //提交
            $("#btnSubmit").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var data = new FormData();
                //基本信息
                data.append('StaffID', StaffID);
                data.append('EntryDate', $("#EntryDate").datebox("getValue"));
                data.append('ContractPeriod', $("#ContractPeriod").datebox("getValue"));
                data.append('ProbationMonths', $('#ProbationMonths').datebox('getValue'));
                data.append('EntryCompany', $("#EntryCompany").combobox("getValue"));
                data.append('EntryCompanyName', $("#EntryCompany").combobox("getText"));
                data.append('SocialSecurityAccount', $("#SocialSecurityAccount").textbox("getValue"));
                data.append('Bank', $("#Bank").combobox("getValue"));
                data.append('BankAddress', $("#BankAddress").textbox("getValue"));
                data.append('BankAccount', $("#BankAccount").textbox("getValue"));

                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                            Reload();
                        }
                    }
                })
            })
            //刷新页面
            $("#btnReload").click(function () {
                //岗位变更，工资项也需同步更新
                window.location.reload();
            })
            //初始化
            Init();
        });
    </script>
    <script>
        //初始化
        function Init() {
            if (model.LabourData != null) {
                $("#EntryDate").datebox("setValue", model.LabourData.EntryDate);
                $("#LeaveDate").datebox("setValue", model.LabourData.LeaveDate);
                $("#ContractPeriod").datebox("setValue", model.LabourData.ContractPeriod);
                $("#ProbationMonths").datebox("setValue", model.LabourData.ProbationMonths);
                //$("#EntryCompany").combobox("setValue", model.LabourData.EntryCompany);
                $("#EntryCompany").combobox("setValue", 'DBAEAB43B47EB4299DD1D62F764E6B6A');
                $("#SocialSecurityAccount").textbox("setValue", model.LabourData.SocialSecurityAccount);
            }
            if (model.BankData != null) {
                $("#Bank").combobox("setValue", model.BankData.Bank);
                $("#BankAddress").textbox("setValue", model.BankData.BankAddress);
                $("#BankAccount").textbox("setValue", model.BankData.BankAccount);
            }
        }
        //初始化表格
        function fetchData(data) {
            var s = "";
            s = "[[";
            $.each(data, function (index, value, array) {
                s += "{field:'" + value.ID + "',title:'" + value.Name + "',width: 80,editor: { type: 'numberbox'}},";
            });
            s = s + "]]";
            //使用js动态创建easyui的datagrid
            $('#tab1').myDatagrid({
                pagination: false,
                nowrap: true,
                columns: eval(s),
                fitColumns: false,
                toolbar: '#tb',
            });
        }
        //工资项表格编辑
        var firstEdit = true;
        function Edit() {
            if (firstEdit) {
                $('#tab1').datagrid('selectRow', 0).datagrid('beginEdit', 0);
                $('#tab1').datagrid('acceptChanges');
                firstEdit = false;
            }
            $('#tab1').datagrid('selectRow', 0).datagrid('beginEdit', 0);
        }
        //工资项表格保存
        function Save() {
            $('#tab1').datagrid('endEdit', 0);
            var changes = $('#tab1').datagrid('getChanges');
            if (changes.length == 0) {
                top.$.timeouts.alert({ position: "TC", msg: "无变更的项，无需保存", type: "error" });
                return;
            }
            $('#tab1').datagrid('acceptChanges');
            //保存
            var data = new FormData();
            var row = $('#tab1').datagrid('getRows')[0];
            data.append('Wages', JSON.stringify(row));
            ajaxLoading();
            $.ajax({
                url: '?action=SaveWages',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    ajaxLoadEnd();
                    var res = eval(res);
                    if (res.success) {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                    }
                    else {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                    }
                }
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'north',height:40">
            <div style="float: left; margin-left: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-save">保存</a>
                <a id="btnReload" class="easyui-linkbutton" iconcls="icon-reload">刷新</a>
            </div>
        </div>
        <div data-options="region:'center',fit:true,border:false" style="border: none;">
            <table class="liebiao">
                <tr>
                    <td class="lbl">所属公司：</td>
                    <td colspan="3">
                        <input id="EntryCompany" class="easyui-combobox" style="width: 250px;" data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">入职时间：</td>
                    <td>
                        <input id="EntryDate" class="easyui-datebox" style="width: 250px;" data-options="required:true" />
                    </td>
                    <td class="lbl">离职时间：</td>
                    <td>
                        <input id="LeaveDate" class="easyui-datebox" style="width: 250px;" data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">劳动合同期限：</td>
                    <td>
                        <input id="ContractPeriod" class="easyui-datebox" style="width: 250px;" data-options="required:true" />
                    </td>
                    <%--<td class="lbl">试用期时长(月)：</td>
                    <td>
                        <input id="ProbationMonths" class="easyui-numberbox" style="width: 250px;" data-options="required:true,min:0,precision:1" />
                    </td>--%>
                    <td class="lbl">试用期结束时间：</td>
                    <td>
                        <input id="ProbationMonths" class="easyui-datebox" style="width: 250px;" data-options="required:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行名称：</td>
                    <td>
                        <input id="Bank" class="easyui-combobox" style="width: 250px;" />
                    </td>
                    <td class="lbl">银行地址：</td>
                    <td>
                        <input id="BankAddress" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行账号：</td>
                    <td>
                        <input id="BankAccount" class="easyui-textbox" style="width: 250px;" />
                    </td>
                    <td class="lbl">社保账号：</td>
                    <td>
                        <input id="SocialSecurityAccount" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
            </table>
            <div id="tb" style="display: none">
                <a class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="Edit()">编辑</a>
                <a class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="Save()">保存</a>
            </div>
            <div id="salary" style="border: none; height: 200px; display: none">
                <table id="tab1" title="工资项默认值">
                </table>
            </div>
        </div>
    </div>
</asp:Content>
