<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EasyUI2.ascx.cs" Inherits="WebApp.UC.EasyUI2" %>
<%=Needs.Settings.SettingsManager<Needs.Settings.IErpSrcContext>.Current.Easyui162%>

<script>
    /* 全局 */
    window.gvSettings = {
        menu: '',
        fatherMenu: '',
        summary: ''
    };
    function Valid() {
        var isValid = $("#form1").form("enableValidation").form("validate");
        if (!isValid) {
            $.messager.alert('提示', '请按提示输入数据！');
            return false;
        }
        else {
            return true;
        }
    }
</script>
<script src="/ga.js.aspx?rawurl=<%=Request.Url.AbsolutePath %>"></script>
