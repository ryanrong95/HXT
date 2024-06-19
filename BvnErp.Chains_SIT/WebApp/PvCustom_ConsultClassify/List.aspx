<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.PvCustom_ConsultClassify.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已完成</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />
    <script src="http://fix.szhxd.net/frontframe/standard-easyui/scripts/preclassify.ajax.js"></script>
    <script src="../Scripts/Ccs.js?time=20190910"></script>
    <script src="../Scripts/pvdata.js"></script>

    
    <script type="text/javascript">
        var setWindow = 'preDoneList_' + Math.floor(Math.random() * 10000);
        $.myWindow.setMyWindow(setWindow, window);
        var admin = eval('(<%=this.Model.Admin%>)');
        var domainUrls = eval('(<%=this.Model.DomainUrls%>)');
        var ClassifyStatus = eval('(<%=this.Model.ClassifyStatus%>)');

        var initPageNumber = 1;
        var initPageSize = 20;

        $(function () {
             //下拉框数据初始化
            
            $('#ClassifyStatus').combobox({
                data: ClassifyStatus
            });

            //待归类产品列表初始化
            $('#products').myDatagrid({
                nowrap: false,
                border: false,
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
            });
        });

        //查询
        function Search(flag) {
            var Model = $('#Model').textbox('getValue');
            var manufacturer = $('#Manufacturer').textbox('getValue');
            var ProductName = $('#ProductName').textbox('getValue');
            var HSCode = $('#HSCode').textbox('getValue');
            var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
            var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');
            var IsCCC = $('#IsCCC').prop('checked');
            var IsForbidden = $('#IsForbidden').prop('checked');

            var opts = $("#products").datagrid("options");
            var pager = $("#products").datagrid("getPager");
            var classifyStatus = $('#ClassifyStatus').combobox('getValue');

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
                Manufacturer: manufacturer,
                ProductName: ProductName,
                HSCode: HSCode,
                LastClassifyTimeBegin: LastClassifyTimeBegin,
                LastClassifyTimeEnd: LastClassifyTimeEnd,
                IsCCC: IsCCC,
                IsForbidden: IsForbidden,
                ClassifyStatus:classifyStatus
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
            $('#Manufacturer').textbox('setValue', null);
            $('#ProductName').textbox('setValue', null);
            $('#HSCode').textbox('setValue', null);
            $('#LastClassifyTimeBegin').textbox('setValue', null);
            $('#LastClassifyTimeEnd').textbox('setValue', null);
            $('#IsCCC').prop('checked', false);
            $('#IsForbidden').prop('checked', false);
            $('#ClassifyStatus').combobox('setValue', null);
            Search();
        }

        //订单归类
        function View(index) {
            //归类锁定
            $('#products').datagrid('selectRow', index);
            var data = $('#products').datagrid('getSelected');
            if (data) {
                MaskUtil.mask();
                $.post('?action=GetOrderInfos', { productUnionCode: data.ProductUnionCode }, function (infos) {
                    MaskUtil.unmask();
                    var step =<%=Needs.Ccs.Services.Enums.ClassifyStep.PreDone.GetHashCode()%>;
                    var useType =<%=Needs.Ccs.Services.Enums.PreProductUserType.Consult.GetHashCode()%>;
                    var data2 = {};
                    for (var k in data) {
                        data2[k] = data[k];
                    }
                    data2['PIs'] = infos.PIs;
                    data2['Supplier'] = infos.Supplier;
                    data2['CallBackUrl'] = domainUrls.CallBackUrl;
                    data2['PvDataApiUrl'] = domainUrls.PvDataApiUrl;
                    data2['NextUrl'] = domainUrls.NextUrl;
                    data2['Step'] = step;
                    data2['UseType'] = useType;
                    data2['CreatorID'] = admin.ID;
                    data2['CreatorName'] = admin.RealName;
                    data2['Role'] = admin.Role;
                    data2['Source'] = '采购咨询查看';
                    doClassify(data2, {
                        xdt: true
                    });
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

 
        //导出Excel
        function Export() {
            var Model = $('#Model').textbox('getValue');
            var Manufacturer = $('#Manufacturer').textbox('getValue');
            var ProductName = $('#ProductName').textbox('getValue');
            var HSCode = $('#HSCode').textbox('getValue');
            var LastClassifyTimeBegin = $('#LastClassifyTimeBegin').textbox('getValue');
            var LastClassifyTimeEnd = $('#LastClassifyTimeEnd').textbox('getValue');
            var classifyStatus = $('#ClassifyStatus').combobox('getValue');
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
                Model: Model,
                Manufacturer: Manufacturer,
                ProductName: ProductName,
                HSCode: HSCode,
                LastClassifyTimeBegin: LastClassifyTimeBegin,
                LastClassifyTimeEnd: LastClassifyTimeEnd,
                ClassifyStatus:classifyStatus
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

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

         //整行关闭一系列弹框
        function NormalClose() {
            $('#approve-dialog').window('close');
            $.myWindow.close();
        }

          //新增费用
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                noheader: false,
                title: '新增咨询产品',
                width:500,
                height: 400,
                overflow:"hidden",
                onClose: function () {
                    $('#products').datagrid('reload');
                }
            });
        }
    </script>
    <style>
        .spacialTip {
            background-color: #EA4335;
            color: white;
            font-size: 12px;
            padding: 0px 4px;
            border-radius: 3px;
        }

        .checkbox {
            position: absolute !important;
        }
    </style>
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
                </li>
                <li>
                    <span class="lbl" style="margin:0 19px;">品名: </span>
                    <input class="easyui-textbox" id="ProductName" data-options="validType:'length[1,50]'" />
                    <span class="lbl">海关编码: </span>
                    <input class="easyui-textbox" id="HSCode" data-options="validType:'length[1,50]'" />
                </li>
                <li>
                    <span class="lbl">归类时间: </span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeBegin" />
                    <span class="lbl" style="margin: 0 27px;">至</span>
                    <input class="easyui-datebox" data-options="" id="LastClassifyTimeEnd" />
                    <span class="lbl">归类状态: </span>
                    <input class="easyui-combobox" id="ClassifyStatus" data-options="valueField:'Key',textField:'Value',editable:false" style="width: 150px;" />
                </li>
                <li>
                    <input type="checkbox" id="IsCCC" name="IsCCC" class="checkbox" /><label for="IsCCC" style="margin-right: 20px">CCC认证</label>
                    <input type="checkbox" id="IsForbidden" name="IsForbidden" class="checkbox" /><label for="IsForbidden" style="margin-right: 20px">禁运</label>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()" style="margin-left: 6px;">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <%--<a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>--%>
                    <%--<a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增产品</a>--%>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="咨询产品" data-options="nowrap:false,border:false,fitColumns:false,fit:true,toolbar:'#topBar',">
            <thead>
                <tr>
                    <%--<th data-options="field:'Elements',align:'left',width:450">申报要素</th>--%>
                    <th data-options="field:'ImportPreferentialTaxRate',align:'center',width:100">关税率%</th>
                    <th data-options="field:'UnitPrice',align:'center',width:100">单价</th>
                    <th data-options="field:'Currency',align:'center',width:100">币种</th>
                    <th data-options="field:'Quantity',align:'center',width:100">申报数量</th>
                    <%--<th data-options="field:'LegalUnit1',align:'center',width:100">法定第一单位</th>--%>
                    <%--<th data-options="field:'LegalUnit2',align:'center',width:100">法定第二单位</th>--%>
                    <%--<th data-options="field:'VATRate',align:'center',width:100">增值税率%</th>--%>
                    <%--<th data-options="field:'ExciseTaxRate',align:'center',width:100">消费税率%</th>--%>
                    <%--<th data-options="field:'CIQCode',align:'center',width:100">检验检疫编码</th>--%>                    
                    <th data-options="field:'ClientCode',align:'center'" style="width: 70px;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 200px;">客户名称</th>                   
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建时间</th>
                    <th data-options="field:'CompleteDate',align:'center',width:150">归类完成时间</th>
                    <th data-options="field:'ClassifyFirstOperatorName',align:'center',width:100">预处理一人员</th>
                    <th data-options="field:'ClassifySecondOperatorName',align:'center',width:100">预处理二人员</th>
                    <%--<th data-options="field:'TaxCode',align:'center',width:150">税务编码</th>--%>                             
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'Btn',align:'left',width:100,formatter:Operation">操作</th>
                    <th data-options="field:'ClassifyStatus',align:'center',width:100">归类状态</th>
                    <th data-options="field:'Manufacturer',align:'center',width:100">品牌</th>
                    <th data-options="field:'PartNumber',align:'left',width:150">产品型号</th>
                    <th data-options="field:'TaxName',align:'center',width:150">税务名称</th>                    
                    <th data-options="field:'SpecialType',align:'center',width:150">特殊类型</th>        
                    <%--<th data-options="field:'TariffName',align:'left',width:130">报关品名</th>--%>
                    <%--<th data-options="field:'HSCode',align:'center',width:100">HS编码</th>--%>
                </tr>
            </thead>
        </table>
    </div>    
</body>
</html>
