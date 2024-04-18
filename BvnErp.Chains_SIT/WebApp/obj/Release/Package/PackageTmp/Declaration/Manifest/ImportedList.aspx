<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportedList.aspx.cs" Inherits="WebApp.Declaration.Manifest.ImportedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>
        gvSettings.fatherMenu = '舱单(XTD)';
        gvSettings.menu = '已申报';
        gvSettings.summary = '舱单列表';
    </script>--%>
    <script type="text/javascript">
        var BaseCusReceiptCode = eval('(<%=this.Model.CusReceiptCodeData%>)');
        $(function () {
            //下拉框数据初始化
            $('#BaseCusReceiptCode').combobox({
                data: BaseCusReceiptCode
            });
            //代理订单列表初始化
            $('#datagraid').myDatagrid({
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
            var VoyageNo = $('#VoyageNo').textbox('getValue');
            //var ContrNo = $('#ContrNo').textbox('getValue');
            var BillNo = $('#BillNo').textbox('getValue');
            var BaseCusReceiptCode = $('#BaseCusReceiptCode').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#datagraid').myDatagrid('search', { VoyageNo: VoyageNo, BillNo: BillNo, BaseCusReceiptCode: BaseCusReceiptCode, StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {
            $('#VoyageNo').textbox('setValue', null);
            $('#BillNo').textbox('setValue', null);
            //$('#ContrNo').textbox('setValue', null);
            $('#BaseCusReceiptCode').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //回执订单
        function ReceiptHead(id,BillNo) {
            var url = location.pathname.replace(/ImportedList.aspx/ig, 'ManifestTraces.aspx?ID=' + id + '&BillNo='+BillNo);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '回执信息',
                width: '1000px',
                height: '550px'
            });
        }
        function Check(index) {
            $('#datagraid').datagrid('selectRow', index);
            var rowdata = $('#datagraid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/ImportedList.aspx/ig, 'Edit.aspx') + '?ID=' + rowdata.BillNo + '&Edit=true&VoyageNo=' + rowdata.VoyageNo;
                $.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '舱单查看',
                    width: '1010px',
                    height: '530px'
                });
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Check(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ReceiptHead(\'' + row.ID + '\',\'' + row.BillNo + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">回执</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
        //申报
        function BatchConfirm() {
             var ids = [];
            var rows = $('#datagraid').datagrid('getSelections');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选需要申报的单号！');
                return;
            }

            var hadDec = "";
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].Status == "xdt02") {
                    ids.push(rows[i].BillNo);
                }
                //已提交申请
                if (rows[i].Status != "xdt01" && rows[i].Status != "xdt02") {
                    hadDec += rows[i].ContrNO + " ";
                }
            }

            if (hadDec != "" || ids.length < 1) {
                $.messager.alert('error', '请勾选已制单的单据！');
                return;
            }

            var ManifestIDs = ids.join();
            var Model =
            {
                ManifestIDs: ManifestIDs
            };
            $.messager.confirm('确认', "请您再次确认申报？", function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    $.post('?action=Declare', Model, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, function (r) {
                            Search();
                        });
                    });
                }
            });

        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
<%--        <div id="tool">
            <a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BatchConfirm()">申报</a>
        </div>--%>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">货物运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageNo" />
                   <%-- <span class="lbl">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" />--%>
                    <span class="lbl">提运单号: </span>
                    <input class="easyui-textbox" id="BillNo" />
                    <span class="lbl">状态: </span>
                    <input class="easyui-combobox" id="BaseCusReceiptCode" name="BaseCusReceiptCode" data-options="valueField:'Value',textField:'Text'" />
                </li>
                <li>
                    <span class="lbl">录单起始日期 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">录单结束日期: </span>
                    <input class="easyui-datebox" id="EndDate" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagraid" title="舱单已申报列表" data-options="fitColumns:true,border:false,fit:true,nowrap:false,singleSelect:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'VoyageNo',align:'center'" style="width: 14%">货物运输批次号</th>
                    <th data-options="field:'BillNo',align:'center'" style="width: 10%">提运单号</th>
                    <th data-options="field:'Port',align:'center'" style="width: 7%">口岸</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 7%">件数</th>
                    <th data-options="field:'ConsigneeName',align:'left'" style="width: 23%">境内收货人</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 8%">制单员</th>
                    <th data-options="field:'CreateTime',align:'center'" style="width: 8%">录单时间</th>
                    <th data-options="field:'StatusName',align:'left'" style="width: 9%">单据状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 12%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
