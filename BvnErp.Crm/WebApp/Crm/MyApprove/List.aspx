<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.MyApprove.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM系统管理';
        gvSettings.menu = '我的审批';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">
        var Status = eval('(<%=this.Model.Status%>)');
        var Admin = eval('(<%=this.Model.Admin%>)');

        //页面加载时
        $(function () {
            $("#Status").combobox("setValue", 10);

            $('#datagrid').bvgrid({
                pageSize: 20,
                queryParams: { Status: 10, action: "data" },
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (!!row.CreateDate) {
                            if (isNaN(Date(row.CreateDate))) {
                                row.CreateDate = row.CreateDate.replace(/T/, ' ').replace(/\.\d+$/, '');
                            } else {
                                row.CreateDate = new Date(row.CreateDate).toDateTimeStr();
                            }
                        }
                    }
                    return data;
                }
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.Status == 10) {
                buttons += '<button id="btnAppr" onclick="Appr(' + index + ')">审批</button>';
            }
            if (row.Status == 20 || row.Status == 30 ) {
                buttons += '<button id="btnAppr" onclick="View(' + index + ')">查看</button>';
            }
            return buttons;
        };

        //查看
        function View(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = null;
                if (rowdata.Type == "30") {
                    url = location.pathname.replace(/List.aspx/ig, 'ClientView.aspx') + "?ID=" + rowdata.ID + "&clientid=" + rowdata.MainID;
                }
                if (rowdata.Type == "50" || rowdata.Type == "80" || rowdata.Type == "100") {
                    url = location.pathname.replace(/List.aspx/ig, 'ProductView.aspx') + "?ID=" + rowdata.ID + "&itemid=" + rowdata.MainID;
                }
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '查看',
                    url: url,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        };


        //审批
        function Appr(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = null;
                if (rowdata.Type == "30") {
                    url = location.pathname.replace(/List.aspx/ig, 'ClientApr.aspx') + "?ID=" + rowdata.ID + "&clientid=" + rowdata.MainID;
                }
                if (rowdata.Type == "50" || rowdata.Type == "80"  || rowdata.Type == "100") {
                    url = location.pathname.replace(/List.aspx/ig, 'ProductApr.aspx') + "?ID=" + rowdata.ID + "&itemid=" + rowdata.MainID;
                }
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '我的审批',
                    url: url,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        };

        //关闭窗口后刷新
        function CloseLoad() {
            var AdminID = $("#AdminID").combobox("getValue");
            var Status = $("#Status").combobox("getValue");
            $('#datagrid').bvgrid('flush', { AdminID: AdminID, Status: Status });
        }

        //查询
        function Search() {
            var AdminID = $("#AdminID").combobox("getValue");
            var Status = $("#Status").combobox("getValue");
            $('#datagrid').bvgrid('search', { AdminID: AdminID, Status: Status });
        }

        //重置
        function Reset() {
            $("#query").form('clear');
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="查询列表" data-options="region:'north',border:false" style="height: 100px">
        <table id="query" cellpadding="0" cellspacing="0" style="margin-top: 10px">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr>
                <td class="lbl" style="text-align: center">申请人</td>
                <td>
                    <input class="easyui-combobox" id="AdminID" name="AdminID"
                        data-options="valueField:'ID',textField:'RealName',data: Admin" style="width: 95%" />
                </td>
                <td class="lbl" style="text-align: center">申请状态</td>
                <td>
                    <input class="easyui-combobox" id="Status" name="Status"
                        data-options="valueField:'value',textField:'text',data: Status" style="width: 95%" />
                </td>
            </tr>
        </table>
        <div style="margin-top: 10px; margin-left: 20px">
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
        </div>
    </div>
    <div title="我的审批" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" data-options="fit:true,scrollbarSize:0">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 50px">操作</th>
                    <th field="ID" data-options="align:'center'" style="width: 100px">ID</th>
                    <th field="TypeName" data-options="align:'center'" style="width: 100px">申请类型</th>
                    <th field="MainID" data-options="align:'center'" style="width: 100px">关联ID</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 100px;">申请人</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 100px;">申请状态</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 200px;">申请时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
