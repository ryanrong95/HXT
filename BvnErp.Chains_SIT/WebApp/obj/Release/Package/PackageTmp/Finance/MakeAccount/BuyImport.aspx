<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuyImport.aspx.cs" Inherits="WebApp.Finance.MakeAccount.BuyImport" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>承兑汇票界面</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>    
    <%-- <script>
        gvSettings.fatherMenu = '银行账户管理';
        gvSettings.menu = '账户管理';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">       
       var BillStatus = eval('(<%=this.Model.BillStatus%>)');       
        
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                toolbar: '#topBar',
                nowrap: false,
                rownumbers: true,
                singleSelect: false,
               
            });
            ////初始化Combobox
            $('#BillStatus').combobox({
                data: BillStatus,
            });

        });

        //查询
        function Search() {
            var Code = $('#Code').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');           
            var BillStatus = $('#BillStatus').combobox('getValue');
            var CreateStartDate = $('#CreateStartDate').datebox('getValue');
            var CreateEndDate = $('#CreateEndDate').datebox('getValue');
            var parm = {
                Code: Code,
                StartDate: StartDate,
                EndDate: EndDate,
                BillStatus: BillStatus,
                CreateStartDate: CreateStartDate,
                CreateEndDate:CreateEndDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Code').textbox('setValue', '');
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#BillStatus').combobox('setValue', null);
            $('#CreateStartDate').datebox('setValue', null);
            $('#CreateEndDate').datebox('setValue', null);
            Search();
        }


        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.ExchangeDate != null) {
                 buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Show(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
               
            }
                       
            return buttons;
        }


        function MakeBuy() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要做账的数据！');
                return;
             }
             for (i = 0; i < data.length; i++) {
                 if (data[i].BuyCreSta == true) {
                     $.messager.alert('提示', data[i].Code+'已生成承兑凭证');
                     return;
                 }
             };

             var AccInfo = [];
             for (i = 0; i < data.length; i++) {
                 AccInfo.push({
                     ID : data[i].ID,
                     Code : data[i].Code,
                     Price: data[i].Price,
                     Interest: data[i].Interest,
                     BankName : data[i].BankName,
                     Endorser: data[i].Endorser,
                     ExchangeDate: data[i].ExchangeDate,
                     FundTransferAccountName: data[i].FundTransferAccountName,
                     ReceiveBank: data[i].ReceiveBank
                 });
             };
             MaskUtil.mask();//遮挡层
            //验证成功
            $.post('?action=MakeBuy', {
                Model: JSON.stringify(AccInfo)
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


        function MakeBuyAll() {         
            var Code = $('#Code').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');           
            var BillStatus = $('#BillStatus').combobox('getValue');
            var CreateStartDate = $('#CreateStartDate').datebox('getValue');
            var CreateEndDate = $('#CreateEndDate').datebox('getValue');
            MaskUtil.mask();//遮挡层
            //验证成功
            $.post('?action=MakeBuyAll', {               
                Code: Code,
                StartDate: StartDate,
                EndDate: EndDate,
                BillStatus: BillStatus,
                CreateStartDate: CreateStartDate,
                CreateEndDate:CreateEndDate
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
                    <td class="lbl">票据号码</td>
                    <td>
                        <input class="easyui-textbox" id="Code" data-options="height:26,width:200" />                        
                    </td>
                    <td class="lbl">贴现日期:</td>
                    <td>
                       <input class="easyui-datebox" id="StartDate" />
                    </td>
                     <td class="lbl">至:</td>
                    <td>
                       <input class="easyui-datebox" id="EndDate" />
                    </td>                    
                </tr>
                <tr>
                    <td class="lbl">状态:</td>
                    <td>
                        <input class="easyui-combobox" id="BillStatus" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                     <td class="lbl">创建日期:</td>
                    <td>
                       <input class="easyui-datebox" id="CreateStartDate" />
                    </td>
                     <td class="lbl">至:</td>
                    <td>
                       <input class="easyui-datebox" id="CreateEndDate" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>                    
                        <a id="btnMakeBuy" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeBuy()">生成贴现凭证</a>
                        <a id="btnMakeBuyAll" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeBuyAll()">生成全部贴现凭证</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="承兑列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'Code',align:'left'" style="width: 150px;">票据号码</th>
                    <th data-options="field:'Endorser',align:'left'" style="width: 150px;">付款人</th>
                    <th data-options="field:'Price',align:'left'" style="width: 80px;">票据金额</th>   
                    <th data-options="field:'CreateDate',align:'left'" style="width: 60px;">创建日期</th>
                    <th data-options="field:'StartDate',align:'left'" style="width: 60px;">出票日期</th>
                    <th data-options="field:'ReceiveBank',align:'left'" style="width: 100px;">贴现银行</th>
                    <th data-options="field:'Interest',align:'left'" style="width: 60px;">贴现利息</th>
                    <th data-options="field:'ExchangeDate',align:'left'" style="width: 60px;">贴现日期</th>   
                    <th data-options="field:'BuyCreSta',align:'left'" style="width: 60px;">是否生成贴现</th>  
                    <%--<th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

