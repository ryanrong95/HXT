<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessedList.aspx.cs" Inherits="WebApp.PvData_ProductChange.ProcessedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>产品变更--已处理</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/classify.ajax.js"></script>
    <script src="../Scripts/Ccs.js?time=20190910"></script>
    <script src="../Scripts/pvdata.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '产品变更(中心数据)';
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
            MaskUtil.mask();
            $('#products').datagrid('selectRow', index);
            var data = $('#products').datagrid('getSelected');
            $.post('?action=GetClassified', { itemId: data.OrderItemID }, function (res) {
                MaskUtil.unmask();
                if (res.success) {
                    doClassify(res.data, {
                        xdt: true
                    });
                } else {
                    $.messager.alert('提示', res.data);
                }
            });
        }
        function doClassify(data, otherOptions) {
            $.classifyAjax.conts.openUrl = '/PvData/Classify/Edit.html'
            $.classifyAjax(data, {
                onClose: function () {
                    Search(true);
                }
            }, otherOptions);
        }

        //查询
        function Search(flag) {
            var clientCode = $('#ClientCode').textbox('getValue');
            var orderId = $('#OrderID').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');

            var opts = $("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");

            opts.pageSize = opts.pageSize;
            opts.url = currentSc.InitUrl;
            
            if (!flag) {
                opts.pageNumber = initPageNumber;
                pager.pagination("refresh", {
                    pageNumber: initPageNumber,
                    pageSize: opts.pageSize,
                });
            } else {
                pager.pagination("refresh", {
                    pageNumber: opts.pageNumber,
                    pageSize: opts.pageSize,
                });
            }


            $('#products').datagrid('reload', {
                action: 'data',
                ClientCode: clientCode.trim(),
                OrderId: orderId.trim(),
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
            var opts = $("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");
            opts.pageNumber = initPageNumber;
            opts.pageSize = initPageSize;
            opts.url = currentSc.InitUrl;
            pager.pagination("refresh", {
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
                    <br />
                    <span class="lbl">添加日期: </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl" style="margin:0 23px;">至:</span>
                    <input class="easyui-datebox" id="EndDate" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left:6px;" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="已归类产品" data-options="nowrap:false,border:false,fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 8%;">操作</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 13%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 7%;">客户编号</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 16%;">客户名称</th>
                    <th data-options="field:'ProductName',align:'left'" style="width: 15%;">品名</th>
                    <th data-options="field:'ProductModel',align:'center'" style="width: 12%;">型号</th>
                    <th data-options="field:'Type',align:'center'" style="width: 8%;">类型</th>
                    <th data-options="field:'Date',align:'center'" style="width: 13%;">添加时间</th>
                    <th data-options="field:'ProcessState',align:'center'" style="width: 8%;">状态</th>
                </tr>
            </thead>
        </table>
    </div>

</body>
</html>
