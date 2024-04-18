<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnInvoiceList.aspx.cs" Inherits="WebApp.Finance.UnInvoiceList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待开票</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--    <script>
        gvSettings.fatherMenu = '开票通知';
        gvSettings.menu = '待开票';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">

        var ApplyData = eval('(<%=this.Model.ApplyData%>)');
        var InvoiceTypeData = eval('(<%=this.Model.InvoiceTypeData%>)');

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                rownumbers: true,
                singleSelect: false,
                onCheck: function (index, row) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onUncheck: function (index, row) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onCheckAll: function (rows) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onUncheckAll: function (rows) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onLoadSuccess: function (data) {
                    var totalSum = 0; //总件数
                    var rows = data.rows;
                    for (var i = 0; i < rows.length; i++) {
                        var currentTotal = Number(Number(Number(rows[i].Amount) + Number(rows[i].Difference)).toFixed(2));
                        totalSum += currentTotal;
                    }

                    $("#total-sum").html(totalSum.toFixed(2)); //勾选金额
                },
            });

            //初始化Combobox
            $('#Apply').combobox({
                data: ApplyData,
            })
            $('#InvoiceType').combobox({
                data: InvoiceTypeData,
            })
        });

        //查询
        function Search() {
            var apply = $('#Apply').combobox('getValue');
            var invoiceType = $('#InvoiceType').combobox('getValue');
            var companyName = $('#CompanyName').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var parm = {
                Apply: apply,
                InvoiceType: invoiceType,
                CompanyName: companyName,
                ClientCode: clientCode,
                StartDate: startDate,
                EndDate: endDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Apply').combobox('setValue', null);
            $('#InvoiceType').combobox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#CompanyName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //导出Excel
        function Export() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要导出的信息！');
                return;
            }
            var strIds = "";
            //拼接字符串
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            //验证成功
            $.post('?action=Export', {
                IDs: JSON.stringify(strIds)
            }, function (result) {
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
                        $('#datagrid').myDatagrid('reload');
                    }
                });
            });
        }

        //导出Xml
        function ExportXml() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要导出的开票通知！');
                return;
            }
            var strIds = "";
            //拼接字符串
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            //验证是否报关 start
            MaskUtil.mask();
            $.post('?action=CheckOrderDeclare', {
                IDs: strIds
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                if (rel.success) {
                    //验证成功
                    $.post('?action=ExportXml', {
                        IDs: JSON.stringify(strIds)
                    }, function (result) {
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
                                $('#datagrid').myDatagrid('reload');
                            }
                        });
                    });

                } else {
                    $.messager.alert('消息', rel.message, 'info', function () {

                    });
                }
            });
            //验证 end
        }

        //计算一些求和, 显示在界面上
        function calcSomeSum(rows) {
            var totalSum = 0.00; //
            var count = 0;
            var invoiceQty = 0;
            for (var i = 0; i < rows.length; i++) {
                var currentTotal = Number(Number(Number(rows[i].Amount) + Number(rows[i].Difference)).toFixed(2));
                invoiceQty += Number(rows[i].InvoiceQty);
                totalSum += currentTotal;
                count++;
            }

            $("#check-count").html("勾选 " + count + "单");
            $("#check-sum").html(totalSum.toFixed(2)); //勾选金额           
            $("#check-xml").html(invoiceQty); //勾选发票数量
             
        }

        function ExportMachine() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要导出的开票通知！');
                return;
            }
            var strIds = "";
            //拼接字符串
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

              //验证是否报关 start
            MaskUtil.mask();
            $.post('?action=CheckOrderDeclare', {
                IDs: strIds
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                if (rel.success) {
                    //验证成功                  
                     var url = location.pathname.replace(/UnInvoiceList.aspx/ig, 'XmlMachine.aspx') + '?InvoiceNoticeIDs='+strIds;                           
                       top.$.myWindow({
                                iconCls: "",
                                url: url,
                                noheader: false,
                                title: '查看',
                                width: 1100,
                                height: 650,
                                onClose: function () {
                                    //Search();
                                }
                            });                   
                } else {
                    $.messager.alert('消息', rel.message, 'info', function () {

                    });
                }
            });
            //验证 end
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td colspan="4">
                        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                        <a id="btnExportXml" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportXml()">导出Xml</a>
                        <a id="btnExportMachine" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportMachine()">导入开票机</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">客户编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">公司名称:</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">开票类型:</td>
                    <td>
                        <input class="easyui-combobox" id="InvoiceType" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">申请人:</td>
                    <td>
                        <input class="easyui-combobox" id="Apply" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                    <td class="lbl">申请日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <span id="sum-container" style="margin-left: 55px;color:red;font-weight:600">
                            <label>合计</label>
                            <label style="margin-left: 25px;">本页合计:</label>
                            <label id="total-sum">0</label>
                            <label id="check-count" style="margin-left: 25px;">勾选 0单</label>
                            <label style="margin-left: 25px;">勾选合计:</label>
                            <label id="check-sum">0</label>
                            <label style="margin-left: 25px;">发票数量合计:</label>
                            <label id="check-xml">0</label>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="待开票" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ID',align:'center',sortable:true" style="width: 180px;">开票编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 100px;">客户编号</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 200px;">公司名称</th>
                    <th data-options="field:'InvoiceType',align:'center'" style="width: 100px;">开票类型</th>
                    <th data-options="field:'Amount',align:'center',sortable:true" style="width: 150px;">含税金额</th>
                    <th data-options="field:'Difference',align:'center'" style="width: 90px;">差额</th>
                    <th data-options="field:'InvoiceQty',align:'center'" style="width: 90px;">发票数量</th>
                    <th data-options="field:'DeliveryType',align:'center'" style="width: 90px;">交付方式</th>
                    <th data-options="field:'Status',align:'center'" style="width: 90px;">开票状态</th>
                    <th data-options="field:'ApplyName',align:'center',sortable:true" style="width: 90px;">申请人</th>
                    <th data-options="field:'CreateDate',align:'center',sortable:true" style="width: 150px;">申请日期</th>
                    <%--<th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 50px;">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
