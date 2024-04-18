<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OnStockList.aspx.cs" Inherits="WebApp.SZWarehouse.Entry.OnStockList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>上架列表-入库通知(SZ)</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '入库通知(SZ)';
        gvSettings.menu = '待入库';
        gvSettings.summary = '';
    </script>--%>
    <script>
        var pageNumber = getQueryString("pageNumber");
        var pageSize = getQueryString("pageSize");
        var initOnStockListScVoyageID = '<%=this.Model.OnStockListScVoyageID%>';
        var initOnStockListScCarrierName = '<%=this.Model.OnStockListScCarrierName%>';

        $(function () {
            $('#VoyageID').textbox('setValue', initOnStockListScVoyageID);
            $('#CarrierName').textbox('setValue', initOnStockListScCarrierName);
            if (pageNumber == null || pageNumber == "") {
                pageNumber = 1;
            }
            if (pageSize == null || pageSize == "") {
                pageSize = 10;
            }

            $('#unonstock-datagrid').myDatagrid({
                border: false,
                nowrap: false,
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                singleSelect: true,
                toolbar: '#topBar-unonstock-datagrid',
                pageNumber: pageNumber,
                pageSize: pageSize,
                actionName: 'UnStockList',
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
            var VoyageID = $('#VoyageID').textbox('getValue');
            var CarrierName = $('#CarrierName').textbox('getValue');

            $('#unonstock-datagrid').myDatagrid({
                pageNumber: pageNumber,
                pageSize: pageSize,
               actionName: 'UnStockList',
                queryParams: {
                    VoyageID: VoyageID,
                    CarrierName: CarrierName,
                },
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
        }

        //重置查询条件
        function Reset() {
            $('#VoyageID').textbox('setValue', null);
            $('#CarrierName').textbox('setValue', null);
            Search();
        }

        function UnOnStockDatagridOperation(val, row, index) {
            var buttons = '<a id="btnEntry" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="EnterOnStockDetail(\'' + row.VoyageID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">详情</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            var completeButton = '';
            if (row.StockedBoxNum >= row.AllBoxNum) {
                completeButton = '<a id="btnEntry" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Complete(\'' + row.VoyageID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">完成</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
                completeButton = '<a id="btnEntry" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px;" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">完成</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            buttons = buttons + completeButton;
            return buttons;
        }

        //进入详情
        function EnterOnStockDetail(voyageID) {
            var url = location.pathname.replace(/OnStockList.aspx/ig, 'OnStockDetail.aspx') + "?VoyageID=" + voyageID
                + "&OnStockListPageNumber=" + pageNumber + "&OnStockListPageSize=" + pageSize
                + "&OnStockListScVoyageID=" + encodeURI($('#VoyageID').textbox('getValue')) + "&OnStockListScCarrierName=" + encodeURI($('#CarrierName').textbox('getValue'));
            window.location = url;
        }

        //完成该运输批次的上架
        function Complete(voyageID) {
            $.messager.confirm('确认完成', '确认完成运输批次号 ' + voyageID + ' 的上架吗？', function (r) {
                if (r) {
                    var url = location.pathname.replace(/OnStockList.aspx/ig, 'OnStockDetail.aspx?action=Complete');
                    var params = {
                        "VoyageID": voyageID,
                    };

                    MaskUtil.mask();
                    $.post(url, params, function (res) {
                        MaskUtil.unmask();

                        var resData = JSON.parse(res);
                        if (resData.success == "true") {
                            $.messager.alert({
                                title: '完成',
                                msg: '运输批次号 ' + voyageID + ' 上架完成',
                                icon: 'info',
                                width: 300,
                                top: 200, //与上边距的距离
                                fn: function () {
                                    $('#unonstock-datagrid').datagrid('reload');
                                }
                            });
                        } else {
                            $.messager.alert('提示', resData.message);
                        }
                    });
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar-unonstock-datagrid">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">运输批次号: </span>
                    <input class="easyui-textbox search" data-options="" id="VoyageID" />
                    <span class="lbl">承运商: </span>
                    <input class="easyui-textbox search" data-options="" id="CarrierName" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="unonstock-datagrid" class="easyui-datagrid" title="待入库" data-options="
            border:false,
            nowrap:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:true,
            toolbar:'#topBar-unonstock-datagrid'">
            <thead>
                <tr>
                    <th field="VoyageID" data-options="align:'center'" style="width: 70px">运输批次号</th>
                    <th field="CarrierName" data-options="align:'center'" style="width: 70px">承运商</th>
                    <th field="HKLicense" data-options="align:'left'" style="width: 60px">车牌号</th>
                    <th field="TransportTime" data-options="align:'center'" style="width: 50px">运输时间</th>
                    <th field="DriverName" data-options="align:'center'" style="width: 50px">驾驶员姓名</th>
                    <th field="VoyageType" data-options="align:'center'" style="width: 40px">运输类型</th>
                    <th field="AllBoxNum" data-options="align:'center'" style="width: 40px">总箱数</th>
                    <th field="StockedBoxNum" data-options="align:'center'" style="width: 40px">已上架的箱数</th>
                    <th data-options="field:'btn',width:50,formatter:UnOnStockDatagridOperation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
