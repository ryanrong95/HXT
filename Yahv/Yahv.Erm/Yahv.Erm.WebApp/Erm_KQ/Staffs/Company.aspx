<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Company.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.Company" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var StaffID = getQueryString("ID");
        $(function () {
            $("#StaffStatus").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.Status,
            })
            $("#WorkCity").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.CityData,
            })
            $("#Postion").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.PostionData,
            })
            $("#WorkingClass").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.SchedulingData,
            })
            $("#Region").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.RegionData,
            })
            $("#DepartmentType").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
            })
            $("#PostType").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.PostType,
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
                data.append('StaffCode', $("#StaffCode").textbox("getValue"));
                data.append('StaffStatus', $("#StaffStatus").combobox("getValue"));
                data.append('UserName', $("#UserName").textbox("getValue"));
                data.append('Password', $("#Password").textbox("getValue"));
                data.append('SelCode', $("#SelCode").textbox("getValue"));
                data.append('WorkCity', $("#WorkCity").combobox("getValue"));
                data.append('Postion', $("#Postion").combobox("getValue"));
                data.append('DepartmentType', $("#DepartmentType").combobox("getValue"));
                data.append('PostType', $("#PostType").combobox("getValue"));
                data.append('WorkingClass', $("#WorkingClass").combobox("getValue"));
                data.append('Region', $("#Region").combobox("getValue"));

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
                        }
                    }
                })
            })
            //初始化
            Init();
        });
    </script>
    <script>
        //初始化
        function Init() {
            //$("#CompanyName").textbox("setValue", model.StaffData.CompanyName);
            $("#CompanyName").textbox("setValue", "深圳市芯达通供应链管理有限公司");
            $("#StaffCode").textbox("setValue", model.StaffData.Code);
            $("#StaffStatus").combobox("setValue", model.StaffData.Status);
            $("#UserName").textbox("setValue", model.StaffData.UserName);
            $("#Password").passwordbox("setValue", model.StaffData.Password);
            $("#SelCode").textbox("setValue", model.StaffData.SelCode);
            $("#WorkCity").combobox("setValue", model.StaffData.WorkCity);
            $("#Postion").combobox("setValue", model.StaffData.PostionID);
            $("#WorkingClass").combobox("setValue", model.StaffData.WorkClassID);
            $("#Region").combobox("setValue", model.StaffData.RegionID);
            if (model.StaffData.DepartmentCode != null) {
                $("#DepartmentType").combobox("setValue", Number(model.StaffData.DepartmentCode));
            }
            if (model.StaffData.PostionCode != null) {
                $("#PostType").combobox("setValue", Number(model.StaffData.PostionCode));
            }
            $("#Password").passwordbox("hidePassword");
            $("#YearVocation").numberbox("setValue", model.VocationData.YearsDay);
            $("#OffVocation").numberbox("setValue", model.VocationData.OffDay);
            $("#SickVocation").numberbox("setValue", model.VocationData.SickDay);
            $("#AntepartumVocation").numberbox("setValue", model.VocationData.ProductionInspectionDay);
        }
    </script>
    <style>
        .title {
            background-color: #F5F5F5;
            color: royalblue;
            font-weight: 600;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'north',height:40">
            <div style="float: left; margin-left: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-save">保存</a>
            </div>
        </div>
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">公司名称：</td>
                    <td>
                        <input id="CompanyName" class="easyui-textbox" style="width: 250px;" data-options="disabled:true" />
                    </td>
                    <td class="lbl">入职状态：</td>
                    <td>
                        <input id="StaffStatus" class="easyui-combobox" style="width: 250px;" data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">员工工号：</td>
                    <td>
                        <input id="SelCode" class="easyui-textbox" style="width: 250px;"/>
                    </td>
                    <td class="lbl">员工编码：</td>
                    <td>
                        <input id="StaffCode" class="easyui-textbox" style="width: 250px;" data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">用户名：</td>
                    <td>
                        <input id="UserName" class="easyui-textbox" style="width: 250px;" data-options="disabled:true" />
                    </td>
                    <td class="lbl">用户密码：</td>
                    <td>
                        <input id="Password" class="easyui-passwordbox" style="width: 250px;" data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">所在区域：</td>
                    <td>
                        <input id="Region" class="easyui-combobox" style="width: 250px;" />
                    </td>
                    <td class="lbl">所在城市：</td>
                    <td>
                        <input id="WorkCity" class="easyui-combobox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">部门名称：</td>
                    <td>
                        <input id="DepartmentType" class="easyui-combobox" style="width: 250px;" />
                    </td>
                    <td class="lbl">职务类型：</td>
                    <td>
                        <input id="PostType" class="easyui-combobox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">岗位名称：</td>
                    <td>
                        <input id="Postion" class="easyui-combobox" style="width: 250px;" />
                    </td>
                    <td class="lbl">班别设置：</td>
                    <td colspan="3">
                        <input id="WorkingClass" class="easyui-combobox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="title" colspan="4">我的假期情况</td>
                </tr>
                <tr>
                    <td class="lbl">带薪年假：</td>
                    <td>
                        <input id="YearVocation" class="easyui-numberbox" style="width: 250px;" data-options="disabled:true,precision:1" />
                    </td>
                    <td class="lbl">调休假期：</td>
                    <td>
                        <input id="OffVocation" class="easyui-numberbox" style="width: 250px;" data-options="disabled:true,precision:1" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">剩余病假：</td>
                    <td>
                        <input id="SickVocation" class="easyui-numberbox" style="width: 250px;" data-options="disabled:true,precision:1" />
                    </td>
                    <td class="lbl">产检假期：</td>
                    <td>
                        <input id="AntepartumVocation" class="easyui-numberbox" style="width: 250px;" data-options="disabled:true,precision:1" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
