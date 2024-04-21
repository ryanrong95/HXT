<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Contacts.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Granule.Contacts" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var isInit = true;
            $('#selStatus').combobox({
                data: model.Status,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            $('#selContactType').combobox({
                data: model.ContactType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            var result = "";
            if (model.Client == null) {
                result += "<span style='color:red'>请从左侧客户列表选择客户</span>"
            }
            else {
                if (model.Client.Vip) {
                    result += "<span class='vip'></span>";
                }
                result += "当前客户：" + model.Client.Enterprise.Name
                result += '<span class="level' + model.Client.Grade + '"></span>';
            }
            $("#currentClient").html(result)
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                checkOnSelect: false,
                rownumbers: true,
                fit: true,
                queryParams: getQuery(),
                onLoadSuccess: function (data) {
                    data.rows.map(function (element, index) {
                        if (element.IsChecked) {
                            $("#dg").datagrid("checkRow", index);
                        }
                    });
                    isInit = false;
                },
                onCheck: function (index, row) {
                    if (row.IsChecked) {
                        return;
                    }
                    $.messager.confirm('确认', '确认将联系人分配给' + model.Admin.RealName + '吗？', function (r) {
                        if (r) {
                            $.post('?action=Bind', { contactid: row.ID, adminid: model.Admin.ID, clientid: model.Client.ID }, function (result) {
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
                        }
                    })
                },
                onUncheck: function (index, row) {
                    if (!row.IsChecked) {
                        return;
                    }
                    $.messager.confirm('确认', '确认取消分配该联系人给' + model.Admin.RealName + '吗？', function (r) {
                        if (r) {
                            $.post('?action=UnBind', { contactid: row.ID, adminid: model.Admin.ID, clientid: model.Client.ID }, function (result) {
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
                        }
                    })
                },
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            $("#btnClear").click(function () {
                location.reload();
                return false;
            })
        })
        var getQuery = function () {
            var params = {
                action: 'data',
                name: $.trim($('#txtName').textbox("getText")),
                mobile: $.trim($('#txtMobile').textbox("getText")),
                //tel: $.trim($('#txtTel').textbox("getText")),
                //email: $.trim($('#txtEmail').textbox("getText")),
                //status: $('#selStatus').combobox("getValue"),
                type: $('#selContactType').combobox("getValue"),
            };
            return params;
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <div id="currentClient" style="padding: 5px"></div>
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">姓名</td>
                    <td>
                        <input id="txtName" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">手机号/电话</td>
                    <td>
                        <input id="txtMobile" data-options="prompt:'手机号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 90px;">类型</td>
                    <td colspan="3">
                        <select id="selContactType" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    </td>
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <table id="dg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'StatusName',width:80">状态</th>
                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field: 'Admin',width:80">添加人</th>
                    <%
                        }
                    %>
                    <%--<th data-options="field: 'Btn',formatter:btnformatter,width:130">操作</th>--%>
                    <th data-options="field: 'Name',width:100">姓名</th>
                    <th data-options="field: 'Type',width:80">类型</th>
                    <th data-options="field: 'Mobile',width:120">手机号</th>
                    <th data-options="field: 'Tel',width:120">电话</th>
                    <th data-options="field: 'Email',width:150">邮箱</th>

                </tr>
            </thead>
        </table>
    </div>
</asp:Content>



