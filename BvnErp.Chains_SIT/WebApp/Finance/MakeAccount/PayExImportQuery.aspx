<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayExImportQuery.aspx.cs" Inherits="WebApp.Finance.MakeAccount.PayExImportQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已换汇</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
  
    <script type="text/javascript">

        var BankData = eval('(<%=this.Model.BankData%>)');

        $(function () {
            //列表初始化
            $('#datagrid').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

            $("#Bank").combobox({
                data: BankData,
                valueField: 'value',
                textField: 'text',
            })
        });

        //查询
        function Search() {
            var BankName = $('#Bank').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var ContrNo = $('#ContrNo').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');

            var parm = {
                BankName: BankName,
                StartDate: StartDate,
                EndDate: EndDate,
                ContrNo: ContrNo,
                EntryId: EntryId,
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Bank').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#ContrNo').textbox('setValue', null);
            $('#EntryId').textbox('setValue', null);
            Search();
        }
      
        //导出换汇做账报表
        function ExportFinanceReport() {
            var BankName = $('#Bank').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var ContrNo = $('#ContrNo').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');


            if (!(StartDate != "" || EndDate != "" || ContrNo != "" || BankName != "")) {
                $.messager.alert({ title: '提示', msg: "请至少选择一种筛选条件", icon: 'info', top: 300 });
                return;
            }

            var parm = {
                BankName: BankName,
                StartDate: StartDate,
                EndDate: EndDate,
                ContrNo: ContrNo,
                EntryId: EntryId,
            };

            MaskUtil.mask();
            $.post('?action=ExportSwapExcel', parm, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                    let a = document.createElement('a');
                    document.body.appendChild(a);
                    a.href = result.url;
                    a.download = "";
                    a.click();
                } else {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                }
            })
        }

        //导出Excel
        function ExportExcel() {
            var BankName = $('#Bank').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var ContrNo = $('#ContrNo').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');

            var param = {
                BankName: BankName,
                StartDate: StartDate,
                EndDate: EndDate,
                ContrNo: ContrNo,
                EntryId: EntryId,
            };

            //验证成功
            MaskUtil.mask();
            $.post('?action=ExportExcel', param, function (result) {
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
            });
        }
      
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">合同号: </td>
                    <td>
                        <input class="easyui-textbox" id="ContrNo" data-options="height:26,width:150,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">报关号: </td>
                    <td>
                        <input class="easyui-textbox" id="EntryId" data-options="height:26,width:150,validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">换汇银行: </td>
                    <td>
                        <input class="easyui-combobox" id="Bank" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">申请日期: </td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:150,editable:false" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:150,editable:false" />
                    </td>
                    <td colspan="2">
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnExportExcel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()" style="margin-left: 15px;">导出Excel</a>
                        <a id="btnExportExcel1" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportFinanceReport()" style="margin-left: 15px;">导出做账</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="换汇" data-options="fitColumns:true,fit:true,rownumbers:true,singleSelect:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'RequestID',align:'center'" style="width: 12%;">申请ID</th>
                    <th data-options="field:'ID',align:'center'" style="width: 10%;">换汇编号</th>
                    <th data-options="field:'Creator',align:'center'" style="width: 5%;">申请人</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 4%;">币种</th>
                    <th data-options="field:'TotalAmount',align:'left'" style="width: 6%;">换汇金额</th>
                    <th data-options="field:'TotalAmountCNY',align:'left'" style="width: 6%;">换汇金额RMB</th>
                    <th data-options="field:'BankName',align:'left'" style="width: 7%;">换汇银行</th>                
                    <th data-options="field:'ConsignorCode',align:'left'" style="width: 14%;">境外发货人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 6%;">申请日期</th>
                    <th data-options="field:'ExchangeRate',align:'left'" style="width: 4%;">换汇汇率</th>
                    <th data-options="field:'UpdateDate',align:'center'" style="width: 6%;">完成日期</th>
                    <th data-options="field:'SwapStatus',align:'center'" style="width: 6%;">换汇状态</th>   
                    <th data-options="field:'SwapCreWord',align:'center'" style="width: 6%;">凭证字</th>
                    <th data-options="field:'SwapCreNo',align:'center'" style="width: 6%;">凭证号</th>       
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
