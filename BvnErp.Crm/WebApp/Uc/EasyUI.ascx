<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EasyUI.ascx.cs" Inherits="WebApp.UC.EasyUI" %>
<%=Needs.Settings.SettingsManager<Needs.Settings.IErpSrcContext>.Current.Easyui%>

<script>
    /* 全局 */
    window.gvSettings = {
        menu: '',
        fatherMenu: '',
        summary: ''
    };
    //保存校验
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

    var plugins = ["combobox", "textbox", "numberbox", "datetimebox", "datebox"];

    //添加下拉框焦点失去事件
    var event = $.extend({}, $.fn.combo.defaults.inputEvents, {
        blur: function (e) {
            var obj = e.data.target;
            $.map($(obj).combobox("getValues"), function (value) {
                var data = $(obj).combobox("getData");
                var valuefiled = $(obj).combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $(obj).combobox("clear");
                }
            });
        },
    });

    //特殊字符转换
    function escape2Html(str) {
        if (str == null) {
            return str;
        }
        var arrEntities = { 'lt': '<', 'gt': '>', 'nbsp': ' ', 'amp': '&', 'quot': '"', '#39': '\'' };
        return str.replace(/&(lt|gt|nbsp|amp|quot|#39);/ig, function (all, t) { return arrEntities[t]; });
    }
</script>
<script src="/ga.js.aspx?rawurl=<%=Request.Url.AbsolutePath %>"></script>
