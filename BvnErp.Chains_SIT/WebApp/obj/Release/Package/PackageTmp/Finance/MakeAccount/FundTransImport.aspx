<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FundTransImport.aspx.cs" Inherits="WebApp.Finance.MakeAccount.FundTransImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>调拨查询界面</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>    
    
 
    <script type="text/javascript">  

        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true, fit: true, toolbar: '#topBar', nowrap: false, rownumbers: true,
                singleSelect: false,
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
           
        });

        //查询
        function Search() {           
            var ApplyNo = $('#ApplyNo').textbox('getValue');
            var OutBankName = $('#OutBankName').textbox('getValue');
            var InBankName = $('#InBankName').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var parm = {               
                ApplyNo: ApplyNo,
                OutBankName: OutBankName,
                InBankName: InBankName,
                StartDate: StartDate,
                EndDate : EndDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {         
            $('#ApplyNo').textbox('setValue', null);
            $('#OutBankName').textbox('setValue', null);
            $('#InBankName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }


        function MakeAccount() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要做账的数据！');
                return;
            }
            MaskUtil.mask();//遮挡层
            //验证成功
            $.post('?action=MakeAccount', {
                Model: JSON.stringify(data)
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', "生成凭证成功", 'info', function () {
                        $('#datagrid').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息', "生成凭证失败", 'info', function () { });
                }
            });
        }

         function MakeAccountAll() {         
            var ApplyNo = $('#ApplyNo').textbox('getValue');
            var OutBankName = $('#OutBankName').textbox('getValue');
            var InBankName = $('#InBankName').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            MaskUtil.mask();//遮挡层
            //验证成功
            $.post('?action=MakeAccountAll', {               
                ApplyNo: ApplyNo,
                OutBankName: OutBankName,
                InBankName: InBankName,
                StartDate: StartDate,
                EndDate : EndDate
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', "生成凭证成功", 'info', function () {
                        $('#datagrid').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息', rel.msg, 'info', function () { });
                }
            });
        }

       
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
       
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">申请编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ApplyNo" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">调出银行:</td>
                    <td>
                        <input class="easyui-textbox" id="OutBankName" data-options="height:26,width:200"/>
                    </td>
                    <td class="lbl">调入银行:</td>  
                    <td>
                        <input class="easyui-textbox" id="InBankName" data-options="height:26,width:200"/>
                    </td>                   
                </tr>
                <tr>
                    <td class="lbl">完成时间:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false,height:26,width:200" />
                    </td>
                    <td class="lbl">至:</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false,height:26,width:200" />
                    </td>
                    <td>
                        &nbsp
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnMake" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccount()">生成凭证</a>
                        <a id="btnMakeAll" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccountAll()">生成全部凭证</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="银行往来" data-options="fitColumns:true,fit:true,toolbar:'#topBar',nowrap:false,rownumbers:true, singleSelect: false,">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ID',align:'left'" style="width: 80px;">申请编号</th>
                    <th data-options="field:'OutAccountID',align:'left'" style="width: 80px;">调出账户</th>
                    <th data-options="field:'OutAccountName',align:'left'" style="width: 80px;">调出银行</th>
                    <th data-options="field:'OutAmount',align:'left'" style="width: 80px;">调出金额</th>
                    <th data-options="field:'InAccountID',align:'left'" style="width: 100px;">调入账户</th>
                    <th data-options="field:'InAccountName',align:'left'" style="width: 80px;">调入银行</th>
                    <th data-options="field:'InAmount',align:'left'" style="width: 100px;">调入金额</th>                  
                    <th data-options="field:'CreateDate',align:'left'" style="width: 100px;">申请时间</th>                                                
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
