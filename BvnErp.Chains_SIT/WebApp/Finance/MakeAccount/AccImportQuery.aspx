<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccImportQuery.aspx.cs" Inherits="WebApp.Finance.MakeAccount.AccImportQuery" %>

<!DOCTYPE html>

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
                    <td class="lbl">票据到期日:</td>
                    <td>
                       <input class="easyui-datebox" id="StartDate" />
                    </td>
                     <td class="lbl">至:</td>
                    <td>
                       <input class="easyui-datebox" id="EndDate" />
                    </td>
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
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="承兑列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,">
            <thead>
                <tr>                  
                    <th data-options="field:'RequestID',align:'left'" style="width: 100px;">申请ID</th>
                    <th data-options="field:'Code',align:'left'" style="width: 120px;">票据号码</th>
                    <th data-options="field:'Endorser',align:'left'" style="width: 150px;">付款人</th>
                    <th data-options="field:'Price',align:'left'" style="width: 80px;">票据金额</th>                   
                    <th data-options="field:'StartDate',align:'left'" style="width: 60px;">出票日期</th>
                    <th data-options="field:'ReceiveBank',align:'left'" style="width: 100px;">贴现银行</th>
                    <th data-options="field:'Interest',align:'left'" style="width: 60px;">贴现利息</th>
                    <th data-options="field:'ExchangeDate',align:'left'" style="width: 60px;">贴现日期</th>  
                    <th data-options="field:'AccCreNo',align:'left'" style="width: 60px;">凭证号</th>
                    <th data-options="field:'AccCreWord',align:'left'" style="width: 60px;">凭证字</th>  
                    <%--<th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

