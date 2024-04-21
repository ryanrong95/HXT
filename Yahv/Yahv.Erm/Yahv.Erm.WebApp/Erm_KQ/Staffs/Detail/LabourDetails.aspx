<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="LabourDetails.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.Detail.LabourDetails" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var StaffID = getQueryString("ID");
        $(function () {
            //表格初始化
            if (model.WageItemData != null) {
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
            //初始化
            Init();
        });
    </script>
    <script>
        //初始化
        function Init() {
            $("#EntryDate").datebox("setValue", model.LabourData.EntryDate);
            $("#LeaveDate").datebox("setValue", model.LabourData.LeaveDate);
            $("#ContractPeriod").datebox("setValue", model.LabourData.ContractPeriod);
            $("#ProbationMonths").datebox("setValue", model.LabourData.ProbationMonths);
            //$("#EntryCompany").combobox("setValue", model.LabourData.EntryCompany);
            $("#EntryCompany").combobox("setValue", 'DBAEAB43B47EB4299DD1D62F764E6B6A');
            $("#SocialSecurityAccount").textbox("setValue", model.LabourData.SocialSecurityAccount);

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
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center',fit:true,border:false" style="border: none;">
            <table class="liebiao">
                <tr>
                    <td class="lbl">入职时间：</td>
                    <td>
                        <input id="EntryDate" class="easyui-datebox" style="width: 250px;" data-options="disabled:true," />
                    </td>
                    <td class="lbl">离职时间：</td>
                    <td>
                        <input id="LeaveDate" class="easyui-datebox" style="width: 250px;" data-options="disabled:true," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">劳动合同年限：</td>
                    <td>
                        <input id="ContractPeriod" class="easyui-datebox" style="width: 250px;" data-options="disabled:true," />
                    </td>
                    <%--<td class="lbl">试用期时长(月)：</td>
                    <td>
                        <input id="ProbationMonths" class="easyui-numberbox" style="width: 250px;" data-options="disabled:true,min:0,precision:1"  />
                    </td>--%>
                    <td class="lbl">试用期结束时间：</td>
                    <td>
                        <input id="ProbationMonths" class="easyui-datebox" style="width: 250px;" data-options="required:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">所属公司：</td>
                    <td>
                        <input id="EntryCompany" class="easyui-combobox" style="width: 250px;" data-options="disabled:true," />
                    </td>
                    <td class="lbl">社保账号：</td>
                    <td>
                        <input id="SocialSecurityAccount" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行名称：</td>
                    <td colspan="3">
                        <input id="Bank" class="easyui-combobox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行地址：</td>
                    <td>
                        <input id="BankAddress" class="easyui-textbox" style="width: 250px;" />
                    </td>
                    <td class="lbl">银行账号：</td>
                    <td>
                        <input id="BankAccount" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
            </table>
            <div style="border: none; height: 200px">
                <table id="tab1" title="工资项默认值">
                </table>
            </div>
        </div>
    </div>
</asp:Content>
