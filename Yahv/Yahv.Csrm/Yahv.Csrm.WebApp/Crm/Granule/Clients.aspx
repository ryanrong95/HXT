<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Clients.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Granule.Clients" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var isInit = true;
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                checkOnSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                onClickRow: function (index, row) {
                    addTab('到货地址管理', 'Consignees.aspx?id=' + row.ID + '&adminid=' + model.Admin.ID);
                    //addTab('发票管理', 'Invoices.aspx?id=' + row.ID);
                    addTab('联系人管理', 'Contacts.aspx?id=' + row.ID + '&adminid=' + model.Admin.ID);
                },
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
                    $.messager.confirm('确认', '确认将该客户分配给' + model.Admin.RealName + '吗？', function (r) {
                        if (r) {
                            $.post('?action=Bind', { clientid: row.ID, adminid: model.Admin.ID }, function (result) {
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
                        }
                        else {
                            $("#dg").datagrid("checkRow", index);
                            row.IsChecked = true;
                        }
                    })
                },
                onUncheck: function (index, row) {
                    if (!row.IsChecked) {
                        return;
                    }
                    $.messager.confirm('确认', '确认取消分配该客户给' + model.Admin.ID + '吗？', function (r) {
                        if (r) {
                            $.post('?action=UnBind', { clientid: row.ID, adminid: model.Admin.ID }, function (result) {
                                row.IsChecked = false;
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
                        }
                        else {
                            $("#dg").datagrid("uncheckRow", index);
                            row.IsChecked = false;
                        }
                    })
                },
            });
            //var sender = $('#tt');
            //sender.tabs({
            //    onSelect: function (title, index) {
            //        var id = tabs[index + 1].id;
            //        var src = $('#' + id).prop('src');
            //        var url = tabs[index + 1].src;
            //        // 判断 src 是否有值？ 如果为真，就跳出。否则，就做如下操作
            //        if (url != src) {
            //            $('#' + id).attr('src', url);
            //        }
            //    }
            //});
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        })

    </script>
    <script>
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function showEditPage(id) {
            $.myWindow({
                title: "编辑客户信息",
                url: '../Clients/Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "50%",
            });
            return false;
        }


        function client_formatter(value, rec) {
            var result = "";
            if (rec.Vip == -1) {
                result += "<span class='vip'></span>";
            }
            else if (rec.Vip > 0) {
                result += '<span class="vip' + rec.Vip + '"></span>';
            }
            result += rec.Name
            result += '<span class="level' + rec.Grade + '"></span>';
            return result;
        }
        var tabs = [{}];
        function addTab(title, url) {
            var sender = $("#tt");
            if (sender.tabs('exists', title)) {
                sender.tabs("select", title);
                var selTab = sender.tabs('getTab', title);
                sender.tabs('update', {
                    tab: selTab,
                    options: {
                        content: createFrame(url)
                    }
                })
            } else {
                sender.tabs('add', {
                    title: title,
                    content: createFrame(url),
                    border: false,
                    bodyCls: "indexTabBody",
                    closable: true
                });
            }
        }

        function createFrame(url) {
            var id = '_' + Math.random().toString().substring(2);
            tabs.push({
                id: id,
                src: url,
            });
            return '<iframe id="' + id + '" scrolling="auto" frameborder="0"'
                          + ' src="' + url + '" style="width:100%;height:100%;"></iframe>';
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="region:'center',title:'',split:false" style="width: 500px">
        <!--工具-->
        <div id="tb">
            <div>
                <table class="liebiao-compact">
                    <tr>
                        <td style="width: 90px;">客户名称</td>
                        <td>
                            <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                            <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- 表格 -->
        <table id="dg" style="width: 50%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <%-- <th data-options="field:'ID',width:100">ID</th>--%>
                    <th data-options="field: 'Name',width:300,formatter:client_formatter">名称</th>
                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field: 'Admin',width:80">添加人</th>
                    <%
                        }
                    %>
                    <th data-options="field: 'Nature',width:70">客户性质</th>
                    <th data-options="field: 'Type',width:70">客户类型</th>
                    <%--<th data-options="field: 'Grade',width:50">等级</th>--%>
                    <th data-options="field: 'DyjCode',width:120">大赢家编码</th>
                    <th data-options="field: 'TaxperNumber',width:120">纳税人识别号</th>
                    <%--<th data-options="field: 'Vip',width:30">vip</th>--%>
                    <th data-options="field: 'StatusName',width:80">状态</th>
                    <%-- <th data-options="field: 'Btn',formatter:btnformatter,width:150">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>

    <div data-options="region:'east',title:'',split:false" style="width: 50%">
        <div class="easyui-tabs" id="tt" data-options="fit:true">
        </div>
    </div>
</asp:Content>

