<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FirstList.aspx.cs" Inherits="WebApp.Classify.PreProduct.FirstList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预处理一</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
<%--    <script>
        gvSettings.fatherMenu = '产品预归类(XDT)';
        gvSettings.menu = '预处理一';
        gvSettings.summary = '首次归类';
    </script>--%>
    <script type="text/javascript">
        var currentSc = eval('(<%=this.Model.CurrentSc%>)');

        var initPageNumber = 1;
        var initPageSize = 20;
        var initUrl = '';

        $(function () {
            $('#Model').textbox('setValue', currentSc.Model);
            $('#IsShowLocked').prop('checked', currentSc.IsShowLocked);
            $("#IsShowLocked").css("display", "none");

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
                onLoadSuccess: function (data) {
                    for (var i = 0; i < data.rows.length; i++) {
                        if (!data.rows[i].IsCanClassify) {
                            $('.datagrid-btable').find("input[type='checkbox']")[i].disabled = 'disabled';
                        }
                    }
                },
                onSelect: function (index, row) {
                    if (!row.IsCanClassify) {
                        $('#products').datagrid('unselectRow', index);

                        if (IsCheckAll()) {
                            $("table input[type='checkbox']")[0].checked = true;
                        }
                    }
                },
                onCheck: function (index, row) {
                    if (IsCheckAll()) {
                        $("table input[type='checkbox']")[0].checked = true;
                    }
                },
                onCheckAll: function (rows) {
                    for (var index = 0; index < rows.length; index++) {
                        var row = rows[index];
                        if (!row.IsCanClassify) {
                            $('#products').datagrid('unselectRow', index);
                        }
                    }
                    $("table input[type='checkbox']")[0].checked = true;
                },
                onBeforeLoad: function (param) {
                    currentSc.PageNumber = param.page;
                    currentSc.PageSize = param.rows;
                },
            });

            $('#IsShowLocked').change(function () {
                Search();
            });
        });

        //查询
        function Search() {
            var Model = $('#Model').textbox('getValue');
            var isShowLocked = $('#IsShowLocked').prop("checked");

            //$('#products').myDatagrid('search', {
            //    Model: Model,
            //    IsShowLocked: isShowLocked,
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
                IsShowLocked: isShowLocked,
            });

            currentSc.PageNumber = $('#products').datagrid('options').pageNumber;
            currentSc.PageSize = $('#products').datagrid('options').pageSize;
            currentSc.IsShowLocked = isShowLocked;
            currentSc.Model = Model;
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
            $('#IsShowLocked').prop('checked', false);
            Search();
        }

        //批量锁定
        function BatchLock() {
            var docData = $('#products').datagrid('getChecked');
            var arr = new Array();
            for (var i = 0; i < docData.length; i++) {
                arr[i] = docData[i].ID;
            }

            if (arr.length == 0) {
                $.messager.alert('提示', '请至少选择一个需要锁定的产品！');
                return;
            }

            var jsonString = JSON.stringify(arr);
            $.messager.confirm('确认', '确认产品归类信息无误，进行锁定？', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=BatchLock', { IDs: jsonString }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('提示', result.message, 'info', function () {
                                $('#products').datagrid('reload');
                            });
                        } else {
                            $.messager.alert('提示', result.message);
                        }
                    });
                }
            });
        }

        //订单归类
        function Classify(index) {
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            if (rowdata) {
                MaskUtil.mask();
                $.post('?action=Lock', { ID: rowdata.ID }, function (res) {
                    MaskUtil.unmask();
                    var result = JSON.parse(res);
                    if (result.success) {
                        var preProductID = rowdata.PreProductID;
                        var url = location.pathname.replace(/FirstList.aspx/ig, 'Edit.aspx') + '?ID=' + rowdata.ID + '&PreProductID=' + rowdata.ID + '&From=<%=Needs.Ccs.Services.Enums.ClassifyStep.PreStep1.GetHashCode()%>&SourcePage=Add'
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

        //是否勾选全选框
        function IsCheckAll() {
            var isCheckAll = true;
            var rows = $('#products').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].IsCanClassify) {
                    if (!$('.datagrid-btable').find("input[type='checkbox']")[i].checked) {
                        isCheckAll = false;
                        break;
                    };
                }
            }

            return isCheckAll;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.IsCanClassify) {
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
        <div id="tool">
            <a id="btnBatchLock" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-lock'" onclick="BatchLock()" style="margin-left: 10px;">批量锁定</a>
            <input type="checkbox" id="IsShowLocked" name="IsShowLocked" checked="checked" class="checkbox" /><label for="IsShowLocked" style="margin-left: 15px;">显示全部</label>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">型号: </span>
                    <input class="easyui-textbox" id="Model" data-options="validType:'length[1,50]'" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>                  
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="待归类产品" data-options="nowrap:false,border:false,fitColumns:false,fit:true,singleSelect:false,toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ProductName',align:'left'" style="width: 9%;">品名</th>
                    <th data-options="field:'Model',align:'left'" style="width: 14%;">产品型号</th>
                    <th data-options="field:'Manufacturer',align:'left'" style="width: 8%;">品牌</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 6%;">单价</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币制</th>
                    <th data-options="field:'ClassifyStatus',align:'center'" style="width: 6%;">归类状态</th>
                    <th data-options="field:'LockStatus',align:'center'" style="width: 6%;">锁定状态</th>
                    <th data-options="field:'Locker',align:'center'" style="width: 7%;">锁定人</th>
                    <th data-options="field:'LockTime',align:'center'" style="width: 6%;">锁定时间</th>
                    <th data-options="field:'DueDate',align:'center'" style="width: 150px;">预计到货日期</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 6%;">创建时间</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 6%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 10%;">客户名称</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>

            <%--<thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'LockStatus',align:'center'" style="width: 5%;">锁定状态</th>
                    <th data-options="field:'Locker',align:'center'" style="width: 6%;">锁定人</th>
                    <th data-options="field:'LockTime',align:'left'" style="width: 10%;">锁定时间</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'Name',align:'left'" style="width: 10%;">报关品名</th>
                    <th data-options="field:'Model',align:'left'" style="width: 12%;">产品型号</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 8%;">品牌</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 5%;">原产地</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 6%;">申报数量</th>
                    <th data-options="field:'Unit',align:'center'" style="width: 5%;">申报单位</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 5%;">单价</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 12%;">操作</th>
                </tr>
            </thead>--%>
        </table>
    </div>
</body>
</html>

