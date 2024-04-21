<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.StandardBrand.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //if (!jQuery.isEmptyObject(model.Entity)) {

            //    $('#form1').form('load', model.Entity);
            //    $('#Name').textbox('textbox').attr('disabled', true);

            //}

        });

              //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnDelete" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="showEditPage(\'' + rowData.ID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'north'" style="height: 253px; border: 0px">
            <table class="liebiao">
                <tr>
               <td>品牌名称</td>
               <td colspan="3"><input id="Name" name="Name" class="easyui-textbox" style="width: 350px;" data-options="required:true,missingMessage:'请输入品牌名称'" /></td>
               
           </tr>
            <tr>
                <td>品牌简称</td>
                <td><input id="Code" name="Code" class="easyui-textbox" style="width: 350px;" data-options="required:false,missingMessage:'请输入简称'" /></td>
            </tr>
            <tr>
                <td>中文名称</td>
               <td colspan="3">
                   <input id="ChineseName" name="ChineseName" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'" />
               </td>
              
            </tr>
                <tr>
                    <td>是否代理品牌</td>
                    <td colspan="3">
                        <input id="IsAgent" class="easyui-checkbox" name="IsAgent" /><label for="IsAgent" style="margin-right: 30px">是</label>
                    </td>
                </tr>
            </table>
        </div>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>
    </div>
    </div>
</asp:Content>
