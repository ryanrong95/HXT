<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="LabourEdit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Staffs.LabourEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .l-btn-icon-left {
            margin-top: -4px !important;
        }
    </style>
    <script>
        var id = getQueryString("ID");
        $(function () {
            $('#EnterpriseID').combobox({
                data: model.CompaniesDate,
            });
            if (model.AllData) {
                $("#EnterpriseID").combobox("setValue", model.AllData.EnterpriseID);
                $("#EntryDate").datebox("setValue", model.AllData.EntryDate);
                $("#LeaveDate").datebox("setValue", model.AllData.LeaveDate);
            }
        });
        function Valid() {
            debugger;
            if (id == "") {
                if (window.parent.tempID == 0) {
                    $.messager.alert('提示', "请先保存员工明细");
                    return false;
                } else {
                    id = window.parent.tempID;
                }
            }

            return true;
        }
        function Save() {
            Valid();
            //验证表单数据
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            debugger;
            data.append('EntryCompany', $('#EnterpriseID').combobox('getText'));
            data.append('ID', id);
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
                        });
                    }
                    else {
                        $.messager.alert('提示', res.message, 'info', function () {
                        });
                    }
                }
            }).done(function (res) {
            });
        }
        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="text-align: left; padding: 5px">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="Save()">提交</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="Close()">关闭</a>
    </div>
    <div class="easyui-panel" title="劳资信息" data-options="fit:true,border:false">
        <div>
            <table class="liebiao">
                <tr>
                    <td>所属公司：</td>
                    <td>
                        <input id="EnterpriseID" name="EnterpriseID" data-options="valueField:'Value',textField:'Text',panelHeight:'160px',required:true" class="easyui-combobox" style="width: 250px" />
                    </td>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td>入职时间：</td>
                    <td>
                        <input class="easyui-datebox" id="EntryDate" name="EntryDate" data-options="editable:false,required:true" style="width: 250px" />
                    </td>
                    <td>离职时间：</td>
                    <td>
                        <input class="easyui-datebox" id="LeaveDate" name="LeaveDate" data-options="editable:false" style="width: 250px" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
