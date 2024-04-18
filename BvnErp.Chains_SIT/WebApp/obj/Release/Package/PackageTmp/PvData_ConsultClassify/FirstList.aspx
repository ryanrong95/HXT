<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FirstList.aspx.cs" Inherits="WebApp.PvData_ConsultClassify.FirstList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预处理一</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/preclassify.ajax.js"></script>
    <script src="../Scripts/Ccs.js?time=20190910"></script>
    <script src="../Scripts/pvdata.js"></script>
    <script>
        //gvSettings.fatherMenu = '咨询归类(中心数据)';
        //gvSettings.menu = '预处理一';
        //gvSettings.summary = '首次归类';
    </script>
    <script type="text/javascript">
        var setWindow = 'preFirstList_' + Math.floor(Math.random() * 10000);
        $.myWindow.setMyWindow(setWindow, window);
        var admin = eval('(<%=this.Model.Admin%>)');
        var domainUrls = eval('(<%=this.Model.DomainUrls%>)');

        var initPageNumber = 1;
        var initPageSize = 20;

        $(function () {
            //待归类产品列表初始化
            $('#products').myDatagrid({
                nowrap: false,
                border: false,
                fitColumns: false,
                fit: true,
                singleSelect: false,
                pageNumber: initPageNumber,
                pageSize: initPageSize,
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
                    $(".datagrid-header-check").html("");//邓媚丹 20230206 隐藏全选框
                    for (var i = 0; i < data.rows.length; i++) {
                        if (!data.rows[i].IsCanClassify) {
                            $('.datagrid-btable').find("input[type='checkbox']")[i].disabled = 'disabled';
                        }
                        if (data.rows[i].ClientName != "北京创新在线电子产品销售有限公司杭州分公司"
                            && data.rows[i].ClientName != "深圳市创芯在线电子商务有限公司"
                            && data.rows[i].ClientName != "北京创新在线电子产品销售有限公司"
                            && data.rows[i].ClientName != "山东创新在线电子商务有限公司"
                            && data.rows[i].ClientName != "北京芯动能科技有限公司"
                            && data.rows[i].ClientName != "深圳市丰掣供应链管理有限公司"
                            && data.rows[i].ClientName != "北京创新在线电子产品销售有限公司深圳分公司") {
                            $("tr[datagrid-row-index=" + i + "]").find("td").css('background', 'bisque');
                            $("tr[datagrid-row-index=" + i + "]").find("td").css('color', 'black');
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
            });

            $("#IsShowLocked").css("display", "none");
            $('#IsShowLocked').prop('checked', false);
            $('#IsShowLocked').change(function () {
                Search();
            });
        });

        //查询
        function Search(flag) {
            var Model = $('#Model').textbox('getValue');
            var CreateDateBegin = $('#CreateDateBegin').textbox('getValue');
            var CreateDateEnd = $('#CreateDateEnd').textbox('getValue');
            var isShowLocked = $('#IsShowLocked').prop("checked");

            var opts = $("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");

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
                Model: Model,
                CreateDateBegin: CreateDateBegin,
                CreateDateEnd: CreateDateEnd,
                IsShowLocked: isShowLocked,
            });
        }

        //重置查询条件
        function Reset() {
            var opts = $("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");
            opts.pageNumber = initPageNumber;
            opts.pageSize = initPageSize;
            pager.pagination("refresh", {
                pageNumber: initPageNumber,
                pageSize: initPageSize,
            });

            $('#Model').textbox('setValue', null);
            $('#CreateDateBegin').textbox('setValue', null);
            $('#CreateDateEnd').textbox('setValue', null);
            $('#IsShowLocked').prop('checked', false);
            Search();
        }

        //批量锁定
        function BatchLock() {
            var gridData = $('#products').datagrid('getChecked');
            var tobeLockedItems = new Array();
            for (var i = 0; i < gridData.length; i++) {
                var item = {};
                item['MainID'] = gridData[i].ID;
                item['PartNumber'] = gridData[i].PartNumber;
                tobeLockedItems[i] = item;
            }
            if (tobeLockedItems.length == 0) {
                $.messager.alert('提示', '请至少选择一个需要锁定的产品！');
                return;
            }
            MaskUtil.mask();
            var step = '<%=Needs.Ccs.Services.Enums.ClassifyStep.PreStep1.GetHashCode()%>';
            postDataFun(domainUrls.PvDataApiUrl + 'PreClassify/BatchLock', { tobeLockedItems: JSON.stringify(tobeLockedItems), creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
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
                        var step =<%=Needs.Ccs.Services.Enums.ClassifyStep.PreStep1.GetHashCode()%>;
                        var useType =<%=Needs.Ccs.Services.Enums.PreProductUserType.Consult.GetHashCode()%>;
                        postDataFun(domainUrls.PvDataApiUrl + 'PreClassify/Lock', { MainID: data.ID, creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
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
                                data2['CallBackUrl'] = domainUrls.CallBackUrl;
                                data2['PvDataApiUrl'] = domainUrls.PvDataApiUrl;
                                data2['NextUrl'] = domainUrls.NextUrl;
                                data2['Step'] = step;
                                data2['UseType'] = useType;
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
                    } else {
                        $.messager.alert('提示', result.Message);
                        $('#products').datagrid('reload');
                    }
                })
            }
        }


        function doClassify(data, otherOptions) {
            $.preclassifyAjax.conts.openUrl = '/PvData/PreClassify/Edit.html'
            $.preclassifyAjax(data, {
                onClose: function () {
                    Search(true);
                }
            }, otherOptions);
        }

        //退回
         function Return(index) {
            $('#products').datagrid('selectRow', index);
            var data = $('#products').datagrid('getSelected');
            if (data) {
                $("#approve-tip").show();         
                $('#approve-dialog').dialog({
                title: '提示',
                width: 450,
                height: 280,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var reason = $("#AdditionSummary").textbox('getValue');
                        reason = reason.trim();
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Return', {
                            mainId: data.ID,
                            Reason: reason,                           
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    document.location.reload();
                                    $('#products').datagrid('reload');
                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        NormalClose();
                                    }
                                });
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {

                                });
                            }
                        });

                    }
                }, {
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });
                $('#approve-dialog').window('center');
            }           
        }

        //删除
        function Delete(index) {
            $('#products').datagrid('selectRow', index);
            var data = $('#products').datagrid('getSelected');
            $.post('?action=DeleteCheck', { mainId: data.ProductUnionCode }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.confirm('确认', '是否删除该型号？', function (success) {
                        if (success) {
                            MaskUtil.mask();
                            $.post('?action=Delete', { mainId: data.ProductUnionCode }, function (res) {
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

                } else {
                    $.messager.alert('提示', result.message);
                }
            });
        }

        //解除归类锁定
        function UnLock(index) {
            $('#products').datagrid('selectRow', index);
            var rowdata = $('#products').datagrid('getSelected');
            var step = '<%=Needs.Ccs.Services.Enums.ClassifyStep.PreStep1.GetHashCode()%>';
            if (rowdata) {
                MaskUtil.mask();
                postDataFun(domainUrls.PvDataApiUrl + 'PreClassify/UnLock', { MainID: rowdata.ID, creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
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

        //列表框按钮加载
        function AnomalyOperation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Return(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">退回</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-cancel">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

         //整行关闭一系列弹框
        function NormalClose() {
            $('#approve-dialog').window('close');
            $.myWindow.close();
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
                    <span class="lbl">创建时间: </span>
                    <input class="easyui-datebox" data-options="" id="CreateDateBegin" />
                    <span class="lbl" style="margin: 0 12px;">至</span>
                    <input class="easyui-datebox" data-options="" id="CreateDateEnd" />
                </li>
                <li>
                    <span class="lbl">产品型号: </span>
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
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 150px;">操作</th>
                    <th data-options="field:'Name',align:'left'" style="width: 6%;">品名</th>
                    <th data-options="field:'ClientName',align:'left',sortable:true" style="width: 10%;">客户名称</th>
                    <th data-options="field:'PartNumber',align:'left'" style="width: 14%;">产品型号</th>
                    <th data-options="field:'Manufacturer',align:'left'" style="width: 8%;">品牌</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 6%;">单价</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 6%;">申报数量</th>
                    <th data-options="field:'ClassifyStatus',align:'center'" style="width: 6%;">归类状态</th>
                    <th data-options="field:'LockStatus',align:'center'" style="width: 6%;">锁定状态</th>
                    <th data-options="field:'Locker',align:'center'" style="width: 7%;">锁定人</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 5%;">跟单员</th>
                    <th data-options="field:'LockTime',align:'center'" style="width: 150px;">锁定时间</th>
                    <th data-options="field:'CreateDate',align:'center',sortable:true" style="width: 150px;">创建时间</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 6%;">客户编号</th>
                    <%--<th data-options="field:'ClientName',align:'left'" style="width: 10%;">客户名称</th>--%>
                    <th data-options="field:'RegisterName',align:'left'" style="width: 10%;">录入人</th>
                    <th data-options="field:'IcgooAdminName',align:'left'" style="width: 10%;">Icgoo录入人</th>
                    <th data-options="field:'AnomalyBtn',align:'left',width:150,formatter:AnomalyOperation">异常操作</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form id="form3">
            <div id="approve-tip" style="padding: 15px; display: none;">
                <div>
                    <label>备注：</label>
                </div>
                <div style="margin-top: 3px;">
                    <input id="AdditionSummary" class="easyui-textbox" data-options="multiline:true," style="width: 300px; height: 62px;" />
                </div>
                <label style="font-size: 14px;">退回后该型号将不能申报，确定退回？</label>
            </div>
        </form>
    </div>
</body>
</html>
