<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.AgreementChange.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>申请列表</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var Status = eval('(<%=this.Model.Status%>)');

        $(function () {

            //列表初始化
            $('#datagrid').myDatagrid({
                actionName: 'data',
                fitColumns: true,
                fit: true,
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
            $("#Status").combobox({
                data: Status
            });
        });

        //查询
        function Search() {

            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var Status = $('#Status').combobox('getValue');

            var parm = {
                ClientCode: ClientCode,
                ClientName: ClientName,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
                Status: Status
            };

            $('#datagrid').myDatagrid({
                actionName: 'data',
                queryParams: parm,
                fitColumns: true,
                fit: true,
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

        }

        //重置查询条件
        function Reset() {
            $('#ClientCode').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            $('#Status').combobox('setValue', null);
            Search();
        }
        //查看详情
        function View(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'AgreementChangeView.aspx')
                + '?From=Add&ID=' + ID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
                width: 1200,
                height: 900,
                onClose: function () {
                    Search();
                }
            });
        }
        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\''
                + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">客户名称：</span>
                    <input class="easyui-textbox" id="ClientName" data-options="height:26,width:180" />
                    <span class="lbl">客户编号：</span>
                    <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:180" />
                    <span class="lbl">状态：</span>
                    <input class="easyui-combobox" id="Status" data-options="valueField:'Key',textField:'Value',editable:false, width: 180" />
                    <br />
                    <span class="lbl">申请日期: </span>
                    <input class="easyui-datebox" id="CreateDateFrom" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="CreateDateTo" data-options="editable:false" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 10px;" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="业务员申请列表" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'ClientName',align:'left'" style="width: 16%;">客户名称</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 8%;">客户编号</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 8%;">信用等级</th>
                    <th data-options="field:'CreateDate',align:'center'," style="width: 8%;">申请变更日期</th>
                    <th data-options="field:'ClientNature',align:'center'" style="width: 8%;">客户类型</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 8%;">跟单员</th>
                    <th data-options="field:'Summary',align:'center'" style="width: 20%;">备注</th>
                    <th data-options="field:'Status',align:'center'" style="width: 8%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 8%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
