<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessList.aspx.cs" Inherits="WebApp.Classify.ProductChange.ProcessList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>重新归类</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
<%--    <script>
        gvSettings.fatherMenu = '产品变更';
        gvSettings.menu = '已处理';
        gvSettings.summary = '';
    </script>--%>
    <script>
        var currentSc = eval('(<%=this.Model.CurrentSc%>)');

        var initPageNumber = 1;
        var initPageSize = 20;
        var initUrl = '';

        $(function () {
            $('#OrderID').textbox('setValue', currentSc.OrderID);
            $('#ClientCode').textbox('setValue', currentSc.ClientCode);
            $('#StartDate').textbox('setValue', currentSc.ProductChangeAddTimeBegin);
            $('#EndDate').textbox('setValue', currentSc.ProductChangeAddTimeEnd);

            if (Number(currentSc.PageNumber) <= 0) {
                currentSc.PageNumber = initPageNumber;
            }
            if (Number(currentSc.PageSize) <= 0) {
                currentSc.PageSize = initPageSize;
            }
            if ('' == initUrl) {
                initUrl = location.pathname;
            }
            currentSc.InitUrl = initUrl;

            //已归类产品列表初始化
            $('#products').myDatagrid({
                pageNumber: currentSc.PageNumber,
                pageSize: currentSc.PageSize,
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
                    currentSc.PageNumber = param.page;
                    currentSc.PageSize = param.rows;
                },
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            return buttons;
        }

        //查看归类信息
        function View(index) {
            //跳转至重新归类
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/ProcessList.aspx/ig, '../Product/Edit.aspx') + '?ID=' + rowdata.OrderItemID + '&From=<%= Needs.Ccs.Services.Enums.ClassifyStep.ReClassified.GetHashCode() %>'
                    + "&" + parseParams(currentSc);
                window.location = url;
                } else {
                    $.messager.alert('提示', result.message);
                    $('#products').datagrid('reload');
                }
        }

        //查询
        function Search() {
            var clientCode = $('#ClientCode').textbox('getValue');
            var orderId = $('#OrderID').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');

            //var parm = {
            //    ClientCode: clientCode,
            //    OrderId: orderId,
            //    StartDate: startDate,
            //    EndDate: endDate
            //};
            //$('#products').myDatagrid('search', parm);

            var opts=$("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");
            opts.pageNumber = initPageNumber;
            opts.pageSize = opts.pageSize;
            opts.url = currentSc.InitUrl;
            pager.pagination("refresh",{
                pageNumber: initPageNumber,
                pageSize: opts.pageSize,
            });

            $('#products').datagrid('reload', {
                action: 'data',
                ClientCode: clientCode,
                OrderId: orderId,
                StartDate: startDate,
                EndDate: endDate,
            });

            currentSc.PageNumber = $('#products').datagrid('options').pageNumber;
            currentSc.PageSize = $('#products').datagrid('options').pageSize;
            currentSc.OrderID = orderId;
            currentSc.ClientCode = clientCode;
            currentSc.ProductChangeAddTimeBegin = startDate;
            currentSc.ProductChangeAddTimeEnd = endDate;
        }

        //重置
        function Reset() {
            var opts=$("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");
            opts.pageNumber = initPageNumber;
            opts.pageSize = initPageSize;
            opts.url = currentSc.InitUrl;
            pager.pagination("refresh",{
                pageNumber: initPageNumber,
                pageSize: initPageSize,
            });

            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <br/>
                    <span class="lbl">添加日期: </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">至:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                    <input class="easyui-datebox" id="EndDate" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="已归类产品" data-options="nowrap:false,border:false,fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead data-options="frozen:true">
            <tr>
                <th data-options="field:'OrderID',align:'left'" style="width: 13%;">订单编号</th>
                <th data-options="field:'ClientCode',align:'left'" style="width: 7%;">客户编号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 15%;">客户名称</th>
                <th data-options="field:'ProductName',align:'left'" style="width: 15%;">品名</th>
                <th data-options="field:'ProductModel',align:'center'" style="width: 9%;">型号</th>
                <th data-options="field:'Type',align:'center'" style="width: 8%;">类型</th>
                <th data-options="field:'Date',align:'center'" style="width: 13%;">添加时间</th>
                <th data-options="field:'ProcessState',align:'center'" style="width: 8%;">状态</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 12%;">操作</th>
            </tr>
            </thead>
        </table>
    </div>

</body>
</html>
