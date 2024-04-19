<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EasyUI.ascx.cs" Inherits="WebApp.UC.EasyUI" %>


<%=Needs.Settings.SettingsManager<Needs.Settings.IErpSrcContext>.Current.Easyui%>

<script>
    /* 全局 */
    window.gvSettings = {
        menu: '',
        fatherMenu: '',
        summary: ''
    };

</script>
<script src="/ga.js.aspx?rawurl=<%=Request.Url.AbsolutePath %>"></script>

