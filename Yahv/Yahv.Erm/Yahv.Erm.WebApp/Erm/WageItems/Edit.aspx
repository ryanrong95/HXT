<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.WageItems.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //校验输入是否为汉字
            $("#Name").textbox("textbox").bind('blur', function (data) {
                //var reg = /^[A-Za-z\u4e00-\u9fa5]+$/;
                var reg = /^[a-zA-Z0-9\u4e00-\u9fa5]+$/;
                var text = $("#Name").textbox("getText");
                var value = reg.exec(text);
                $("#Name").textbox("setValue", value);
            });

            //表格初始化
            $("#Type").combobox({
                data: model.Type,
                onChange: function (newValue, oldValue) {
                    if (newValue == 2) {
                        $("#Formula").textbox('enable');
                        $("#CalcOrder").textbox('enable');

                        $("#tr_calc").show();
                    }
                    else if (newValue == 1) {
                        $("#Formula").textbox('disable');
                        $("#Formula").textbox('clear');
                        $("#CalcOrder").textbox('disable');
                        $("#CalcOrder").textbox('clear');

                        $("#tr_calc").hide();
                    }
                }
            });

            if (model.Data) {
                $("form").form("load", model.Data);
                $("#Name").textbox("readonly");
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <div>
            <table class="liebiao">
                <tr>
                    <td>工资项名称</td>
                    <td>
                        <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,75]'" />
                    </td>
                    <td>类型</td>
                    <td>
                        <input id="Type" name="Type" class="easyui-combobox" style="width: 200px;"
                            data-options="required:true,limitToList:true,valueField:'value',textField:'text'" />
                    </td>
                </tr>
                <tr>
                    <td>是否可变更</td>
                    <td>
                        <input id="IsImport" name="IsImport" class="easyui-combobox" style="width: 200px;"
                            data-options="required:true,limitToList:true,valueField:'value',textField:'text',data: model.IsImport," />
                    </td>
                    <td>描述</td>
                    <td>
                        <input id="Description" name="Description" class="easyui-textbox" style="width: 200px; height: 80px;"
                            data-options="prompt:'',validType:'length[1,200]',multiline:true" />
                    </td>
                </tr>
                <tr>
                    <td>录入人</td>
                    <td colspan="3">
                        <input id="InputerId" name="InputerId" class="easyui-combobox" style="width: 200px;"
                            data-options="required:false,limitToList:true,valueField:'value',textField:'text',url:'?action=getAdmins'" />
                    </td>
                </tr>
                <tr id="tr_calc" style="display: none;">
                    <td>公式内容</td>
                    <td colspan="3">
                        <input id="Formula" name="Formula" class="easyui-textbox" style="width: 600px; height: 80px;"
                            data-options="disabled: true,required:true,validType:'length[1,500]',multiline:true" />
                    </td>
                    <%--<td>计算顺序</td>
                    <td>
                        <input id="CalcOrder" name="CalcOrder" class="easyui-numberbox"  style="width: 200px;"
                            data-options="required:true,disabled: true, min: 0," />
                    </td>--%>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />
</asp:Content>
