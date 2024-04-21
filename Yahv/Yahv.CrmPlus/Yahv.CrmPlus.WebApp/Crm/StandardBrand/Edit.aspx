<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.StandardBrand.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model.Entity)) {

                $('#form1').form('load', model.Entity);
                $('#Name').text(model.Entity.Name);

            }
            window.grid = $('#admins').myDatagrid({
                pageSize: 50,
                actionName: 'admins',
                toolbar: '#tb',
                nowrap: false,
                fitColumns: true,
                fit: false,
                pagination: false,
            });
            window.grid = $('#suppliers').myDatagrid({
                actionName: 'suppliers',
               pageSize: 50,
               toolbar: '#suppliertb',
                nowrap: false,
                fitColumns: true,
               fit: false,
               pagination: false,
            });
            window.grid = $('#companys').myDatagrid({
                actionName: 'companys',
                pageSize: 50,
                toolbar: '#companytb',
                nowrap: false,
                fitColumns: true,
                fit: false,
                pagination: false,
            });

        });
         function addAdmin()
         {
             $.myDialogFuse({
                 title: "人员新增",
                 url: 'Admins.aspx?brandid=' + model.Entity.ID, onClose: function () {
                   // window.grid.myDatagrid('flush');
                $('#admins').myDatagrid('flush');
                },
                width: "550px",
                height: "400px",
            });
            return false;
        }

        function addCompany() {
            $.myDialogFuse({

                title: "合作公司新增",
                 url: 'Companys.aspx?brandid=' + model.Entity.ID, onClose: function () {
                   // window.grid.myDatagrid('flush');
                $('#companys').myDatagrid('flush');
                },
                width: "550px",
                height: "400px",
            });
        }

           function addSupplier() {
            $.myDialogFuse({

                title: "合作供应商新增",
                 url: 'Suppliers.aspx?brandid=' + model.Entity.ID, onClose: function () {
                   // window.grid.myDatagrid('flush');
                $('#suppliers').myDatagrid('flush');
                },
                width: "550px",
                height: "400px",
            });
        }

         function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnDetails" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="abandonAdmins(\'' + rowData.ID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function abandonAdmins(id) {
            $.messager.confirm('确认', '确定删除吗？', function (r) {
                if (r) {
                    $.post('?action=abandonAdmins', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });

                }
            });
        }

           function abandonCompany(id) {
            $.messager.confirm('确认', '确定删除吗？', function (r) {
                if (r) {
                    $.post('?action=abandonCompany', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                         $('#companys').myDatagrid('flush');
                    });

                }
            });
        }
         function abandonSupplier(id) {
            $.messager.confirm('确认', '确定删除吗？', function (r) {
                if (r) {
                    $.post('?action=abandonSupplier', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                      $('#suppliers').myDatagrid('flush');
                    });

                }
            });
        }

        function btnformatter1(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnDelete" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="abandonCompany(\'' + rowData.nBrandID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function btnformatter2(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnDelete" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="abandonSupplier(\'' + rowData.nBrandID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'north'" style="height: 100px; border: 0px">
            <table class="liebiao">
                <tr>
                    <td>品牌名称</td>
                    <td>
                        <label id="Name" ></label>
                       <%-- <input id="Name" name="Name" class="easyui-textbox" style="width: 250px;" data-options="required:true,missingMessage:'请输入品牌名称'" />--%></td>

                    <td>品牌简称</td>
                    <td>
                        <input id="Code" name="Code" class="easyui-textbox" style="width: 250px;" data-options="required:false,missingMessage:'请输入简称'" /></td>
                </tr>
                <tr>
                    <td>中文名称</td>
                    <td >
                        <input id="ChineseName" name="ChineseName" class="easyui-textbox" style="width: 250px;" data-options="required:false,validType:'length[1,50]'" />
                    </td>
                    <td>是否代理品牌</td>
                    <td>
                        <input id="IsAgent" class="easyui-checkbox" name="IsAgent" /><label for="IsAgent" style="margin-right: 30px">是</label>
                    </td>
                </tr>
            </table>
        </div>
         <div data-options="region:'center'", style="height:300px;">
           <table id="admins" class="easyui-datagrid" style="width: auto; height: auto" data-options="toolbar: '#tb'" title="人员配置">
                <thead>
                    <tr>
                        <th data-options="field:'UserName',width:150">用户名</th>
                        <th data-options="field:'RealName',width:150">真实名</th>
                        <th data-options="field:'RoleName',width:60">角色</th>
                        <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
                    </tr>
                </thead>
            </table>
            <div id="tb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="addAdmin()">新增</a>
            </div>

               <table id="companys" class="easyui-datagrid" style="width: auto; height: auto" data-options="toolbar: '#tb'" title="合作公司">
                <thead>
                    <tr>
                        <th data-options="field:'CompanyName',width:150">合作公司</th>
                        <th data-options="field:'Btn',formatter:btnformatter1,width:150">操作</th>
                    </tr>
                </thead>
            </table>
            <div id="companytb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="addCompany()">新增</a>
            </div>
             <div>
                   <table id="suppliers" class="easyui-datagrid" style="width: auto; height: auto" data-options="toolbar: '#tb'" title="合作供应商">
                <thead>
                    <tr>
                        <th data-options="field:'SupplierName',width:150">合作供应商</th>
                        <th data-options="field:'Btn',formatter:btnformatter2,width:150">操作</th>
                    </tr>
                </thead>
            </table>
            <div id="suppliertb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="addSupplier()">新增</a>
            </div>
             </div>
           

               </div>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>
    </div>
    </div>
</asp:Content>
