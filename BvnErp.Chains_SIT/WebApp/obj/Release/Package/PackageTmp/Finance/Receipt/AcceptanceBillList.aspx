<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AcceptanceBillList.aspx.cs" Inherits="WebApp.Finance.Receipt.AcceptanceBillList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>费用申请-收款账户列表</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var From = '<%=this.Model.From%>';
        var WindowName = '<%=this.Model.WindowName%>';

        $(function () {
            if (From == 'select') {
                $("#btn-select").show();
            }

            $('#PayeeList').myDatagrid({
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
                    $(".datagrid-header-check").html("");
                },
            });

        });

        function Search() {
            var Code = $('#Code').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');           
           // var BillStatus = $('#BillStatus').combobox('getValue');
            var parm = {
                Code: Code,
                StartDate: StartDate,
                EndDate: EndDate,
                //BillStatus:BillStatus
            };
            $('#PayeeList').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Code').textbox('setValue', '');
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            //$('#BillStatus').combobox('setValue', null);
            Search();
        }

       
        function Select() {
            var selectPayee = $('#PayeeList').datagrid('getChecked');
            if (selectPayee == null || selectPayee.length <= 0) {
                $.messager.alert('提示', '请选择承兑汇票！');
                return;
            }
            debugger
            var ewindow = $.myWindow.getMyWindow(WindowName);

            if (WindowName == "FinanceReceipt") {

                //ewindow.SelectPayee.IsSelected = true;
                //ewindow.SelectPayee.PayeeName = selectPayee[0].AccountName;
                //ewindow.SelectPayee.PayeeAccount = selectPayee[0].BankAccount;
                //ewindow.SelectPayee.PayeeBank = selectPayee[0].BankName;
                //ewindow.SelectPayee.PayeeAccountID = selectPayee[0].ID;

                ewindow.$("#Payer").textbox('setValue',selectPayee[0].Endorser);
                ewindow.$("#SeqNo").textbox('setValue', selectPayee[0].Code);
                ewindow.$("#PaymentType").combobox('setValue',4)
                ewindow.$("#Amount").textbox('setValue', selectPayee[0].Price);
                //ewindow.$("#OutAccountBankName").textbox('setValue', selectPayee[0].BankName);
            } else {
                 ewindow.$("#InAccountID").val(selectPayee[0].ID);
                ewindow.$("#InAccountName").textbox('setValue', selectPayee[0].AccountName);
                ewindow.$("#InAccountNo").textbox('setValue', selectPayee[0].BankAccount);
                ewindow.$("#InAccountBankName").textbox('setValue', selectPayee[0].BankName);
            }




            var $iframes = self.parent.$('iframe');
            for (var i = 0; i < $iframes.length; i++) {
                if ($iframes[i].src.indexOf("AcceptanceBillList") != -1) {
                    //console.log('123');
                    //console.log($($iframes[i]));
                    //console.log('456');
                    //console.log($($iframes[i]).parent());
                    //console.log('789');
                    //console.log($($iframes[i]).parent().parent());

                    $($iframes[i]).parent().parent().next().next().remove();
                    $($iframes[i]).parent().parent().next().remove();
                    $($iframes[i]).parent().parent().remove();



                    break;
                }
            }

        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <ul>
            <li>
                <span class="lbl">票据号码:</span>
                <input class="easyui-textbox" id="Code" data-options="height:26,width:200" />     
                <span class="lbl">票据到期日:</span>
                <input class="easyui-datebox" id="StartDate" />
                <span class="lbl">至:</span>
                <input class="easyui-datebox" id="EndDate" />
                <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
            </li>
            <li>               
                <a id="btn-select" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" style="margin-left: 5px; display: none;"  onclick="Select()">选择</a>
            </li>
        </ul>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="PayeeList" data-options="fitColumns:true,fit:true,border:false,toolbar:'#topBar',">
            <thead>
                <tr>
                   
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 3%;">全选</th>
                    <th data-options="field:'Code',align:'left'" style="width: 200px;">承兑票号</th>
                    <th data-options="field:'Price',align:'left'" style="width: 80px;">金额</th>
                    <th data-options="field:'InAccountName',align:'left'" style="width: 150px;">收款人账户</th>
                    <th data-options="field:'OutAccountName',align:'left'" style="width: 150px;">出款人账户</th> 
                    <th data-options="field:'StartDate',align:'left'" style="width: 80px;">出票日期</th>
                    <th data-options="field:'EndDate',align:'left'" style="width: 80px;">汇票到期日</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="comfirm-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="comfirm-dialog-content" style="margin: 15px 15px 15px 15px;"></div>
    </div>

</body>
</html>
