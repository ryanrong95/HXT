<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnMatchedInterestList.aspx.cs" Inherits="WebApp.Finance.Receipt.Notice.UnMatchedInterestList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>贴现利息</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>


        var pageNumber = getQueryString("pageNumber");
        var pageSize = getQueryString("pageSize");

        var initClientName = '<%=this.Model.ClientName%>';  //getQueryString("ClientName");
        var initQuerenStatus = getQueryString("QuerenStatus");
        var initStartDate = getQueryString("StartDate");
        var initEndDate = getQueryString("EndDate");
    </script>
    <script type="text/javascript">
        $(function () {
            if (pageNumber == null || pageNumber == "") {
                pageNumber = 1;
            }
            if (pageSize == null || pageSize == "") {
                pageSize = 10;
            }

            //订单列表初始化
            $('#datagrid').myDatagrid({
                //url: location.pathname,  // + "?ClientName=" + initClientName + "&QuerenStatus=" + initQuerenStatus + "&StartDate=" + initStartDate + "&EndDate=" + initEndDate,
                //queryParams: {
                //ClientName: initClientName,
                //QuerenStatus: initQuerenStatus,
                //StartDate: initStartDate,
                //EndDate: initEndDate,
                //},
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                pageNumber: pageNumber,
                pageSize: pageSize,
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
                },
                onBeforeLoad: function (param) {
                    pageNumber = param.page;
                    pageSize = param.rows;
                },
            });


        });

        //查询
        function Search() {
            var ClientName = $('#ClientName').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var OrderMatched = $('#OrderMatched').combobox('getValue');

            debugger
            var parm = {
                ClientName: ClientName,
                StartDate: StartDate,
                EndDate: EndDate,
                OrderMatched:OrderMatched
            };
            $('#datagrid').myDatagrid('search', parm);

        }

        //重置查询条件
        function Reset() {
            $('#ClientName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#OrderMatched').combobox('setValue', null);
            Search();
        }

        //编辑
        function Edit(ID,Payer) {
            var url = location.pathname.replace(/UnMatchedInterestList.aspx/ig, 'InterestMatch.aspx') + "?ID=" + ID+"&Payer="+Payer;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '关联订单',
                width: '350',
                height: '200',
                url: url,
                onClose: function () {
                    Search();
                }
            });

        }



        //列表框按钮加载
        function Operation(val, row, index) {
            var buttonsAddReceipt = "";
            if (row.OrderID == "" || row.OrderID == null) {
                buttonsAddReceipt = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\''
                    + row.ID + '\',\''+row.Payer+'\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">关联</span>' +
                    '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttonsAddReceipt;
        }
    </script>

    <style>
        table.search-condition td {
            padding-left: 5px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table class="search-condition" style="line-height: 30px;">
                <tr>
                    <td class="lbl">付款人:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">订单是否关联:</td>
                    <td>
                        <select class="easyui-combobox" id="OrderMatched" data-options="width:200,editable:false">
                            <option value="2">全部</option>
                            <option value="1">已关联</option>
                            <option value="0">未关联</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="贴现利息列表" data-options="
            border:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'OutSeqNo',align:'left'" style="width: 70px;">流水号</th>
                    <th data-options="field:'FromSeqNo',align:'left'" style="width: 70px;">承兑票号</th>
                    <th data-options="field:'AccountName',align:'center'" style="width: 150px;">账户名称</th>
                    <th data-options="field:'Payer',align:'left'" style="width: 150px;">付款人</th>
                    <th data-options="field:'DiscountInterest',align:'center'" style="width: 40px;">贴现利息</th>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 70px;">收款日期</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 110px;">关联订单</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 80px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
