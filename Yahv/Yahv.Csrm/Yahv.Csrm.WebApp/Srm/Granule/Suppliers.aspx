<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Suppliers.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Granule.Suppliers" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .yc .textbox-label {
            width: 30px;
        }
    </style>
    <script>
        $(function () {
            var isInit = true;
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                    selGrade: $('#selGrade').combobox("getValue"),
                    factory: $("#chb_factory").checkbox('options')['checked']
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                checkOnSelect: false,
                rownumbers: true,
                fit: true,
                method: 'get',
                queryParams: getQuery(),
                onClickRow: function (index, row) {
                    addTab('受益人', 'Beneficiaries.aspx?id=' + row.ID + '&adminid=' + model.Admin.ID);
                    //addTab('发票管理', 'Invoices.aspx?id=' + row.ID + '&adminid=' + model.Admin.ID);
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
                    $.messager.confirm('确认', '确认将该供应商分配给' + model.Admin.RealName + '吗？', function (r) {
                        if (r) {
                            $.post('?action=Bind', { supplierid: row.ID, adminid: model.Admin.ID }, function (result) {
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
                        }
                    })
                },
                onUncheck: function (index, row) {
                    if (!row.IsChecked) {
                        return;
                    }
                    $.messager.confirm('确认', '确认取消分配该供应商给' + model.Admin.ID + '吗？', function (r) {
                        if (r) {
                            $.post('?action=UnBind', { supplierid: row.ID, adminid: model.Admin.ID }, function (result) {
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
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
            $('#selGrade').combobox({
                data: model.Grade,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
            function StatusCheck(status) {
                return status == '<%=ApprovalStatus.Waitting%>';
            }
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
                title: "供应商基本信息",
                url: '../Suppliers/Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "50%",
            });
            return false;
        }
        function supplier_formatter(value, rec) {
            var result = rec.Name
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
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">供应商名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">等级</td>
                    <td>
                        <select id="selGrade" name="selGrade" class="easyui-combobox" data-options="editable:false,panelheight:'auto',isKeydown:true"></select>
                    </td>
                    <td class="yc">
                        <input id="chb_factory" class="easyui-checkbox" name="chb_factory" label="原厂" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    </td>
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <table id="dg" style="width: 50%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <%-- <th data-options="field:'ID',width:100">ID</th>--%>
                    <th data-options="field: 'Name',width:240,formatter:supplier_formatter">名称</th>
                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field: 'Admin',width:80">添加人</th>
                    <%
                        }
                    %>
                    <th data-options="field: 'Nature',width:80">性质</th>
                    <th data-options="field: 'Type',width:50">类型</th>
                    <%-- <th data-options="field: 'AreaType',width:80">所在地区</th>--%>
                    <th data-options="field: 'DyjCode',width:80">大赢家编码</th>
                    <th data-options="field: 'TaxperNumber',width:80">纳税人识别号</th>
                    <th data-options="field: 'InvoiceType',width:80">发票</th>
                    <th data-options="field: 'IsFactory',width:30">原厂</th>
                    <th data-options="field: 'AgentCompany',width:200">代理公司</th>
                    <th data-options="field: 'StatusName',width:50">状态</th>
                    <th data-options="field: 'RepayCycle',width:50">账期/天</th>
                    <th data-options="field: 'Price',width:120">额度</th>
                    <%--<th data-options="field: 'Btn',formatter:btnformatter,width:150">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>

    <div data-options="region:'east',title:'',split:false" style="width: 50%">
        <div class="easyui-tabs" id="tt" data-options="fit:true">
        </div>
    </div>
</asp:Content>
