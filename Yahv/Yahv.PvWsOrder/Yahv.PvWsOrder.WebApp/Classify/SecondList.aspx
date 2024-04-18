<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="SecondList.aspx.cs" Inherits="Yahv.PvData.WebApp.Classify.SecondList" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/classify.ajax.js"></script>
    <script src="../scripts/pvwsorder.js"></script>
    <script>
        var setWindow = 'SecondList_' + Math.floor(Math.random() * 10000);
        $.myWindow.setMyWindow(setWindow, window);
        var admin = eval('(<%=this.Model.Admin%>)');
        var domainUrls = eval('(<%=this.Model.DomainUrls%>)');

        var getQuery = function () {
            var params = {
                action: 'data',
                orderId: $.trim($('#orderId').textbox("getText")),
                partNumber: $.trim($('#partNumber').textbox("getText")),
                name: $.trim($('#name').textbox("getText")),
                hsCode: $.trim($('#hsCode').textbox("getText")),
                startDate: $.trim($('#startDate').textbox("getText")),
                endDate: $.trim($('#endDate').textbox("getText")),
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
                nowrap: false,
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
                return false;
            });

            //一键归类
            $('#btnQuickClassify').click(function () {
                var gridData = $('#dg').datagrid('getChecked');
                var ids = new Array();
                for (var i = 0; i < gridData.length; i++) {
                    if (gridData[i].TaxCode == null || gridData[i].TaxCode == "" || gridData[i].TaxName == null || gridData[i].TaxName == "") {
                        $.messager.alert('提示', '型号【' + gridData[i].PartNumber + '】的税务信息未归类,不能一键归类！');
                        return;
                    }
                    else {
                        ids[i] = gridData[i].ID;
                    }
                }
                if (ids.length == 0) {
                    $.messager.alert('提示', '请至少选择一个需要归类的产品！');
                    return;
                }

                if (ids.length != 0) {
                    $.messager.confirm('确认', '确认产品归类信息无误，完成归类？', function (success) {
                        if (success) {
                            //提交至子系统
                            ajaxLoading();
                            var step = '<%= Yahv.PvWsOrder.Services.Enums.ClassifyStep.Step2.GetHashCode()%>';
                            $.post('?action=QuickClassify', { ids: ids.join() }, function (res) {
                                ajaxLoadEnd();
                                var result = JSON.parse(res);
                                if (result.success) {
                                    $.messager.alert('提示', result.message);
                                    grid.myDatagrid('search', getQuery());

                                    //提交至中心数据
                                    postDataFun(domainUrls.PvDataApiUrl + 'Classify/QuickClassify', { results: result.data }, {
                                        exceptionFun: function (res) {
                                            $.messager.alert('提示', res.data);
                                        }
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
                var step = '<%= Yahv.PvWsOrder.Services.Enums.ClassifyStep.Step2.GetHashCode()%>';
                postDataFun(domainUrls.PvDataApiUrl + 'Classify/BatchLock', { tobeLockedItems: JSON.stringify(tobeLockedItems), creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
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
                var step = '<%= Yahv.PvWsOrder.Services.Enums.ClassifyStep.Step2.GetHashCode()%>';
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
                var step = '<%= Yahv.PvWsOrder.Services.Enums.ClassifyStep.Step2.GetHashCode()%>';
            postDataFun(domainUrls.PvDataApiUrl + 'Classify/UnLock', {
                itemId: itemId, creatorId: admin.ID, creatorName: admin.RealName,
                step: step
            }, {
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
                <td style="width: 90px;">型号</td>
                <td>
                    <input id="partNumber" data-options="prompt:'型号',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">品名</td>
                <td>
                    <input id="name" data-options="prompt:'品名',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">海关编码</td>
                <td>
                    <input id="hsCode" data-options="prompt:'海关编码',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">订单编号</td>
                <td>
                    <input id="orderId" data-options="prompt:'订单编号',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">归类时间</td>
                <td>
                    <input id="startDate" data-options="prompt:'开始时间',validType:'length[1,50]',isKeydown:true" class="easyui-datebox" />
                </td>
                <td style="width: 90px;">至</td>
                <td>
                    <input id="endDate" data-options="prompt:'截止时间',validType:'length[1,50]',isKeydown:true" class="easyui-datebox" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnQuickClassify" class="easyui-linkbutton" data-options="iconCls:'icon-yg-confirm'">一键归类</a>
                    <a id="btnBatchLock" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">批量锁定</a>
                    <input id="isShowLocked" name="isShowLocked" class="easyui-checkbox" value="true" data-options="label:'显示全部',labelPosition:'after',labelAlign:'left'">
                </td>
            </tr>
        </table>
    </div>
    <table id="dg" title="待归类产品">
        <thead>
            <tr>
                <th data-options="field:'Elements',align:'left',width:500">申报要素</th>
                <th data-options="field:'Manufacturer',align:'center',width:80">品牌</th>
                <th data-options="field:'ImportPreferentialTaxRate',align:'center',width:80">优惠税率%</th>
                <th data-options="field:'OriginATRate',align:'center',width:80">加征税率%</th>
                <th data-options="field:'VATRate',align:'center',width:80">增值税率%</th>
                <th data-options="field:'ExciseTaxRate',align:'center',width:80">消费税率%</th>
                <th data-options="field:'Origin',align:'center',width:80">原产地</th>
                <th data-options="field:'Quantity',align:'center',width:80">申报数量</th>
                <th data-options="field:'Unit',align:'center',width:80">申报单位</th>
                <th data-options="field:'LegalUnit1',align:'center',width:80">法定第一单位</th>
                <th data-options="field:'UnitPrice',align:'center',width:80">单价</th>
                <th data-options="field:'Currency',align:'center',width:80">币种</th>
                <th data-options="field:'CIQCode',align:'center',width:100">检验检疫编码</th>
                <th data-options="field:'OrderID',align:'left'" style="width: 150px;">订单编号</th>
                <th data-options="field:'ClientCode',align:'left'" style="width: 70px;">客户入仓号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 200px;">客户名称</th>
                <th data-options="field:'LockStatus',align:'center',width:80">锁定状态</th>
                <th data-options="field:'Locker',align:'center',width:100">锁定人</th>
                <th data-options="field:'LockTime',align:'center',width:100">锁定时间</th>
                <th data-options="field:'CreateDate',align:'center',width:100">创建时间</th>
                <th data-options="field:'ClassifyFirstOperatorName',align:'center',width:100,">预处理一人员</th>
                <th data-options="field:'SpecialType',align:'center',width:150">特殊类型</th>
            </tr>
        </thead>
        <thead data-options="frozen:true">
            <tr>
                <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                <th data-options="field:'Btn',align:'left',width:150,formatter:btn_formatter">操作</th>
                <th data-options="field:'HSCode',align:'center',width:100">HS编码</th>
                <th data-options="field:'TariffName',align:'left',width:120">报关品名</th>
                <th data-options="field:'PartNumber',align:'left',width:150">产品型号</th>
            </tr>
        </thead>
    </table>
</asp:Content>

