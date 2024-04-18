<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditingList.aspx.cs" Inherits="WebApp.Client.ProductTaxCategory.AuditingList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待审核产品税号</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
<%--    <script>
        gvSettings.fatherMenu = '客户自定义税号(XDT)';
        gvSettings.menu = '待审核';
        gvSettings.summary = '跟单员待审核客户自定义的产品税号';
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //自定义产品税号列表初始化
            $('#taxCategories').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //查询
        function Search() {
            var clientCode = $('#ClientCode').textbox('getValue');
            var name = $('#Name').textbox('getValue');
            var parm = {
                ClientCode: clientCode,
                Name: name
            };
            $('#taxCategories').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#ClientCode').textbox('setValue', null);
            $('#Name').textbox('setValue', null);
            Search();
        }

        //审核通过
        function Approve(ID) {
            $.messager.confirm('确认', '同意客户自定义的产品税号？', function (success) {
                if (success) {
                    $.post('?action=Approve', { ID: ID }, function () {
                        $.messager.alert('审核', '审核成功！');
                        $('#taxCategories').datagrid('reload');
                    })
                }
            });
        }

        ///审核未通过
        function Reject(ID) {
            $.messager.confirm('确认', '拒绝客户自定义的产品税号？', function (success) {
                if (success) {
                    $.post('?action=Reject', { ID: ID }, function () {
                        $.messager.alert('审核', '审核成功！');
                        $('#taxCategories').datagrid('reload');
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Approve(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">同意</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Reject(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">拒绝</span>' +
                '<span class="l-btn-icon icon-no">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
    <style type="text/css">
        .datagrid-header-row,
        .datagrid-row {
            height: 30px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <span class="lbl">品名: </span>
                    <input class="easyui-textbox" id="Name" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="taxCategories" data-options="nowrap:false,border:false,fitColumns:true,fit:true,toolbar:'#topBar'" title="待审核产品税号列表">
            <thead>
                <tr>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 8%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'Name',align:'left'" style="width: 15%;">品名</th>
                    <th data-options="field:'Model',align:'left'" style="width: 10%;">型号</th>
                    <th data-options="field:'TaxCode',align:'left'" style="width: 15%;">税务编码</th>
                    <th data-options="field:'TaxName',align:'left'" style="width: 15%;">税务名称</th>
                    <th data-options="field:'TaxStatus',align:'center'" style="width: 7%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation,fitColumns:true" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

