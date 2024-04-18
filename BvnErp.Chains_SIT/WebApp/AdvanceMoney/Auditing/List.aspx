<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.AdvanceMoney.Auditing.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>垫资列表</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <style type="text/css">
        .rowlink {
            color: blue;
            cursor: pointer;
            text-decoration: underline
        }
    </style>
    <script type="text/javascript">
        var AdvanceMoneyStatus = eval('(<%=this.Model.AdvanceMoneyStatus%>)');
        var ServiceManager = eval('(<%=this.Model.ServiceManager%>)');
        var RealName = '<%=this.Model.RealName%>';
        $(function () {

            //初始化业务员下拉框
            $('#ServiceManager').combobox({
                data: ServiceManager,
            })
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
                data: AdvanceMoneyStatus

            });
        });

        //查询
        function Search() {

            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var Status = $('#Status').combobox('getValue');
            var ServiceManager = $('#ServiceManager').combobox('getValue');

            var parm = {
                ClientCode: ClientCode,
                ClientName: ClientName,
                Status: Status,
                ServiceManager: ServiceManager
            };

            //公告列表初始化
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
            $('#Status').combobox('setValue', null);
            $('#ServiceManager').combobox('setValue', null);
            Search();
        }

        //查看详情
        function View(ID) {
            var url = location.pathname.replace(/List.aspx/ig, '../View.aspx')
                + '?From=Audit&ID=' + ID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
                width: 820,
                height: 620,
                onClose: function () {
                    Search();
                }
            });
        }
        //查看详细
        function Auditing(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, '../View.aspx') + "?From=Audit&ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '查看',
                    width: 820,
                    height: 620,
                    onClose: function () {
                        Search();
                    }
                });
            }
        }
        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';
            if (row.IntStatus == '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective.GetHashCode()%>'
                || row.IntStatus == '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete.GetHashCode()%>'
                || row.IntStatus == '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Auditing.GetHashCode()%>') {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\''
                    + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            else {

                //if ((row.DepartmentCode == '业务二部' && RealName == '张令金') || (row.DepartmentCode == '业务一部' && RealName == '张庆永') || RealName == '风控') {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Auditing(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">审核</span>' +
                    '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                //}
            }
            return buttons;
        }
        //已使用金额
        function AmountUsedView(val, row, index) {
            return "<a class=\"rowlink\" onclick=\"ViewPaidDetail('" + row.ClientID + "')\" >" + row.AmountUsed + "</a>";
        }
        function ViewPaidDetail(ClientID) {
            var url = location.pathname.replace(/List.aspx/ig, '../AdvanceRecord.aspx')
                + '?ClientID=' + ClientID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '垫资记录',
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
                    <span class="lbl">客户编号：</span>
                    <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:180" />
                    <span class="lbl">客户名称：</span>
                    <input class="easyui-textbox" id="ClientName" data-options="height:26,width:180" />
                    <span class="lbl">状态：</span>
                    <input class="easyui-combobox" id="Status" data-options="valueField:'Key',textField:'Value',editable:false, width: 180" />
                    <span class="lbl">业务员：</span>
                    <input class="easyui-combobox" id="ServiceManager" data-options="valueField:'Key',textField:'Value',editable:false, width: 180" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 10px;" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="垫资列表" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 16%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 16%;">客户名称</th>
                    <th data-options="field:'Name',align:'left'" style="width: 10%;">业务员</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 8%;">金额</th>
                    <th data-options="field:'AmountUsed',align:'center',formatter:AmountUsedView" style="width: 8%;">已使用金额</th>
                    <th data-options="field:'LimitDays',align:'center'" style="width: 8%;">垫资期限（天）</th>
                    <th data-options="field:'InterestRate',align:'center'" style="width: 8%;">月利率</th>
                    <th data-options="field:'OverdueInterestRate',align:'center'" style="width: 8%;">逾期日利率</th>
                    <th data-options="field:'Status',align:'center'" style="width: 8%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
