<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DoneList.aspx.cs" Inherits="WebApp.Classify.Product.DoneList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已完成列表</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
   <%-- <script>
        gvSettings.fatherMenu = '产品归类(XDT)';
        gvSettings.menu = '已完成';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        var currentSc = eval('(<%=this.Model.CurrentSc%>)');

        var initPageNumber = 1;
        var initPageSize = 20;
        var initUrl = '';

        $(function () {
            $('#OrderID').textbox('setValue', currentSc.OrderID);
            $('#Model').textbox('setValue', currentSc.Model);
            $('#ProductName').textbox('setValue', currentSc.ProductName);
            $('#HSCode').textbox('setValue', currentSc.HSCode);
            $('#LastClassifyTimeBegin').textbox('setValue', currentSc.LastClassifyTimeBegin);
            $('#LastClassifyTimeEnd').textbox('setValue', currentSc.LastClassifyTimeEnd);

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

            //订单列表初始化
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

        //查询
        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var model = $('#Model').textbox('getValue');
            var ProductName = $('#ProductName').textbox('getValue');
            var HSCode = $('#HSCode').textbox('getValue');
            var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
            var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');

            //$('#products').myDatagrid('search', {
            //    OrderID: orderID,
            //    Model: model,
            //    ProductName: ProductName,
            //    HSCode: HSCode,
            //    LastClassifyTimeBegin: LastClassifyTimeBegin,
            //    LastClassifyTimeEnd: LastClassifyTimeEnd,
            //});

            var opts = $("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");
            opts.pageNumber = initPageNumber;
            opts.pageSize = opts.pageSize;
            opts.url = currentSc.InitUrl;
            pager.pagination("refresh", {
                pageNumber: initPageNumber,
                pageSize: opts.pageSize,
            });

            $('#products').datagrid('reload', {
                action: 'data',
                OrderID: orderID,
                Model: model,
                ProductName: ProductName,
                HSCode: HSCode,
                LastClassifyTimeBegin: LastClassifyTimeBegin,
                LastClassifyTimeEnd: LastClassifyTimeEnd,
            });

            currentSc.PageNumber = $('#products').datagrid('options').pageNumber;
            currentSc.PageSize = $('#products').datagrid('options').pageSize;
            currentSc.Model = model;
            currentSc.OrderID = orderID;
            currentSc.ProductName = ProductName;
            currentSc.HSCode = HSCode;
            currentSc.LastClassifyTimeBegin = LastClassifyTimeBegin;
            currentSc.LastClassifyTimeEnd = LastClassifyTimeEnd;
        }

        //重置查询条件
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
            $('#Model').textbox('setValue', null);
            $('#ProductName').textbox('setValue', null);
            $('#HSCode').textbox('setValue', null);
            $('#LastClassifyTimeBegin').textbox('setValue', null);
            $('#LastClassifyTimeEnd').textbox('setValue', null);
            Search();
        }

        //查看产品归类信息
        function View(index) {
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/DoneList.aspx/ig, 'Edit.aspx') + '?ID=' + rowdata.ID + '&From=<%=Needs.Ccs.Services.Enums.ClassifyStep.Done.GetHashCode()%>'
                    + "&" + parseParams(currentSc);
                window.location = url;
            }
        }

        //重新归类
        function Classify(index) {
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            if (rowdata) {
                MaskUtil.mask();
                $.post('?action=Lock', { ID: rowdata.ID, From: '<%=Needs.Ccs.Services.Enums.ClassifyStep.DoneEdit.GetHashCode()%>', }, function (res) {
                    MaskUtil.unmask();
                    var result = JSON.parse(res);
                    if (result.success) {
                        var url = location.pathname.replace(/DoneList.aspx/ig, 'Edit.aspx') + '?ID=' + rowdata.ID + '&From=<%=Needs.Ccs.Services.Enums.ClassifyStep.DoneEdit.GetHashCode()%>'
                            + "&" + parseParams(currentSc);
                        window.location = url;
                    } else {
                        $.messager.alert('提示', result.message);
                        $('#products').datagrid('reload');
                    }
                });
            }
        }

        //解除归类锁定
        function UnLock(ID) {
            MaskUtil.mask();
            $.post('?action=UnLock', { ID: ID, }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert('提示', result.message);
                    $('#products').datagrid('reload');
                } else {
                    $.messager.alert('提示', result.message);
                }
            });
        }

        //导出Excel
        function Export() {
            var OrderID = $('#OrderID').textbox('getValue');
            var Model = $('#Model').textbox('getValue');
            var ProductName = $('#ProductName').textbox('getValue');
            var HSCode = $('#HSCode').textbox('getValue');
            var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
            var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
                OrderID: OrderID,
                Model: Model,
                ProductName: ProductName,
                HSCode: HSCode,
                LastClassifyTimeBegin: LastClassifyTimeBegin,
                LastClassifyTimeEnd: LastClassifyTimeEnd,
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            if (row.IsQuoted == false && row.IsCanClassify) {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Classify(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">归类</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px" onclick="Classify(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">归类</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            if (row.IsCanUnlock) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="UnLock(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">解锁</span>' +
                    '<span class="l-btn-icon icon-lock">&nbsp;</span>' +
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

                    <span class="lbl" style="margin-left: 34px;">型号: </span>
                    <input class="easyui-textbox" id="Model" data-options="validType:'length[1,50]'" />
                    <span class="lbl" style="margin-left: 34px;">品名: </span>
                    <input class="easyui-textbox" id="ProductName" data-options="validType:'length[1,50]'" />
                    <span class="lbl">海关编号: </span>
                    <input class="easyui-textbox" id="HSCode" data-options="validType:'length[1,50]'" />
                </li>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">归类时间: </span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeBegin" />
                    <span class="lbl" style="margin-left: 24px;">至 </span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeEnd" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="已归类产品" data-options="nowrap:false,border:false,fitColumns:false,fit:true,toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'HSCode',align:'center',width:100">HS编码</th>
                    <th data-options="field:'Name',align:'left',width:180">报关品名</th>
                    <th data-options="field:'Elements',align:'left',width:500">申报要素</th>
                    <th data-options="field:'Model',align:'center',width:150">产品型号</th>
                    <th data-options="field:'Manufacturer',align:'center',width:80">品牌</th>
                    <th data-options="field:'Origin',align:'center',width:80">原产地</th>
                    <th data-options="field:'Quantity',align:'center',width:80">申报数量</th>
                    <th data-options="field:'Unit',align:'center',width:80">申报单位</th>
                    <th data-options="field:'Unit1',align:'center',width:80">第一单位</th>
                    <th data-options="field:'UnitPrice',align:'center',width:80">单价</th>
                    <th data-options="field:'Currency',align:'center',width:80">币种</th>
                    <th data-options="field:'TariffRate',align:'center',width:80">关税率%</th>
                    <th data-options="field:'ValueAddRate',align:'center',width:80">增值税率%</th>
                    <th data-options="field:'CIQCode',align:'center',width:80">检验检疫编码</th>
                    <th data-options="field:'ClassifyStatus',align:'center',width:100">归类状态</th>
                    <th data-options="field:'ClassifyFirstOperatorName',align:'center',width:100">预处理一人员</th>
                    <th data-options="field:'ClassifySecondOperatorName',align:'center',width:100">预处理二人员</th>
                    <th data-options="field:'LockStatus',align:'center',width:80">锁定状态</th>
                    <th data-options="field:'Locker',align:'center',width:100">锁定人</th>
                    <th data-options="field:'LockTime',align:'center',width:100">锁定时间</th>
                    <th data-options="field:'CreateDate',align:'center',width:100">创建时间</th>
                    <th data-options="field:'OrderStatus',align:'center',width:100">订单状态</th>
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 130px;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 70px;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 180px;">客户名称</th>
                    <th data-options="field:'Btn',align:'left',width:200,formatter:Operation">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
