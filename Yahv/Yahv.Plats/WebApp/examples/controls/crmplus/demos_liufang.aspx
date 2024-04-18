<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="demos_liufang.aspx.cs" Inherits="WebApp.examples.controls.crmplus.demos_liufang" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <title>企业联系人插件 - jQuery EasyUI plugin</title>
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />

    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />



    <link href="http://fixed2.b1b.com/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/customs-easyui/Styles/main.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/customs-easyui/Styles/reset.css" rel="stylesheet" />

    <link href="http://fixed2.b1b.com/Yahv/customs-easyui/fonts/iconfont.css" rel="stylesheet" />

    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/customs-easyui/Scripts/main.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/ajaxPrexUrl.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/crmPlus/easyui.sfs.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/customs-easyui/Scripts/easyui.myDialog.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/customs-easyui/Scripts/easyui.myWindow.fuse.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/crmPlus/easyui.sfs.static.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/customs-easyui/Scripts/easyui.myDialog.fuse.js"></script>

    <script>

</script>
</head>

<body>
    <form id="form1" runat="server">
        <div class="easyui-panel" data-options="title:'企业联系人插件'" style="width: 100%; padding: 3px 6px;">
            <input id="contact" name="contact" style="width: 200px" /><br />
        </div>
        <div class="easyui-panel" data-options="title:'企业收件地址插件'" style="width: 100%; padding: 3px 6px;">
            <input id="consignee" name="consignee" style="width: 200px" /><br />
        </div>
        <div class="easyui-panel" data-options="title:'标准型号插件'" style="width: 100%; padding: 3px 6px;">
            <input id="standardPartNumber" name="standardPartNumber" style="width: 200px" /><br />
        </div>
        <div class="easyui-panel" data-options="title:'内部公司插件'" style="width: 100%; padding: 3px 6px;">
            <input id="company" name="company" style="width: 200px" /><br />
        </div>
        <div class="easyui-panel" data-options="title:'供应商插件'" style="width: 100%; padding: 3px 6px;">
            <input id="supplier" name="supplier" style="width: 200px" /><br />
        </div>

        <hr />

        <div class="easyui-panel" data-options="title:'客户插件'" style="width: 100%; padding: 3px 6px;">
            <input id="client" name="client" style="width: 200px" /><br />
        </div>
        <div class="easyui-MuliArea" id="mm3"></div>

        <div class="easyui-panel" data-options="title:'联动地区'" style="width: 100%; padding: 3px 6px;">
            <div class="easyui-ChinaArea" id="chinaarea"></div>
        </div>

        <%--<div class="easyui-panel" data-options="title:'阅读人多选'" style="width: 100%; padding: 3px 6px;">
            <input id="admin" name="admin" style="width: 350px" /><br />
        </div>--%>
        <%--<div class="easyui-panel" style="width: 100%; max-width: 400px; padding: 30px 60px;">
            <input id="tagbox" name="tagbox" class="easyui-tagbox" style="width: 200px">
        </div>--%>
        <div>
            <%-- <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="$('#btnSubmit').click();return false;">提交</a>--%>
            <asp:Button ID="btnSubmit" runat="server" Text="保存" OnClick="btnSubmit_Click" Style="display: none;" />
        </div>
        <button onclick="setEnterpriseID()">点击setEnterpriseID</button><br />
        <button onclick="setValue()">点击setValue</button>
    </form>



    <script>
        $(function () {

            $("#contact").contactCrmPlus({
                required: true,
                isAdd: true,
            });
            $("#consignee").consigneeCrmPlus({
                required: true,
            });
            $("#standardPartNumber").standardPartNumberCrmPlus({
                required: true,
            });
            $("#company").companyCrmPlus({
                required: true,
                onChange: function (newValue, oldValue) {
                    console.log('companyCrmPlus.onChange:' + [newValue, oldValue]);
                }
            });
            $("#supplier").supplierCrmPlus({
                required: true,
                onChange: function (newValue, oldValue) {
                    console.log('supplierCrmPlus.onChange:' + [newValue, oldValue]);
                }
            });
            $("#client").clientCrmPlus({
                required: true,
                onChange: function (newValue, oldValue) {
                    alert('client.onChange:' + [newValue, oldValue]);
                }
            });
            $("#chinaarea").ChinaArea({
                required: false,
                value: ["山西", "吕梁", "孝义市"]
            });
            $('#mm3').MuliArea({
                value: ["中国", "山西", "吕梁", "孝义市", "1234"]
            });
            $("#admin").tagboxCrmPlus({
                required: false,
               
            });
            $("#tagbox").tagbox({
                mode: 'remote',
                method: 'get',
                valueField: 'ID',
                textField: 'Name',
                limitToList: true,
                prompt: 'Select a Language',
               // url:'tagbox.json'
                url: 'http://hv.erp.b1b.com//crmplusapi/Admins/AdminLists',

            });
        })
        function setEnterpriseID() {
            $("#contact").contactCrmPlus('loadSetValue', { enterpriseID: 'Ep20210115025', contactID: 'Ct2021022300003' })
            //$("#contact").contactCrmPlus('setValue', 'Ct2021022300001')
            //$("#contact").contactCrmPlus('setContactID', 'Ep20210115025')
            <%--$("#consignee").consigneeCrmPlus('setEnterpriseID', 'Ep20210115025')--%>
            $("#consignee").consigneeCrmPlus('loadSetValue', { enterpriseID: 'Ep20210115025', consigneeID: 'Ar2021022300001' })
            $("#company").companyCrmPlus('setValue', 'Ep20210115002')
            $("#supplier").companyCrmPlus('setValue', 'Ep20210209026')
            $("#standardPartNumber").standardPartNumberCrmPlus('setValue', 'Spn20210115002')
        }
        function setValue() {
            $("#contact").contactCrmPlus('setValue', 'Ct20210220000011')
            $("#consignee").consigneeCrmPlus('setValue', 'Ar2021012800002')
        }
    </script>
</body>
</html>
