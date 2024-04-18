<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportedList.aspx.cs" Inherits="WebApp.Declaration.Declare.ImportedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '报关单(XTD)';
        gvSettings.menu = '已导入';
        gvSettings.summary = '报关单已导入';
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
                },
                 onLoadSuccess: function (data) {
                    $("a[name=btnView]").on('click', function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");

                        $('#viewfileImg').css("display", "none");
                        $('#viewfilePdf').css("display", "none");
                        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                            $('#viewfilePdf').attr('src', fileUrl);
                            $('#viewfilePdf').css("display", "block");

                        }
                        else {
                            $('#viewfileImg').attr('src', fileUrl);
                            $('#viewfileImg').css("display", "block");
                        }
                        $("#viewFileDialog").window('open').window('center');
                    });
                },
            });
        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var PreEntryId = $('#PreEntryId').textbox('getValue');
            var BaseCusReceiptCode = $('#BaseCusReceiptCode').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#datagraid').myDatagrid('search', { ContrNo: ContrNo, OrderID: OrderID, PreEntryId: PreEntryId, BaseCusReceiptCode: BaseCusReceiptCode, StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#PreEntryId').textbox('setValue', null);
            $('#BaseCusReceiptCode').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //回执订单
        function ReceiptHead(id, ContrNo) {
            var url = location.pathname.replace(/ImportedList.aspx/ig, 'DecTrace.aspx?ID=' + id + '&ContrNo=' + ContrNo);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '回执信息',
                width: '1000px',
                height: '550px'
            });
        }
        //查看订单
        function SearchHead(id, OrderID) {
            var url = location.pathname.replace(/ImportedList.aspx/ig, 'Declare.aspx?ID=' + id + '&Source=View&SourcePage=Imported&OrderID=' + OrderID);
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            if (row["IsDecHeadFile"] == "否") {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SearchHead(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ReceiptHead(\'' + row.ID + '\',\'' + row.ContrNO + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">回执</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                return buttons;
            }
            else {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SearchHead(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';

                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ReceiptHead(\'' + row.ID + '\',\'' + row.ContrNO + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">回执</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                buttons += '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.URL + '" style="margin:3px" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看文件</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                return buttons;
            }

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
                if (rows[i].Status == "02") {
                    ids.push(rows[i].ID);
                }
                //已提交申请
                if (rows[i].Status != "01" && rows[i].Status != "02") {
                    hadDec += rows[i].ContrNO + " ";
                }
            }

            if (hadDec != "" || ids.length < 1) {
                $.messager.alert('error', '请勾选已制单的单据！');
                return;
            }

            var DecHeadIDs = ids.join();
            var Model =
            {
                DecHeadIDs: DecHeadIDs
            };
            $.messager.confirm('确认', "请您再次确认申报？", function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    $.post('?action=Declare', Model, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, function (r) {
                            if (r) {
                                closeWin();
                            }
                        });
                    });
                }
            });

        }

        //批量转换舱单
        function Transform() {
            var ids = [];
            var rows = $('#datagraid').datagrid('getSelections');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选需要转换舱单的报关单！');
                return;
            }
            var hadbills = "";
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].Transformed) {
                    hadbills += rows[i].BillNo + " ";
                }
                else {
                    ids.push(rows[i].ID);
                }
            }
            //
            if (hadbills != "") {
                $.messager.alert('提示', hadbills + ' 提运单已存在！');
                return;
            }
            if (ids.length < 1) {
                $.messager.alert('提示', ' 请勾选需要转换舱单的报关单！');
                return;
            }

            id = ids.join();
            $.messager.confirm('确认', '请您再次确认转换舱单？', function (success) {
                MaskUtil.mask();//遮挡层
                if (success) {
                    $.post('?action=Transform', { ID: id }, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, 'info', function () {
                            Search();
                        });
                    });
                } else {
                   MaskUtil.unmask();//关闭遮挡层
                }

            });
        }

        //报关完成 (暂用)
        function Succeed() {
            var ids = [];
            var message = "";
            var rows = $('#datagraid').datagrid('getSelections');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选报关完成的报关单！');
                return;
            }

            for (var i = 0; i < rows.length; i++) {
                ids.push(rows[i].ID);

            }

            id = ids.join();
            $.messager.confirm('确认', "此步骤暂用，不是实际流程", function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    $.post('?action=Succeed', { ID: id }, function (res) {
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
        <div id="tool">
            <%--            <a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BatchConfirm()">申报</a>--%>

            <a id="btnSucceed" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Succeed()">报关完成(暂用)</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" />
                    <%--订单编号id暂时未知--%>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <span class="lbl">预录入号: </span>
                    <input class="easyui-textbox" id="PreEntryId" />
                </li>
                <li>
                    <span class="lbl">单据状态: </span>
                    <input class="easyui-combobox" id="BaseCusReceiptCode" name="BaseCusReceiptCode" data-options="valueField:'Value',textField:'Text'" />
                    <span class="lbl">录单起始日期 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">至: </span>
                    <input class="easyui-datebox" id="EndDate" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagraid" title="已导入" data-options="fitColumns:true,border:false,fit:true,singleSelect:true,nowrap:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ContrNO',align:'left'" style="width: 10%">合同号</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 11%">订单编号</th>
                    <th data-options="field:'BillNo',align:'left'" style="width: 7%">提(运)单号</th>
                    <th data-options="field:'EntryId',align:'left'" style="width: 10%">海关编号</th>
                    <th data-options="field:'PreEntryId',align:'left'" style="width: 10%">预录入号</th>
                    <th data-options="field:'AgentName',align:'left'" style="width: 10%">委托报关企业</th>
                    <th data-options="field:'IsInspection',align:'left'" style="width: 5%">商检/检疫</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 4%">录单时间</th>
                    <th data-options="field:'InputerID',align:'center'" style="width: 7%">制单员</th>
                    <th data-options="field:'StatusName',align:'left'" style="width: 6%">单据状态</th>
                    <th data-options="field:'IsDecHeadFile',align:'left'" style="width: 6%">是否上传报关单</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 13%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
