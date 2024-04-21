<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.PersonalRates.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Data) {
                $("form").form("load", model.Data);
            }
        });
        function Valid() {
            debugger;
            var uPattern = /(^[\-0-9][0-9]*(.[0-9]+)?)$/;
            var re = new RegExp(uPattern);
            var b = $("#BeginAmount").textbox("getValue");
            var e = $("#EndAmount").textbox("getValue");
            var r = $("#Rate").textbox("getValue");
            var d = $("#Deduction").textbox("getValue");
            if (b.search(re) == -1 || e.search(re) == -1 || r.search(re) == -1 || d.search(re) == -1) {
                $.messager.alert('提示', "请输入正确的数值");
                return false;
            }

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <div>
            <table class="liebiao">
                <tr>
                    <td>级数</td>
                    <td>
                        <input id="Levels" name="Levels" class="easyui-numberbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,10]'" />
                    </td>
                    <td>起始金额</td>
                    <td>
                        <input id="BeginAmount" name="BeginAmount" class="easyui-textbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,25]'" />
                    </td>
                </tr>
                <tr>
                    <td>结束金额</td>
                    <td>
                        <input id="EndAmount" name="EndAmount" class="easyui-textbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,25]'" />
                    </td>
                    <td>预扣率（%）</td>
                    <td>
                        <input id="Rate" name="Rate" class="easyui-numberbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,25]'" />
                    </td>
                </tr>
                <tr>
                    <td>速算扣除数</td>
                    <td>
                        <input id="Deduction" name="Deduction" class="easyui-textbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,25]'" />
                    </td>
                    <td>描述</td>
                    <td>
                        <input id="Description" name="Description" class="easyui-textbox" style="width: 200px; height: 80px;"
                            data-options="prompt:'',validType:'length[1,200]',multiline:true" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClientClick="return Valid()" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
</asp:Content>
