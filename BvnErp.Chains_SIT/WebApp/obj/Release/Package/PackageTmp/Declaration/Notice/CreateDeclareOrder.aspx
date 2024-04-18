<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateDeclareOrder.aspx.cs" Inherits="WebApp.Declaration.Notice.CreateDeclareOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />

     <script>
        /*字典 Dictionary类*/
        function Dictionary() {
            this.add = add;
            this.datastore = new Array();
            this.find = find;           
        }

        function add(key, value) {
            this.datastore[key] = value;
        }

        function find(key) {
            return this.datastore[key];
        }     
    </script>

    <script type="text/javascript">
        var CaseNoCount = new Dictionary();

        $(function () {
            var DeclareNotice = eval('(<%=this.Model.DeclareNotice%>)');
            $("#NoticeID").textbox('setValue', DeclareNotice.NoticeID);
            $("#OrderID").textbox('setValue', DeclareNotice.OrderID);
            $("#ClientName").textbox('setValue', DeclareNotice.ClientName);
            $("#AdminName").textbox('setValue', DeclareNotice.AdminName);
            $("#ClientID").val(DeclareNotice.ClientID);
            //订单列表初始化
            $('#orders').myDatagrid({
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                idField: 'SortingID',
                onLoadSuccess: function (data) {
                    MergeCells('orders', 'CaseNumber', 'CaseNumber,CheckBox,AdminName,PackingDate,Status');
                },
            });
        });

        //查询
        function Search() {
            $('#orders').myDatagrid({
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                idField: 'SortingID',
                onLoadSuccess: function (data) {
                    MergeCells('orders', 'CaseNumber', 'CaseNumber,CheckBox,AdminName,PackingDate,Status');
                },
            });
        }



        //查看到货
        function CreateDeclareOrder(id) {

            var url = location.pathname.replace(/SplitDeclareList.aspx/ig, 'SplitDeclareDisplay.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '制单',
                width: '70%',
                height: '70%'
            });
        }

    </script>

    <script>
        /**
      * EasyUI DataGrid根据字段动态合并单元格
      * @param fldList 要合并table的id
      * @param fldList 要合并的列,用逗号分隔(例如："name,department,office");
      */
        function MergeCells(tableID, baseCol, fldList) {
            var dg = $('#' + tableID);
            var fldName = baseCol;
            var RowCount = dg.datagrid("getRows").length;
            var span;
            var PerValue = "";
            var CurValue = "";
            for (row = 0; row <= RowCount; row++) {
                if (row == RowCount) {
                    CurValue = "";
                }
                else {
                    CurValue = dg.datagrid("getRows")[row][fldName];
                }
                if (PerValue == CurValue) {
                    span += 1;
                }
                else {
                    var index = row - span;
                    $.each(fldList.split(","), function (i, val) {
                        dg.datagrid('mergeCells', {
                            index: index,
                            field: val,
                            rowspan: span,
                            colspan: null
                        });
                    });
                    CaseNoCount.add(PerValue, span);
                    span = 1;
                    PerValue = CurValue;
                }
            }
        }

        function SplitDeclare() {
            var ids = [];
            var boxIndexs = [];
            var rows = $('#orders').datagrid('getSelections');

            if (rows.length < 1) {
                $.messager.alert('消息', '请勾选需要拆分报关的箱号！');
                return;
            }
           
            var count = 0;
            var istatus = 0
            $.each(rows, function (i, item) {
                count += CaseNoCount.find(item.CaseNumber);
                if (item.Status == "已制单") {
                    istatus++;
                }
            });
     
            if (count > 50) {
                $.messager.alert('消息', '一次只能选择50个型号！');
                return;
            }

            if (istatus != 0) {
                $.messager.alert('消息', '存在已制单的箱子，请重新选择!');
                return;
            }
            else {
                for (var i = 0; i < rows.length; i++) {
                    ids.push(rows[i].SortingID);
                    boxIndexs.push(rows[i].CaseNumber);
                }              
                var SortingIDS = ids.join();
                var id = "";
                var OrderID = $("#OrderID").textbox("getValue");
                var ClientID = $("#ClientID").val();
                var NoticeID = $("#NoticeID").textbox("getValue");
                var url = location.pathname.replace(/CreateDeclareOrder.aspx/ig, '../Declare/Declare.aspx?ID=' + id + '&OrderID=' + OrderID + '&ClientID=' + ClientID + '&NoticeID=' + NoticeID + '&SortingIDS=' + SortingIDS + '&BoxIndexs='+boxIndexs+'&Source=Add');
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '报关信息',
                    width: '1000px',
                    height: '550px'
                });
            }

        }

        function AllDeclare() {
            var ids = [];
            var rows = $('#orders').datagrid('getRows');
      
            if (rows.length < 1) {
                $.messager.alert('消息', '请勾选需要拆分报关的箱号！');
                return;
            }
            if (rows.length > 50) {
                $.messager.alert('消息', '一次只能选择50个型号！');
                return;
            }
            var istatus = 0
            $.each(rows, function (i, item) {
                if (item.Status == "已制单") {
                    istatus++;
                }
            });
            if (istatus != 0) {
                $.messager.alert('消息', '存在已制单的箱子，请重新选择!');
                return;
            } else {
                for (var i = 0; i < rows.length; i++) {
                    ids.push(rows[i].PackingID);
                }
                var SortingIDS = ids.join();
                var id = "";
                var OrderID = $("#OrderID").textbox("getValue");
                var ClientID = $("#ClientID").val();
                var NoticeID = $("#NoticeID").textbox("getValue");
                var url = location.pathname.replace(/CreateDeclareOrder.aspx/ig, '../Declare/Declare.aspx?ID=' + id + '&OrderID=' + OrderID + '&ClientID=' + ClientID + '&NoticeID=' + NoticeID + '&SortingIDS=' + SortingIDS + '&Source=Add');
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '报关信息',
                    width: '1000px',
                    height: '550px'
                });
            }
        }
    </script>

   
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div>
            <span class="lbl">通知编号: </span>
            <input class="easyui-textbox" id="NoticeID" readonly="true" />
            <span class="lbl">订单编号: </span>
            <input class="easyui-textbox" id="OrderID" readonly="true" />
            <span class="lbl">客户名称: </span>
            <input class="easyui-textbox" id="ClientName" readonly="true" />
            <input type="hidden" id="ClientID" />
            <span class="lbl">客服: </span>
            <input class="easyui-textbox" id="AdminName" readonly="true" />
            <a id="btnSplitDeclare" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="SplitDeclare()">制单</a>
            <%-- <a id="btnAllDeclare" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="AllDeclare()">一键制单</a>--%>
        </div>
    </div>

    <div data-options="region:'center',border:false">
        <table id="orders" title="报关通知列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'CaseNumber',align:'center'" style="width: 120px;">箱号</th>
                    <th data-options="field:'Batch',align:'center'" style="width: 120px;">批号</th>
                    <th data-options="field:'Name',align:'center'" style="width: 100px;">品名</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 100px;">品牌</th>
                    <th data-options="field:'Model',align:'center'" style="width: 100px;">型号</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 100px;">产地</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 100px;">数量</th>
                    <th data-options="field:'Unit',align:'center'" style="width: 100px;">单位</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 100px;">单价</th>
                    <th data-options="field:'TotalPrice',align:'center'" style="width: 100px;">总价</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 100px;">币种</th>
                    <th data-options="field:'GrossWeight',align:'center'" style="width: 100px;">毛重(KG)</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 100px;">库房人员</th>
                    <th data-options="field:'PackingDate',align:'center'" style="width: 100px;">装箱日期</th>
                    <th data-options="field:'Status',align:'center'" style="width: 100px;">状态</th>
                    <th data-options="field:'SortingID',align:'center', hidden: true"></th>                    
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
