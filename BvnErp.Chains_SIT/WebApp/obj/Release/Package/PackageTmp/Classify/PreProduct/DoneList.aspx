<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DoneList.aspx.cs" Inherits="WebApp.Classify.PreProduct.DoneList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已完成</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <%--<script>
        gvSettings.fatherMenu = '产品预归类(XDT)';
        gvSettings.menu = '已完成';
        gvSettings.summary = '已完成';
    </script>--%>
    <script type="text/javascript">
        var currentSc = eval('(<%=this.Model.CurrentSc%>)');

        var initPageNumber = 1;
        var initPageSize = 20;
        var initUrl = '';

        $(function () {
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

            //待归类产品列表初始化
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
        function SearchFilter() {
            var Model = $('#Model').textbox('getValue');
            var ProductName = $('#ProductName').textbox('getValue');
            var HSCode = $('#HSCode').textbox('getValue');
            var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
            var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');

            //$('#products').myDatagrid('search', {
            //    Model: Model,
            //    ProductName: ProductName,
            //    HSCode: HSCode,
            //    LastClassifyTimeBegin: LastClassifyTimeBegin,
            //    LastClassifyTimeEnd: LastClassifyTimeEnd,
            //});

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
                Model: Model,
                ProductName: ProductName,
                HSCode: HSCode,
                LastClassifyTimeBegin: LastClassifyTimeBegin,
                LastClassifyTimeEnd: LastClassifyTimeEnd,
            });

            currentSc.PageNumber = $('#products').datagrid('options').pageNumber;
            currentSc.PageSize = $('#products').datagrid('options').pageSize;
            currentSc.Model = Model;
            currentSc.ProductName = ProductName;
            currentSc.HSCode = HSCode;
            currentSc.LastClassifyTimeBegin = LastClassifyTimeBegin;
            currentSc.LastClassifyTimeEnd = LastClassifyTimeEnd;
        }

        //重置查询条件
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

            $('#Model').textbox('setValue', null);
            $('#ProductName').textbox('setValue', null);
            $('#HSCode').textbox('setValue', null);
            $('#LastClassifyTimeBegin').textbox('setValue', null);
            $('#LastClassifyTimeEnd').textbox('setValue', null);
            SearchFilter();
        }

      //订单归类
        function Search(index) {
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            if (rowdata) {
                var preProductID = rowdata.PreProductID;    
                var url = location.pathname.replace(/DoneList.aspx/ig, 'Edit.aspx') + '?ID=' + rowdata.ID + '&PreProductID=' + rowdata.ID + '&From=<%=Needs.Ccs.Services.Enums.ClassifyStep.PreDone.GetHashCode()%>&SourcePage=Search'
                    + "&" + parseParams(currentSc);
                window.location = url;
            }
        }

        //订单归类
        function Classify(index) {
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            if (rowdata) {
                MaskUtil.mask();
                $.post('?action=Lock', { ID: rowdata.ID, From: '<%=Needs.Ccs.Services.Enums.ClassifyStep.PreDoneEdit.GetHashCode()%>', }, function (res) {
                    MaskUtil.unmask();
                    var result = JSON.parse(res);
                    if (result.success) {
                        var preProductID = rowdata.PreProductID;
                        var url = location.pathname.replace(/DoneList.aspx/ig, 'Edit.aspx') + '?ID=' + rowdata.ID + '&PreProductID=' + rowdata.ID + '&From=<%=Needs.Ccs.Services.Enums.ClassifyStep.PreDoneEdit.GetHashCode()%>&SourcePage=Add'
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
        function UnLock(index) {
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            if (rowdata) {
                MaskUtil.mask();
                $.post('?action=UnLock', { ID: rowdata.ID }, function (res) {
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
        }

        //导出Excel
        function Export() {
            var Model = $('#Model').textbox('getValue');
            var ProductName = $('#ProductName').textbox('getValue');
            var HSCode = $('#HSCode').textbox('getValue');
            var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
            var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
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
            var buttons = "";

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Search(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            if (row.IsOrdered == false && row.IsCanClassify) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Classify(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">归类</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            else {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px" onclick="Classify(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">归类</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            if (row.IsCanUnlock) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="UnLock(' + index + ')" group >' +
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
                    <span class="lbl">品名: </span>
                    <input class="easyui-textbox" id="ProductName" data-options="validType:'length[1,50]'" />
                    <span class="lbl">海关编码: </span>
                    <input class="easyui-textbox" id="HSCode" data-options="validType:'length[1,50]'" />
                </li>
                <li>
                    <span class="lbl">归类时间: </span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeBegin" />
                    <span class="lbl" style="margin-left: 24px;">至 </span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeEnd" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="SearchFilter()" style="margin-left: 10px;">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="已归类产品" data-options="nowrap:false,border:false,fitColumns:false,fit:true,toolbar:'#topBar',singleSelect:false,">
            <thead>
                <tr>
                    <th data-options="field:'Elements',align:'left',width:450">申报要素</th>
                    <th data-options="field:'TariffRate',align:'center',width:100">关税率%</th>
                    <th data-options="field:'UnitPrice',align:'center',width:100">单价</th>
                    <th data-options="field:'Currency',align:'center',width:100">币种</th>
                    <th data-options="field:'Unit1',align:'center',width:100">第一单位</th>
                    <th data-options="field:'Unit2',align:'center',width:100">第二单位</th>
                    <th data-options="field:'AddedValueRate',align:'center',width:100">增值税率%</th>
                    <th data-options="field:'CIQCode',align:'center',width:100">检验检疫编码</th>
                    <th data-options="field:'ClassifyStatus',align:'center',width:100">归类状态</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 70px;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 200px;">客户名称</th>
                    <th data-options="field:'LockStatus',align:'center',width:100">锁定状态</th>
                    <th data-options="field:'Locker',align:'center',width:100">锁定人</th>
                    <th data-options="field:'LockTime',align:'center',width:150">锁定时间</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建时间</th>
                    <th data-options="field:'ClassifyFirstOperatorName',align:'center',width:100">预处理一人员</th>
                    <th data-options="field:'ClassifySecondOperatorName',align:'center',width:100">预处理二人员</th>
                    <th data-options="field:'TaxCode',align:'center',width:150">税务编码</th>
                    <th data-options="field:'TaxName',align:'center',width:150">税务名称</th>
                    <th data-options="field:'PreOrderStatus',align:'center',width:150">是否下单</th>
                   <%-- <th data-options="field:'CompanyType',align:'center',width:150," style="display:none">公司类型</th>--%>
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'Btn',align:'left',width:200,formatter:Operation">操作</th>
                    <th data-options="field:'Manufacturer',align:'center',width:100">品牌</th>
                    <th data-options="field:'Model',align:'left',width:150">产品型号</th>
                    <th data-options="field:'ProductName',align:'left',width:130">报关品名</th>
                    <th data-options="field:'HSCode',align:'center',width:100">HS编码</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

