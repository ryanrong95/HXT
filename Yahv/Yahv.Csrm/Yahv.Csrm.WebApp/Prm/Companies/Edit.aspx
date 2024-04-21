<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Prm.Companies.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model)) {
                $('#form1').form('load', model);
                $('#txtName').textbox('readonly');
                //addTab('受益人管理', '../Beneficiaries/List.aspx?id=' + model.ID);
                //addTab('发票管理', '../Invoices/List.aspx?id=' + model.ID);
                //addTab('联系人管理', '../Contacts/List.aspx?id=' + model.ID);
                //addTab('到货地址管理', '../Consignees/List.aspx?id=' + model.ID);
            }
        })
        //function addTab(title, url) {
        //    var sender = $('#tt');
        //    if (sender.tabs('exists', title)) {
        //        sender.tabs('select', title);
        //    } else {
        //        var id = '_' + Math.random().toString().substring(2);

        //        var content = '<iframe id="' + id + '" scrolling="auto" frameborder="0"'
        //            + ' src="' + url + '" style="width:100%;height:100%;"></iframe>';

        //        sender.tabs('add', {
        //            title: title,
        //            content: content,
        //            closable: false,
        //            selected: false
        //        });

        //        $('#' + id).parent('div').css({ 'overflow': 'hidden' });
        //    }
        //}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <%--<div style="width: 610px">
            <div style="padding: 10px 60px 20px 60px;">--%>
        <table class="liebiao">
            <tr>
                <td style="width: 120px;">公司名称</td>
                <td>
                    <input id="txtName" name="Name" class="easyui-textbox readonly_style" style="width: 350px;"
                        data-options="prompt:'公司（主体）公司名称,名称要保证全局唯一',required:true,validType:'length[1,75]'">
                </td>
            </tr>
            <tr>
                <td>公司类型</td>
                <td>
                    <select id="selType" name="Type" class="easyui-combobox" runat="server" data-options="editable:false" panelheight="auto" style="width: 350px;"></select>
                </td>
            </tr>
            <tr id="trcode">
                <td>所在地</td>
                <td>
                    <select id="selRange" name="Range" runat="server" class="easyui-combobox" data-options="editable:false" panelheight="auto" style="width: 350px;"></select>
                </td>
            </tr>

            <tr>
                <td>管理员编码</td>
                <td>
                    <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 350px;" data-options="prompt:'自行保障正确性',required:false,validType:'length[1,50]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                    <label for="male">所属国家或地区</label>
                </td>
                <td>
                    <input id="txtDistrict" name="District" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">法人</td>
                <td colspan="3">
                    <input id="txtCorporation" name="Corporation" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">注册地址</td>
                <td colspan="3">
                    <input id="txtRegaddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,150]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">统一社会信用代码</td>
                <td colspan="3">
                    <input id="txtUscc" name="Uscc" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
        </div>
        <%--  </div>
        </div>--%>
    </div>
</asp:Content>
