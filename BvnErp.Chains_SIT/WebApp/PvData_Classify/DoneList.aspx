<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DoneList.aspx.cs" Inherits="WebApp.PvData_Classify.DoneList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已完成列表</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />
    <script src="http://fix.szhxd.net/frontframe/standard-easyui/scripts/classify.ajax.js"></script>
    <script src="../Scripts/Ccs.js?time=20190910"></script>
    <script src="../Scripts/pvdata.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '产品归类(中心数据)';
        gvSettings.menu = '已完成';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        var setWindow = 'DoneList_' + Math.floor(Math.random()*10000);
        $.myWindow.setMyWindow(setWindow, window);
        var admin = eval('(<%=this.Model.Admin%>)');
        var domainUrls = eval('(<%=this.Model.DomainUrls%>)');

        var initPageNumber = 1;
        var initPageSize = 20;

        $(function () {
            //订单列表初始化
            $('#products').myDatagrid({
                fitColumns: false,
                fit: true,
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
        });

        //查询
        function Search(flag) {
            var orderID = $('#OrderID').textbox('getValue');
            var model = $('#Model').textbox('getValue');
            var manufacturer = $('#Manufacturer').textbox('getValue');
            var ProductName = $('#ProductName').textbox('getValue');
            var HSCode = $('#HSCode').textbox('getValue');
            var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
            var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');

            var opts=$("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");

            if(!flag){
                opts.pageNumber = initPageNumber;
                pager.pagination("refresh",{
                    pageNumber: initPageNumber,
                    pageSize: opts.pageSize,
                });
            }else{
                pager.pagination("refresh",{
                    pageNumber: opts.pageNumber,
                    pageSize: opts.pageSize,
                });
            }

            $('#products').datagrid('reload', {
                action: 'data',
                OrderID: orderID,
                Model: model,
                Manufacturer: manufacturer,
                ProductName: ProductName,
                HSCode: HSCode,
                LastClassifyTimeBegin: LastClassifyTimeBegin,
                LastClassifyTimeEnd: LastClassifyTimeEnd,
            });
        }

        //重置查询条件
        function Reset() {
            var opts=$("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");
            opts.pageNumber = initPageNumber;
            opts.pageSize = initPageSize;
            pager.pagination("refresh",{
                pageNumber: initPageNumber,
                pageSize: initPageSize,
            });

            $('#OrderID').textbox('setValue', null);
            $('#Model').textbox('setValue', null);
            $('#Manufacturer').textbox('setValue', null);
            $('#ProductName').textbox('setValue', null);
            $('#HSCode').textbox('setValue', null);
            $('#LastClassifyTimeBegin').textbox('setValue', null);
            $('#LastClassifyTimeEnd').textbox('setValue', null);
            Search();
        }

        //查看产品归类信息
        function View(index) {
            //归类锁定
            $('#products').datagrid('selectRow', index);
            var data = $('#products').datagrid('getSelected');
            if (data) {
                MaskUtil.mask();
                $.post('?action=GetOrderInfos', { orderId: data.OrderID }, function (infos) {
                    MaskUtil.unmask();
                    var step=<%=Needs.Ccs.Services.Enums.ClassifyStep.Done.GetHashCode()%>;
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
                    doClassify(data2,{
                        xdt:true
                    });
                })
            }
        }
        //重新归类
        function Classify(index) {
            //归类锁定
            $('#products').datagrid('selectRow', index);
            var data = $('#products').datagrid('getSelected');
            if (data) {
                if(data.IsQuoted == true) {
                    $.messager.alert('提示', '该订单已报价！');
                    return;
                }

                MaskUtil.mask();
                $.post('?action=GetOrderInfos', { orderId: data.OrderID }, function (infos) {
                    MaskUtil.unmask();
                    var step=<%=Needs.Ccs.Services.Enums.ClassifyStep.DoneEdit.GetHashCode()%>;
                    postDataFun(domainUrls.PvDataApiUrl+'Classify/Lock', { itemId: data.ID, creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
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
                            doClassify(data2,{
                                xdt:true
                            });
                        },
                        exceptionFun: function (res) {
                            MaskUtil.unmask();
                            $.messager.alert('提示', res.data);
                        }
                    });
                })
            }
        }

        function doClassify(data, otherOptions) {
            $.classifyAjax.conts.openUrl='/PvData/Classify/Edit.html'
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
            var step=<%=Needs.Ccs.Services.Enums.ClassifyStep.DoneEdit.GetHashCode()%>;
                if (rowdata) {
                    MaskUtil.mask();
                    postDataFun(domainUrls.PvDataApiUrl+'Classify/UnLock', { itemId: rowdata.ID, creatorId: admin.ID, creatorName: admin.RealName, step: step }, {
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

            //导出Excel
            function Export() {
                var OrderID = $('#OrderID').textbox('getValue');
                var Model = $('#Model').textbox('getValue');
                var Manufacturer = $('#Manufacturer').textbox('getValue');
                var ProductName = $('#ProductName').textbox('getValue');
                var HSCode = $('#HSCode').textbox('getValue');
                var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
                var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');
                //验证成功
                MaskUtil.mask();
                $.post('?action=Export', {
                    OrderID: OrderID,
                    Model: Model,
                    Manufacturer: Manufacturer,
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
                    buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="UnLock(\'' + index + '\')" group >' +
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
                    <span class="lbl">产品型号: </span>
                    <input class="easyui-textbox" id="Model" data-options="validType:'length[1,50]'" />
                    <span class="lbl">产品品牌: </span>
                    <input class="easyui-textbox" id="Manufacturer" data-options="validType:'length[1,50]'" />
                    <span class="lbl" style="margin-left: 30px;">品名: </span>
                    <input class="easyui-textbox" id="ProductName" data-options="validType:'length[1,50]'" />
                    <span class="lbl">海关编号: </span>
                    <input class="easyui-textbox" id="HSCode" data-options="validType:'length[1,50]'" />
                </li>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">归类时间: </span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeBegin" />
                    <span class="lbl" style="margin: 0 24px;">至</span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeEnd" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" style="margin-left: 6px;" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
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
                    <th data-options="field:'TariffName',align:'left',width:180">报关品名</th>
                    <th data-options="field:'Elements',align:'left',width:500">申报要素</th>
                    <th data-options="field:'PartNumber',align:'center',width:150">产品型号</th>
                    <th data-options="field:'Manufacturer',align:'center',width:80">品牌</th>
                    <th data-options="field:'Origin',align:'center',width:80">原产地</th>
                    <th data-options="field:'Quantity',align:'center',width:80">申报数量</th>
                    <th data-options="field:'Unit',align:'center',width:80">申报单位</th>
                    <th data-options="field:'LegalUnit1',align:'center',width:100">法定第一单位</th>
                    <th data-options="field:'UnitPrice',align:'center',width:80">单价</th>
                    <th data-options="field:'Currency',align:'center',width:80">币种</th>
                    <th data-options="field:'ImportPreferentialTaxRate',align:'center',width:80">关税率%</th>
                    <th data-options="field:'VATRate',align:'center',width:80">增值税率%</th>
                    <th data-options="field:'ExciseTaxRate',align:'center',width:80">消费税率%</th>
                    <th data-options="field:'CIQCode',align:'center',width:80">检验检疫编码</th>
                    <th data-options="field:'ClassifyStatus',align:'center',width:100">归类状态</th>
                    <th data-options="field:'LockStatus',align:'center',width:80">锁定状态</th>
                    <th data-options="field:'Locker',align:'center',width:100">锁定人</th>
                    <th data-options="field:'LockTime',align:'center',width:150">锁定时间</th>
                    <th data-options="field:'CreateDate',align:'center',width:150">创建时间</th>
                    <th data-options="field:'CompleteDate',align:'center',width:150">归类完成时间</th>
                    <th data-options="field:'ClassifyFirstOperatorName',align:'center',width:100">预处理一人员</th>
                    <th data-options="field:'ClassifySecondOperatorName',align:'center',width:100">预处理二人员</th>
                    <th data-options="field:'OrderStatus',align:'center',width:100">订单状态</th>
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'Btn',align:'left',width:200,formatter:Operation">操作</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 150px;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 70px;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 180px;">客户名称</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
