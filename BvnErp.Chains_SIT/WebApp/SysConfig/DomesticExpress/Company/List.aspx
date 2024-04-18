<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.SysConfig.DomesticExpress.Company.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
<uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>
        gvSettings.fatherMenu = '系统配置（xdt）';
        gvSettings.menu = '国内快递';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //银行列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
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
            var name = $('#Name').textbox('getValue');
            var Code = $('#Code').textbox('getValue');
            $('#datagrid').myDatagrid('search', { Name: name ,Code:Code });
        }

        //重置查询条件
        function Reset() {
            $('#Name').textbox('setValue', null);
            $('#Code').textbox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';           
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetType(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">设置快递方式</span>' +
                '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }


        //编辑
        function Edit(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '编辑快递公司',
                    width: '450',
                    height: '320',
                    url: url,
                    onClose: function () {
                        Search();
                    }
                });
            }
        }

        //设置快递方式
        function SetType(id) {
            var url = location.pathname.replace(/Company\/List.aspx/ig, 'Type/List.aspx')+ "?ID=" + id;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '设置快递方式',
                width: '1000px',
                height: '550px'
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">承运商名称: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <span class="lbl">承运商简称: </span>
                    <input class="easyui-textbox search" id="Code" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" data-options="singleSelect:true,fit:true,scrollbarSize:0,border:false" title="快递公司" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'center'" style="width: 100px;">承运商名称</th>
                    <th data-options="field:'Code',align:'left'" style="width: 80px;">承运商简称</th>
                    <th data-options="field:'CustomerName',align:'left'" style="width: 100px;">账号名称</th>
                    <th data-options="field:'CustomerPwd',align:'center'" style="width: 100px;">账号密码</th>
                    <th data-options="field:'MonthCode',align:'left'" style="width: 80px;">月结账号</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 80px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
