<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FirstList.aspx.cs" Inherits="WebApp.PvData_Classify.FirstList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>归类一列表</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/classify.ajax.js"></script>
    <script src="../Scripts/Ccs.js?time=20190910"></script>
    <script src="../Scripts/pvdata.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '产品归类(中心数据)';
        gvSettings.menu = '预处理一';
        gvSettings.summary = '报关员首次归类';
    </script>--%>
    <script type="text/javascript">
        var setWindow = 'FirstList_' + Math.floor(Math.random() * 10000);
        $.myWindow.setMyWindow(setWindow, window);
        var currentSc = eval('(<%=this.Model.CurrentSc%>)');
        var admin = eval('(<%=this.Model.Admin%>)');
        var domainUrls = eval('(<%=this.Model.DomainUrls%>)');

        var initPageNumber = 1;
        var initPageSize = 20;
        var initUrl = '';
        //$.myWindow.setMyWindow('FirstList', window);
        $(function () {
            $('#OrderID').textbox('setValue', currentSc.OrderID);
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
                fitColumns: false,
                fit: true,
                singleSelect: false,
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
                onLoadSuccess: function (data) {
                    var leftTrs = $(".datagrid-view1>.datagrid-body tr");
                    var rightTrs = $(".datagrid-view2>.datagrid-body tr");

                    for (var i = 0; i < leftTrs.length; i++) {
                        var useHeight = 0;

                        if ($(leftTrs[i]).height() > $(rightTrs[i]).height()) {
                            useHeight = $(leftTrs[i]).height();
                        } else {
                            useHeight = $(rightTrs[i]).height();
                        }

                        $(leftTrs[i]).height(useHeight);
                        $(rightTrs[i]).height(useHeight);
                    }

                },
            });

            $('#IsShowLocked').change(function () {
                Search();
            });
        });

        //查询
        function Search(flag) {
            var orderID = $('#OrderID').textbox('getValue');
            var model = $('#Model').textbox('getValue');
            var isShowLocked = $('#IsShowLocked').prop("checked");

            //$('#products').myDatagrid('search', {
            //    OrderID: orderID,
            //    Model: model,
            //    IsShowLocked: isShowLocked,
            //});

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
                OrderID: orderID,
                Model: model,
                IsShowLocked: isShowLocked,
            });

            currentSc.PageNumber = $('#products').datagrid('options').pageNumber;
            currentSc.PageSize = $('#products').datagrid('options').pageSize;
            currentSc.IsShowLocked = isShowLocked;
            currentSc.Model = model;
            currentSc.OrderID = orderID;
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
            $('#IsShowLocked').prop('checked', false);
            Search();
        }
        //批量锁定
        function batchLock() {
            var gridData = $('#products').datagrid('getChecked');
            var tobeLockedItems = new Array();
            for (var i = 0; i < gridData.length; i++) {
                var item = {};
                item['ItemId'] = gridData[i].ID;
                item['PartNumber'] = gridData[i].PartNumber;
                tobeLockedItems[i] = item;
            }
            if (tobeLockedItems.length == 0) {
                $.messager.alert('提示', '请至少选择一个需要锁定的产品！');
                return;
            }
            MaskUtil.mask();
            var step = '<%=Needs.Ccs.Services.Enums.ClassifyStep.Step1.GetHashCode()%>';
            postDataFun(domainUrls.PvDataApiUrl + 'Classify/BatchLock', { tobeLockedItems: JSON.stringify(tobeLockedItems), creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
                success: function (res) {
                    MaskUtil.unmask();
                    $.messager.alert('提示', res.data);
                    $('#products').datagrid('reload');
                },
                exceptionFun: function (res) {
                    MaskUtil.unmask();
                    $.messager.alert('提示', res.data);
                }
            });
        }


        //产品归类
        function Classify(index) {
            //归类锁定
            $('#products').datagrid('selectRow', index);
            var data = $('#products').datagrid('getSelected');
            if (data) {
                MaskUtil.mask();
                $.post('?action=ValidateClassify', { itemId: data.ID }, function (result) {
                    MaskUtil.unmask();
                    if (result.IsCanClassify) {
                        MaskUtil.mask();
                        $.post('?action=GetOrderInfos', { orderId: data.OrderID }, function (infos) {
                            MaskUtil.unmask();
                            var step =<%=Needs.Ccs.Services.Enums.ClassifyStep.Step1.GetHashCode()%>;
                            postDataFun(domainUrls.PvDataApiUrl + 'Classify/Lock', { itemId: data.ID, creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
                                noDataFun: function (res) {
                                    MaskUtil.unmask();
                                    $.messager.alert('提示', res.data);
                                },
                                success: function (res) {
                                    MaskUtil.unmask();
                                    var data2 = {};
                                    for (var k in data) {
                                        data2[k] = data[k];
                                    }
                                    data2['PIs'] = infos.PIs;
                                    data2['SpecialType'] = infos.SpecialType;
                                    data2['CallBackUrl'] = domainUrls.CallBackUrl;
                                    data2['PvDataApiUrl'] = domainUrls.PvDataApiUrl;
                                    data2['NextUrl'] = domainUrls.NextUrl;
                                    data2['Step'] = step;
                                    data2['CreatorID'] = admin.ID;
                                    data2['CreatorName'] = admin.RealName;
                                    data2['Role'] = admin.Role;
                                    data2['SetWindow'] = setWindow;
                                    doClassify(data2, {
                                        xdt: true
                                    });
                                },
                                exceptionFun: function (res) {
                                    MaskUtil.unmask();
                                    $.messager.alert('提示', res.data);
                                }
                            });
                        })
                    } else {
                        $.messager.alert('提示', result.Message);
                        $('#products').datagrid('reload');
                    }
                })
            }
        }

        function doClassify(data, otherOptions) {
            $.classifyAjax.conts.openUrl = '/PvData/Classify/Edit.html'
            $.classifyAjax(data, {
                onClose: function () {
                    Search(true);
                }
            }, otherOptions);
        }

        //解除归类锁定
        function UnLock(index) {
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            var step = '<%=Needs.Ccs.Services.Enums.ClassifyStep.Step1.GetHashCode()%>';
            if (rowdata) {
                MaskUtil.mask();
                postDataFun(domainUrls.PvDataApiUrl + 'Classify/UnLock', { itemId: rowdata.ID, creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
                    success: function (res) {
                        MaskUtil.unmask();
                        $.messager.alert('提示', res.data);
                        $('#products').datagrid('reload');
                    },
                    exceptionFun: function (res) {
                        MaskUtil.unmask();
                        $.messager.alert('提示', res.data);
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
            var buttons = '';
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
            <a id="btnBatchLock" class="easyui-linkbutton" data-options="iconCls:'icon-lock'" style="margin-left: 10px;" onclick="batchLock()">批量锁定</a>
            <input type="checkbox" id="IsShowLocked" name="IsShowLocked" checked="checked" class="checkbox" /><label for="IsShowLocked" style="margin-left: 15px;">显示全部</label>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">产品型号: </span>
                    <input class="easyui-textbox" id="Model" data-options="validType:'length[1,50]'" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 6px;" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="待归类产品" data-options="nowrap:false,border:false,fitColumns:false,fit:true,singleSelect:false,toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'LockStatus',align:'center'" style="width: 5%;">锁定状态</th>
                    <th data-options="field:'Locker',align:'center'" style="width: 6%;">锁定人</th>
                    <th data-options="field:'LockTime',align:'left'" style="width: 10%;">锁定时间</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 10%;">创建时间</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 5%;">跟单员</th>
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 12%;">操作</th>
                    <th data-options="field:'Name',align:'left'" style="width: 10%;">品名</th>
                    <th data-options="field:'PartNumber',align:'left'" style="width: 12%;">产品型号</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 8%;">品牌</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 5%;">原产地</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 6%;">申报数量</th>
                    <th data-options="field:'Unit',align:'center'" style="width: 5%;">申报单位</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 5%;">单价</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
