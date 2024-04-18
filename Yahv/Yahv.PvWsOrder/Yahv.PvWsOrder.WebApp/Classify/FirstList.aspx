<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="FirstList.aspx.cs" Inherits="Yahv.PvData.WebApp.Classify.FirstList" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/classify.ajax.js"></script>
    <script src="../scripts/pvwsorder.js"></script>
    <script>
        var setWindow = 'FirstList_' + Math.floor(Math.random()*10000);
        $.myWindow.setMyWindow(setWindow, window);
        var admin = eval('(<%=this.Model.Admin%>)');
        var domainUrls = eval('(<%=this.Model.DomainUrls%>)');
        var getQuery = function () {
            var params = {
                action: 'data',
                orderId: $.trim($('#orderId').textbox("getText")),
                partNumber: $.trim($('#partNumber').textbox("getText")),
                isShowLocked: $('#isShowLocked').checkbox('options').checked
            };
            return params;
        };

        function btn_formatter(val, row, index) {
            var btns = [];
            btns.push('<span class="easyui-formatted">');
            if (row.IsCanClassify) {
                btns.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="classify(' + index + ');return false;">归类</a> ');
            }
            if (row.IsCanUnlock) {
                btns.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-enabled\'" onclick="unLock(\'' + row.ItemID + '\');return false;">解锁</a> ');
            }
            btns.push('</span>');
            return btns.join('');
        }

        $(function () {
            window.grid = $("#dg").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: false,
                fitColumns: false,
                onLoadSuccess: function (data) {
                    for (var i = 0; i < data.rows.length; i++) {
                        if (!data.rows[i].IsCanClassify) {
                            $('.datagrid-btable').find("input[type='checkbox']")[i].disabled = 'disabled';
                        }
                    }
                },
                onSelect: function (index, row) {
                    if (!row.IsCanClassify) {
                        $('#dg').datagrid('unselectRow', index);

                        if (IsCheckAll()) {
                            $('.datagrid-header-check').find("input[type='checkbox']")[0].checked = true;
                        }
                    }
                },
                onCheck: function (index, row) {
                    if (IsCheckAll()) {
                        $('.datagrid-header-check').find("input[type='checkbox']")[0].checked = true;
                    }
                },
                onCheckAll: function (rows) {
                    for (var index = 0; index < rows.length; index++) {
                        var row = rows[index];
                        if (!row.IsCanClassify) {
                            $('#dg').datagrid('unselectRow', index);
                        }
                    }
                    $('.datagrid-header-check').find("input[type='checkbox']")[0].checked = true;
                },
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                location.reload();
            });

            //批量锁定
            $('#btnBatchLock').click(function () {
                var gridData = $('#dg').datagrid('getChecked');
                var tobeLockedItems = new Array();
                for (var i = 0; i < gridData.length; i++) {
                    var item = {};
                    item['ItemId'] = gridData[i].ItemID;
                    item['PartNumber'] = gridData[i].PartNumber;
                    tobeLockedItems[i] = item;
                }
                if (tobeLockedItems.length == 0) {
                    $.messager.alert('提示', '请至少选择一个需要锁定的产品！');
                    return;
                }

                ajaxLoading();
                var step = '<%= Yahv.PvWsOrder.Services.Enums.ClassifyStep.Step1.GetHashCode()%>';
                postDataFun(domainUrls.PvDataApiUrl+'Classify/BatchLock', { tobeLockedItems: JSON.stringify(tobeLockedItems), creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
                    success: function (res) {
                        ajaxLoadEnd();
                        $.messager.alert('提示', res.data);
                        grid.myDatagrid('search', getQuery());
                    },
                    exceptionFun: function (res) {
                        ajaxLoadEnd();
                        $.messager.alert('提示', res.data);
                    }
                });
            });

            //显示被其他报关员锁定的产品型号
            $("#isShowLocked").checkbox({
                onChange: function (record) {
                    grid.myDatagrid('search', getQuery());
                }
            });
        });

        //归类
        function classify(index) {
            ajaxLoading();
            var data = $("#dg").myDatagrid('getRows')[index];
            $.post('?action=GetOrderInfos', { orderId: data.OrderID }, function (infos) {
                var step = '<%= Yahv.PvWsOrder.Services.Enums.ClassifyStep.Step1.GetHashCode()%>';
                postDataFun(domainUrls.PvDataApiUrl + 'Classify/Lock', { itemId: data.ItemID, creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
                    noDataFun: function (res) {
                        ajaxLoadEnd();
                        $.messager.alert('提示', res.data);
                    },
                    success: function (res) {
                        ajaxLoadEnd();
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
                        data2['SetWindow'] = setWindow;//将本页面的window传递给编辑页面
                        doClassify(data2);
                    },
                    exceptionFun: function (res) {
                        ajaxLoadEnd();
                        $.messager.alert('提示', res.data);
                    }
                });
            });
        }

        function doClassify(data, otherOptions) {
            $.classifyAjax(data, {
                onClose: function () {
                    grid.myDatagrid('reload', getQuery());
                }
            }, otherOptions);
        }

        //解锁
        function unLock(itemId) {
            ajaxLoading();
            var step = '<%= Yahv.PvWsOrder.Services.Enums.ClassifyStep.Step1.GetHashCode()%>';
            postDataFun(domainUrls.PvDataApiUrl + 'Classify/UnLock', { itemId: itemId, creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
                success: function (res) {
                    ajaxLoadEnd();
                    $.messager.alert('提示', res.data);
                    grid.myDatagrid('search', getQuery());
                },
                exceptionFun: function (res) {
                    ajaxLoadEnd();
                    $.messager.alert('提示', res.data);
                }
            });
        }

        //是否勾选全选框
        function IsCheckAll() {
            var isCheckAll = true;
            var rows = $('#dg').datagrid('getRows');
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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">订单编号</td>
                <td>
                    <input id="orderId" data-options="prompt:'订单编号',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">型号</td>
                <td>
                    <input id="partNumber" data-options="prompt:'型号',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnBatchLock" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">批量锁定</a>
                    <input id="isShowLocked" name="isShowLocked" class="easyui-checkbox" value="true" data-options="label:'显示全部',labelPosition:'after',labelAlign:'left'">
                </td>
            </tr>
        </table>
    </div>
    <table id="dg" title="待归类产品">
        <thead>
            <tr>
                <th data-options="field:'OrderID',align:'left'" style="width: 10%;">订单编号</th>
                <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                <th data-options="field:'LockStatus',align:'center'" style="width: 5%;">锁定状态</th>
                <th data-options="field:'Locker',align:'center'" style="width: 6%;">锁定人</th>
                <th data-options="field:'LockTime',align:'left'" style="width: 10%;">锁定时间</th>
                <th data-options="field:'CreateDate',align:'left'" style="width: 10%;">创建时间</th>
                <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户入仓号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>

            </tr>
        </thead>
        <thead data-options="frozen:true">
            <tr>
                <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                <th data-options="field:'Btn',align:'left',formatter:btn_formatter" style="width: 12%;">操作</th>
                <%--<th data-options="field:'Name',align:'left'" style="width: 10%;">品名</th>--%>
                <th data-options="field:'PartNumber',align:'left'" style="width: 12%;">产品型号</th>
                <th data-options="field:'Manufacturer',align:'center'" style="width: 8%;">品牌</th>
                <th data-options="field:'Origin',align:'center'" style="width: 5%;">原产地</th>
                <th data-options="field:'Quantity',align:'center'" style="width: 6%;">申报数量</th>
                <th data-options="field:'Unit',align:'center'" style="width: 5%;">申报单位</th>
                <th data-options="field:'UnitPrice',align:'center'" style="width: 5%;">单价</th>
            </tr>
        </thead>
    </table>
</asp:Content>
