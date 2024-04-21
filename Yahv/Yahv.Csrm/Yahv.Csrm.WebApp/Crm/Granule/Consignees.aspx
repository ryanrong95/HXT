<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Consignees.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Granule.Consignees" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var isInit = true;
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
                rownumbers: true,
                singleSelect: false,
                fit: true,
                checkOnSelect: false,
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
                    $.messager.confirm('确认', '确认将该地址分配给' + model.Admin.RealName + '吗？', function (r) {
                        if (r) {
                            $.post('?action=Bind', { consigneeid: row.ID, adminid: model.Admin.ID, clientid: model.Client.ID }, function (result) {
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
                        }
                    })
                },
                onUncheck: function (index, row) {
                    if (!row.IsChecked) {
                        return;
                    }
                    $.messager.confirm('确认', '确认取消分配该地址给' + model.Admin.RealName + '吗？', function (r) {
                        if (r) {
                            $.post('?action=UnBind', { consigneeid: row.ID, adminid: model.Admin.ID, clientid: model.Client.ID }, function (result) {
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
                address: $.trim($('#txtaddress').textbox("getText")),
                contactname: $.trim($('#txtcontact').textbox("getText")),
                //tel: $.trim($('#txttel').textbox("getText")),
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
                    <td style="width: 90px;">地址</td>
                    <td>
                        <input id="txtaddress" data-options="prompt:'地址',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">联系人</td>
                    <td>
                        <input id="txtcontact" data-options="prompt:'联系人姓名',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
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
        <table id="dg" data-options="fit:true" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'StatusName',width:50">状态</th>
                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field: 'Admin',width:80">添加人</th>
                    <%
                        }
                    %>
                    <%-- <th data-options="field: 'Btn',formatter:btnformatter,width:130">操作</th>--%>
                    <th data-options="field: 'District',width:50">地区</th>
                    <th data-options="field: 'Address',width:280">地址</th>
                    <th data-options="field: 'Postzip',width:50">邮编</th>
                    <th data-options="field: 'DyjCode',width:80">大赢家编码</th>
                    <th data-options="field: 'ContactName',width:80">联系人</th>
                    <th data-options="field: 'Tel',width:100">电话</th>
                    <th data-options="field: 'Mobile',width:100">手机号</th>

                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
