<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomsInvoiceMerchandiser.aspx.cs" Inherits="WebApp.Finance.Declare.CustomsInvoiceMerchandiser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报关单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--   <script>
        gvSettings.fatherMenu = '报关单';
        gvSettings.menu = '报关单';
        gvSettings.summary = '';
    </script>--%>
    <script>

        var InvoiceTypeData = eval('(<%=this.Model.InvoiceTypeData%>)');

        $(function () {
            //订单列表初始化
            $('#decheads').myDatagrid({
                singleSelect: false,
                checkOnSelect: false,
                selectOnCheck: false,
            });

            $('#InvoiceType').combobox({
                data: InvoiceTypeData,
                valueField: 'Key',
                textField: 'Value',
            })

        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var EntryID = $('#EntryID').textbox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var parm = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                EntryID: EntryID,
                InvoiceType: InvoiceType,
                StartDate: StartDate,
                EndDate: EndDate,
                OwnerName: OwnerName,
            };
            $('#decheads').myDatagrid('search', parm);
        }
        //重置
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#EntryID').textbox('setValue', null);
            $('#InvoiceType').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#OwnerName').textbox('setValue', null);
            Search();
        }

        //超期未换汇
        function ViewUnPayExchange(val, row, index) {

                if (val >= 500000) {
                    return "<label style='color:red'>" + val + "</label>";
                }
                return val;
        
        }

        function ExportInvoice(InstanceType, FileType, FileTypeName) {

            var docData = $('#decheads').datagrid('getChecked');

            if (docData.length == 0) {
                $.messager.alert('提示', '请至少选择一个需要下载的报关单！');
                return;
            }

            var clientID = docData[0].ClientID;

            var arr = "";
            for (var i = 0; i < docData.length; i++) {
                //增值税发票
                if (FileType == '18' && docData[i].IsDecHeadVatFile != "未上传") {
                    arr += docData[i].ID + ',';
                }

                //关税发票
                if (FileType == '17' && docData[i].IsDecHeadTariffFile != "未上传") {
                    arr += docData[i].ID + ',';
                }

                //报关单
                if (FileType == '16') {
                    arr += docData[i].ID + ',';
                }

                if (docData[i].ClientID != clientID) {
                    $.messager.alert('提示', "请勾选同一客户的报关单！");
                    return;
                }
            }

            if (arr == "") {
                $.messager.alert('提示', '勾选的报关单没有可下载的发票！');
                return;
            }

     

            var param = {
                DecheadIDs : arr,
                ClientID :clientID,
                UnPayExchangeAmount : docData[0].UnPayExchangeAmount,
                DeclareAmount : docData[0].DeclareAmount,
                PayExchangeAmount: docData[0].PayExchangeAmount,
                DeclareAmountMonth: docData[0].DeclareAmountMonth,
                PayExchangeAmountMonth: docData[0].PayExchangeAmountMonth,
                FileType : FileType,
                FileTypeName : FileTypeName
            };


            $.messager.confirm('确认', '此客户超期未付汇金额：' + docData[0].UnPayExchangeAmount + " 确认下载？", function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=ExportSingleTypeFiles', param, function (res) {
                        var rel = JSON.parse(res);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            MaskUtil.unmask();
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
                    });
                }
            });

        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <form id="form1">
                <table style="margin: 5px 0;">
                    <tr>
                        <td class="lbl">订单编号：</td>
                        <td>
                            <input class="easyui-textbox" id="OrderID" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">合同号：</td>
                        <td>
                            <input class="easyui-textbox" id="ContrNo" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">客户名称：</td>
                        <td>
                            <input class="easyui-textbox" id="OwnerName" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                         <td class="lbl">海关编号：</td>
                        <td>
                            <input class="easyui-textbox" id="EntryID" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        </tr>
                    <tr>
                        <td class="lbl" style="padding-left: 5px">开票类型：</td>
                        <td>
                            <input class="easyui-combobox" id="InvoiceType" name="InvoiceType"
                                data-options="height:26,width:150,editable:false" />
                        </td>

                        <td class="lbl" style="padding-left: 5px">报关开始日期: </td>
                        <td>
                            <input class="easyui-datebox" id="StartDate" data-options="height:26,width:150,editable:false" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">结束日期</td>
                        <td>
                            <input class="easyui-datebox" id="EndDate" data-options="height:26,width:150,editable:false" />
                        </td>
                        <td colspan="2">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </td>
                    </tr>
                     <tr>
                        <td colspan="6" style="padding-bottom: 8px">
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('2','16','报关单')">导出报关单</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('2','18','增值税')">导出增值税发票</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('2','17','关税')">导出关税发票</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="报关单" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'OwnerName',align:'left'" style="width: 12%;">客户名称</th>
                    <th data-options="field:'UnPayExchangeAmount',align:'center',formatter:ViewUnPayExchange" style="width: 6%;">超期未付汇</th>
                    <th data-options="field:'DeclareAmount',align:'center'" style="width: 6%;">近期报关</th>
                    <th data-options="field:'PayExchangeAmount',align:'center'" style="width: 6%;">近期付汇</th>
                    <th data-options="field:'DeclareAmountMonth',align:'center'" style="width: 6%;">近30天报关</th>
                    <th data-options="field:'PayExchangeAmountMonth',align:'center'" style="width: 6%;">近30天付汇</th>
                    <th data-options="field:'ContrNo',align:'left'" style="width: 8%;">合同号</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'SwapAmount',align:'center'" style="width: 6%;">报关金额</th>
                    <th data-options="field:'OrderAgentAmount',align:'center'" style="width: 7%;">委托金额</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 7%;">报关日期</th>
                    <th data-options="field:'InvoiceType',align:'center'" style="width: 7%;">开票类型</th>
                    <th data-options="field:'IsDecHeadTariffFile',align:'center'" style="width: 6%;">关税发票</th>
                    <th data-options="field:'IsDecHeadExciseTaxFile',align:'center'" style="width: 6%;">消费税发票</th>
                    <th data-options="field:'IsDecHeadVatFile',align:'center'" style="width: 6%;">增值税发票</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
