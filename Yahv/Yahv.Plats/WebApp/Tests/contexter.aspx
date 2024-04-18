<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="contexter.aspx.cs" Inherits="WebApp.Tests.contexter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div style="text-align: center; padding: 5px">
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$('#btnSubmit').click();">Submit</a>
        <a href="javascript:void(0)" class="easyui-linkbutton">Clear</a>
        <asp:Button ID="btnSubmit" ClientIDMode="Static" runat="server" Text="111" OnClick="btnSubmit_Click" OnClientClick="alert('ok:click!');" Style="display: none;" />
    </div>

</asp:Content>
