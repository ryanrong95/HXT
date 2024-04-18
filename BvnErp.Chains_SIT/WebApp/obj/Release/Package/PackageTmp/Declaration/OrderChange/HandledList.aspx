<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HandledList.aspx.cs" Inherits="WebApp.Declaration.OrderChange.HandledList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报关单-税费异常</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
 <%--   <script>
        gvSettings.fatherMenu = '税费异常';
        gvSettings.menu = '已处理';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //代理订单列表初始化
            $('#orders').myDatagrid({
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
            var contrNo = $('#ContrNo').textbox('getValue');
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            $('#orders').myDatagrid('search', {ContrNo: contrNo,OrderID: orderID, ClientCode: clientCode, StartDate: startDate, EndDate: endDate });
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        function ViewFee(ID) {
            var url = location.pathname.replace(/HandledList.aspx/ig, 'Edit.aspx') + '?ID=' + ID +'&From=ViewDeclareChange';
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.Type ==<%=Needs.Ccs.Services.Enums.OrderChangeType.TaxChange.GetHashCode()%>) {
                buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewFee(\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">详情</span>' +
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
                    <span class="lbl">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" data-options="validType:'length[1,50]'" />
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />
                    <br />
                    <span class="lbl">报关日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="订单变更" data-options="border:false,nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'EntryID',align:'left'" style="width: 10%;">报关单号</th>
                    <th data-options="field:'ContrNo',align:'left'" style="width: 10%;">合同号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 7%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'DDdate',align:'center'" style="width: 8%;">报关日期</th>
                    <th data-options="field:'OrderChangeType',align:'center'" style="width: 8%;">类型</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">添加时间</th>
                    <th data-options="field:'processState',align:'center'" style="width: 8%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

