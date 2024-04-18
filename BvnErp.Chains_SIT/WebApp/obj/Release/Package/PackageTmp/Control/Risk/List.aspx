<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Control.Risk.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>风控管控审批</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var RealName = '<%=this.Model.RealName%>';
        $(function () {
            //待审批列表初始化
            $('#controls').myDatagrid({
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
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            $('#controls').myDatagrid('search', { OrderID: orderID, ClientCode: clientCode });
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            Search();
        }

        //管控审批
        function Auditing(index) {
            $('#controls').datagrid('selectRow', index);
            var rowdata = $('#controls').datagrid('getSelected');
            if (rowdata) {
                var url;
                if (rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.CCC.GetHashCode()%>' ||
                    rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.OriginCertificate.GetHashCode()%>') {
                    url = location.pathname.replace(/List.aspx/ig, 'FileDisplay.aspx') + '?ID=' + rowdata.ID;
                } else if (rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.Forbid.GetHashCode()%>' ||
                    rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.ClassifyAnomaly.GetHashCode()%>' ||
                    rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.CheckingAbnomaly.GetHashCode()%>' ||
                    rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.SortingAbnomaly.GetHashCode()%>') {
                    url = location.pathname.replace(/List.aspx/ig, 'ReturnDisplay.aspx') + '?ID=' + rowdata.ID;
                } else if (rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.ExceedLimit.GetHashCode()%>' ||
                    rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.OverdueAdvancePayment.GetHashCode()%>') {
                    url = location.pathname.replace(/List.aspx/ig, '../Merchandiser/ExceedLimitDisplay.aspx') + '?ID=' + rowdata.ID + '&OrderID=' + rowdata.OrderID + '&Ststus=Edit';
                } else if (rowdata.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.OriginChange.GetHashCode()%>') {
                    url = location.pathname.replace(/List.aspx/ig, 'OriginChangeDisplay.aspx') + '?ID=' + rowdata.ID;
                }
                window.location = url;
            }
        }

        //查看管控详情
        function View(index) {
            $('#controls').datagrid('selectRow', index);
            var rowdata = $('#controls').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, '../Merchandiser/Detail.aspx') + '?ID=' + rowdata.ID + '&Ststus=Edit';
                window.location = url;
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';
            if (row.ControlStatus == '待总部审批') {
                //if ((row.DepartmentCode == '业务二部' && RealName == '张令金') || (row.DepartmentCode == '业务一部' && RealName == '张庆永') || RealName == '风控') {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Auditing(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">审批</span>' +
                    '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                // }
            } else {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="controls" title="待审批列表" data-options="nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 15%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 5%;">信用等级</th>
                    <th data-options="field:'ControlType',align:'center'" style="width: 10%;">管控类型</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 10%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'Merchandiser',align:'center'" style="width: 10%;">跟单员</th>
                    <th data-options="field:'OrderDate',align:'center'" style="width: 10%;">下单日期</th>
                    <th data-options="field:'ControlStatus',align:'center'" style="width: 5%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
