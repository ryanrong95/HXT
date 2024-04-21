<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works_ns.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.RoleComposes.Edit" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: true,
                pagination: false,
                singleSelect: false,
                onLoadSuccess: function (data) {
                    if (model && data) {
                        $.each(data.rows, function (index, value) {
                            if (model.Roles.indexOf(value.ID) >= 0)
                                $("#tab1").datagrid("checkRow", index);
                        });
                    }
                }
            });

            if (model) {
                if (model.Name) {
                    $('#txtName').textbox('readonly', true);
                }

                $('form').form('load', model);
            }


            $('#btnSubmit').click(function () {
                var arry = $('#tab1').datagrid('getChecked');

                if (!arry || arry.length <= 0) {
                    $.messager.alert('提示', '请您选择合成的角色!');
                    return false;
                }

                var ids = arry.map(function (element, index) {
                    return element.ID;
                }).join(',');;

                $("#hCheckId").val(ids);
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphFormNorth" runat="server">
    <div class="easyui-panel" data-options="fit:true,border:false">
        <table class="liebiao">
            <tr>
                <td>名称</td>
                <td>
                    <input id="txtName" name="Name" style="width: 200px;" class="easyui-textbox"
                        data-options="prompt:'',required:true,validType:'length[1,75]',isKeydown:true" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphFormCenter" runat="server">
    <table id="tab1" title="角色列表">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true">选择</th>
                <th data-options="field:'ID',width:80">ID</th>
                <th data-options="field:'Name',width:150">名称</th>
                <th data-options="field:'CreateDate',width:120">创建日期</th>
                <th data-options="field:'StatusName',width:80">状态</th>
            </tr>
        </thead>
    </table>
    <input type="hidden" runat="server" id="hCheckId" />
    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
</asp:Content>
