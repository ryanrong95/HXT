<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.WmsAdmins.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //区域树的初始化
            $('#wareHouse').tree({
                url: '?action=tree&id=' + (model ? model.ID : ''),
                checkbox: true
            });
        });


        //保存
        function Save() {
            var array = $("#wareHouse").tree("getChecked");
            var ids = array.map(function (element, index) {
                if (element.attributes != null && element.attributes != 'undefined') {
                    return  element.attributes.fatherId + "|" + element.id;
                }
            }).join(',');
           
            $("#hidids").val(ids);
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <ul id="wareHouse" class="easyui-tree" data-options="method:'get'"></ul>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="return Save();" OnClick="btnSubmit_Click" />
        </div>
    </div>
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />
    <input type="hidden" runat="server" id="hidids" />
</asp:Content>
