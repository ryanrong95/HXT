<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="BankcardEdit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Staffs.BankcardEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .l-btn-icon-left {
            margin-top: -4px !important;
        }
    </style>
    <script>
        var id = getQueryString("ID");
        $(function () {
            $('#Bank').combobox({
                data: model.BankDate,
            });
            if (model.AllData) {
                $("#Bank").combobox("setValue", model.AllData.Bank);
                $("#Account").textbox("setValue", model.AllData.Account);
            }
        });
        function Valid() {
            if (id == "") {
                if (window.parent.tempID == 0) {
                    $.messager.alert('提示', "请先保存员工明细");
                    return false;
                } else {
                    id = window.parent.tempID;
                }
            }
        }
        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
        function Save() {
            Valid();
            //验证表单数据
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            debugger;
            data.append('ID', id)
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
                    } else {
                        $.messager.alert('提示', res.message, 'info', function () {
                        });
                    }
                }
            }).done(function (res) {
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="text-align: left; padding: 5px">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="Save()">提交</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="Close()">关闭</a>
    </div>
    <div class="easyui-panel" title="银行卡信息" data-options="fit:true,border:false">
        <div>
            <table class="liebiao">
                <tr>
                    <td style="width:250px">所属银行：</td>
                    <td>
                        <input id="Bank" name="Bank" data-options="valueField:'Value',textField:'Text',panelHeight:'160px',required:true" class="easyui-combobox" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td style="width:250px">银行卡号：</td>
                    <td>
                        <input id="Account" name="Account" class="easyui-textbox" style="width: 250px;"
                            data-options="prompt:'',required:true,validType:'length[1,50]'" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
