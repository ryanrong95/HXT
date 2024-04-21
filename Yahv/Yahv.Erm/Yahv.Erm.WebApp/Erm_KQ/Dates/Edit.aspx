<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Dates.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            //安排方式
            $('#Method').combobox({
                data: model.Methods,
                textField: 'text',
                valueField: 'value',
            });

            //名称
            $('#Name').combobox({
                data: model.Names,
                textField: 'text',
                valueField: 'value',
            });

            //来源
            $('#From').combobox({
                data: model.Froms,
                textField: 'text',
                valueField: 'value',
            });

            //区域
            $('#RegionID').combobox({
                data: model.Regions,
                textField: 'text',
                valueField: 'value',
            });

            //所属班别
            $('#ShiftID').combobox({
                data: model.Schedulings,
                textField: 'text',
                valueField: 'value',
            });

            //实际班别
            $('#SchedulingID').combobox({
                data: model.Schedulings,
                textField: 'text',
                valueField: 'value',
            });

            if (model.Data) {
                $("form").form("load", model.Data);

                $('#Date').textbox('setValue', model.DateString);
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td>日期</td>
                <td>
                    <input id="Date" name="Date" class="easyui-textbox" style="width: 200px;" data-options="disabled:true" />
                </td>
                <td>名称描述</td>
                <td>
                    <input id="Name" name="Name" class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>日期类型</td>
                <td>
                    <input id="Method" name="Method" class="easyui-combobox" style="width: 200px;" />
                </td>
                <td>来源</td>
                <td>
                    <input id="From" name="From" class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>区域</td>
                <td>
                    <input id="RegionID" name="RegionID" class="easyui-combobox" style="width: 200px;" data-options="disabled:true" />
                </td>
                <td>薪酬倍数</td>
                <td>
                    <input id="SalaryMultiple" name="SalaryMultiple" class="easyui-numberbox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>所属班别</td>
                <td>
                    <input id="ShiftID" name="ShiftID" class="easyui-combobox" style="width: 200px;" data-options="disabled:true" />
                </td>
                <td>实际班别</td>
                <td>
                    <input id="SchedulingID" name="SchedulingID" class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClick="btnSave_Click" />
        </div>
    </div>
</asp:Content>
