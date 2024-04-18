<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SecondList.aspx.cs" Inherits="WebApp.Classify.PreProduct.SecondList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预处理二</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
<%--    <script>
        gvSettings.fatherMenu = '产品预归类(XDT)';
        gvSettings.menu = '预处理二';
        gvSettings.summary = '二次归类';
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
            var ProductName = $('#ProductName').textbox('getValue');
            //var TaxCode = $('#TaxCode').textbox('getValue');
            var HSCode = $('#HSCode').textbox('getValue');
            var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
            var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');
            var isShowLocked = $('#IsShowLocked').prop("checked");

            //$('#products').myDatagrid('search', {
            //    Model: Model,
            //    ProductName: ProductName,
            //    //TaxCode: TaxCode,
            //    HSCode: HSCode,
            //    LastClassifyTimeBegin: LastClassifyTimeBegin,
            //    LastClassifyTimeEnd: LastClassifyTimeEnd,
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
                ProductName: ProductName,
                HSCode: HSCode,
                LastClassifyTimeBegin: LastClassifyTimeBegin,
                LastClassifyTimeEnd: LastClassifyTimeEnd,
                IsShowLocked: isShowLocked,
            });

            currentSc.PageNumber = $('#products').datagrid('options').pageNumber;
            currentSc.PageSize = $('#products').datagrid('options').pageSize;
            currentSc.IsShowLocked = isShowLocked;
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
            //$('#TaxCode').textbox('setValue', null);
            $('#HSCode').textbox('setValue', null);
            $('#LastClassifyTimeBegin').textbox('setValue', null);
            $('#LastClassifyTimeEnd').textbox('setValue', null);
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
                        var url = location.pathname.replace(/SecondList.aspx/ig, 'Edit.aspx') + '?ID=' + rowdata.ID + '&PreProductID=' + rowdata.ID + '&From=<%=Needs.Ccs.Services.Enums.ClassifyStep.PreStep2.GetHashCode()%>&SourcePage=Add'
                            + "&" + parseParams(currentSc);
                        window.location = url;
                    } else {
                        $.messager.alert('提示', result.message);
                        $('#products').datagrid('reload');
                    }
                });

            }
        }

        function QuickClassify() {
            var docData = $('#products').datagrid('getChecked');
            var arr = new Array();
            for (var i = 0; i < docData.length; i++) {              
                if (docData[i].TaxCode == null || docData[i].TaxCode == "" || docData[i].TaxName == null || docData[i].TaxName == "") {
                    $.messager.alert('提示', docData[i].Model + '税务信息未归类,不能一键归类！');
                    return;
                }
                else if (docData[i].CompanyType == "A类") {
                     $.messager.alert('提示', docData[i].Model + '为A类订单型号,不能一键归类！');
                    return;
                }
                else {
                    arr[i] = docData[i].ID;
                }
            }

            if (arr.length != 0) {
                var jsonString = JSON.stringify(arr);
                $.messager.confirm('确认', '确认产品归类信息无误，完成归类？', function (success) {
                    if (success) {
                        MaskUtil.mask();
                        $.post('?action=QuickClassify', { IDs: jsonString }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                $.messager.alert('提示', result.message, 'info', function () {
                                    $('#products').datagrid('reload');
                                });
                            } else {
                                $.messager.alert('提示', result.message);
                            }
                        })
                    }
                });
            } else {
                $.messager.alert('提示', '请至少选择一个需要完成归类的产品！');
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
            <a id="btnClassify" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="QuickClassify()">一键归类</a>
            <a id="btnBatchLock" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-lock'" onclick="BatchLock()" style="margin-left: 10px;">批量锁定</a>
            <input type="checkbox" id="IsShowLocked" name="IsShowLocked" checked="checked" class="checkbox" /><label for="IsShowLocked" style="margin-left: 15px;">显示全部</label>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl" style="margin-left: 34px;">型号: </span>
                    <input class="easyui-textbox" id="Model" data-options="validType:'length[1,50]'" />
                    <span class="lbl">品名: </span>
                    <input class="easyui-textbox" id="ProductName" data-options="validType:'length[1,50]'" />
                    <%--<span class="lbl">税号: </span>
                    <input class="easyui-textbox" id="TaxCode" data-options="validType:'length[1,50]'" />--%>
                    <span class="lbl">海关编码: </span>
                    <input class="easyui-textbox" id="HSCode" data-options="validType:'length[1,50]'" />
                </li>
                <li>
                    <span class="lbl">归类时间: </span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeBegin" />
                    <span class="lbl" style="margin-left: 24px;">至 </span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeEnd" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()" style="margin-left: 10px;">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="待归类产品" data-options="nowrap:false,border:false,fitColumns:false,fit:true,toolbar:'#topBar',singleSelect:false,">
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
                    <th data-options="field:'DueDate',align:'center'" style="width: 150px;">预计到货日期</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建时间</th>
                    <th data-options="field:'ClassifyFirstOperatorName',align:'center',width:100,">预处理一人员</th>
                    <th data-options="field:'TaxCode',align:'center',width:150">税务编码</th>
                    <th data-options="field:'TaxName',align:'center',width:150">税务名称</th>
                   <%-- <th data-options="field:'CompanyType',align:'center',width:150," style="display:none">公司类型</th>--%>
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'Btn',align:'left',width:130,formatter:Operation">操作</th>
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

