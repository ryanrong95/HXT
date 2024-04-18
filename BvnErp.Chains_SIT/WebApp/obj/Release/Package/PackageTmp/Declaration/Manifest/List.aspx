<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Declaration.Manifest.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>舱单列表</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '舱单(XTD)';
        gvSettings.menu = '草稿';
        gvSettings.summary = '舱单列表';
    </script>--%>
    <script type="text/javascript">
<%--        var Status = eval('(<%=this.Model.Status%>)');--%>
        var ThisAdminOriginID = '<%=this.Model.ThisAdminOriginID%>';        
        $(function () {
            //下拉框数据初始化
            //$('#Status').combobox({
            //    data: Status
            //});
            //代理订单列表初始化
            $('#datagraid').myDatagrid({
                fitColumns: true, border: false, fit: true, singleSelect: false, toolbar: '#topBar',
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
            //var ContrNo = $('#ContrNo').textbox('getValue');
            var VoyageNo = $('#VoyageNo').textbox('getValue');
            var BillNo = $('#BillNo').textbox('getValue');
            //var Status = $('#Status').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#datagraid').myDatagrid('search', { VoyageNo: VoyageNo, BillNo: BillNo, StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {
            //$('#ContrNo').textbox('setValue', null);
            $('#VoyageNo').textbox('setValue', null);
            $('#BillNo').textbox('setValue', null);
            //$('#Status').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //编辑舱单
        function Edit(index) {
            $('#datagraid').myDatagrid('selectRow', index);
            var rowdata = $('#datagraid').myDatagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'DoubleCheck.aspx') + '?ID=' + rowdata.BillNo + '&Edit=true&VoyageNo=' + rowdata.VoyageNo;
                $.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '舱单复核',
                    width: '1010px',
                    height: '560px',
                    onClose: function () {
                        Search();
                    }
                });
            }
        }
        function Check(index) {
            $('#datagraid').myDatagrid('selectRow', index);
            var rowdata = $('#datagraid').myDatagrid('getSelected');
            if (rowdata) {
                //var url = location.pathname.replace(/List.aspx/ig, 'Show.aspx') + '?ID=' + rowdata.ID;
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + rowdata.ID + '&Edit=false&VoyageNo=' + rowdata.VoyageNo;
                window.location = url;
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';         
            if (row.DoubleCheckerAdminID == ThisAdminOriginID) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">复核</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttons;
        }

        function BatchConfirm() {
            var ids = [];
            var message = "";
            var rows = $('#datagraid').myDatagrid('getSelections');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选需要制单的舱单！');
                return;
            }
            var hadMark = "";
            for (var i = 0; i < rows.length; i++) {
                ids.push(rows[i].BillNo);
                if (rows[i].Transformed) {
                    hadMark += rows[i].BillNo + " ";
                }
            }
            //
            if (hadMark != "") {
                message = hadMark + ' 已制单，是否全部重新制单?';
            }
            else {
                message = '请您再次确认制单？';
            }

            id = ids.join();
            $.messager.confirm('确认', message, function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    $.post('?action=Make', { ID: id }, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, 'info', function () {
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
        <%-- <div id="tool">
            <a id="btnConfirm" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="BatchConfirm()">制单</a>
        </div>--%>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">货物运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageNo" />
                    <%--<span class="lbl">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" />--%>
                    <span class="lbl">提运单号: </span>
                    <input class="easyui-textbox" id="BillNo" />
                </li>
                <li>
                    <span class="lbl">录单起始日期 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">录单结束日期: </span>
                    <input class="easyui-datebox" id="EndDate" />
                    <%--                    <span class="lbl">状态: </span>
                    <input class="easyui-combobox" id="Status" name="Status" data-options="valueField:'Value',textField:'Text'" />--%>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagraid" title="舱单草稿列表" data-options="fitColumns:true,border:false,fit:true,singleSelect:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'VoyageNo',align:'center'" style="width: 13%">货物运输批次号</th>
                    <th data-options="field:'BillNo',align:'center'" style="width: 10%">提运单号</th>
                    <th data-options="field:'Port',align:'center'" style="width: 9%">口岸</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 8%">件数</th>
                    <th data-options="field:'ConsigneeName',align:'center'" style="width: 25%">境内收货人</th>
                     <th data-options="field:'DoubleCheckerName',align:'center'" style="width: 8%">复核员</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 8%">制单员</th>
                    <th data-options="field:'CreateTime',align:'center'" style="width: 8%">录单时间</th>

                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
