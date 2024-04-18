<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnExitedNew.aspx.cs" Inherits="WebApp.ExitOrder.Exit.UnExitedNew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>送货单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        var exitType = eval('(<%=this.Model.ExitType%>)');

        //页面加载时
        $(function () {
            $('#ExitType').combobox({
                data: exitType
            });

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
                },
                onLoadSuccess: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        //
                        $('[id^=uploadFile]').filebox({
                            validType: ['fileSize[10240,"KB"]'],
                            buttonText: '上传文件',
                            buttonAlign: 'right',
                            prompt: '请选择图片或PDF类型的文件',
                            accept: ['application/pdf', 'image/jpeg','image/png'],
                            onChange: function (e) {
                                if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                                    $.messager.alert('提示', '文件大小不能超过10M！');
                                    return;
                                }

                                var formData = new FormData($('#form1')[0]);
                                formData.append('ID', this.id.split("-")[1]);
                                formData.append('NO', this.id.split("-")[2]);
                                MaskUtil.mask();
                                $.ajax({
                                    url: '?action=UploadFile',
                                    type: 'POST',
                                    data: formData,
                                    dataType: 'JSON',
                                    cache: false,
                                    processData: false,
                                    contentType: false,
                                    success: function (res) {
                                        MaskUtil.unmask();
                                        if (res.success) {
                                            //$('#AgreementChangeList').datagrid('reload');
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            }
                        });
                    }
                },
            });
        });

        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var entryNumber = $('#EntryNumber').textbox('getValue');
            var exitType = $('#ExitType').combobox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: orderID, EntryNumber: entryNumber, ExitType: exitType });
        }

        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $("#EntryNumber").textbox('setValue', "");
            $("#ExitType").combobox('setValue', "");
            Search();
        }

        function Printing(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                switch (rowdata.ExitType) {
                    case "送货上门":
                        var url = location.pathname.replace(/UnExitedNew.aspx/ig, 'DeliveryBillNew.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID;
                        window.location = url;
                        break;
                    case "本地快递":
                        var url = location.pathname.replace(/UnExitedNew.aspx/ig, 'ExpressBillNew.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID;
                        window.location = url;
                        break;
                    case "自提":
                        var url = location.pathname.replace(/UnExitedNew.aspx/ig, 'LadingBillNew.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID;
                        window.location = url;
                        break;
                }
            }
        }


        function Operation(val, row, index) {
            //if (row.IsModify == '<%=Needs.Ccs.Services.Models.CgPickingExcuteStatus.PartialShiped.GetHashCode()%>'||row.IsModify == '<%=Needs.Ccs.Services.Models.CgPickingExcuteStatus.Completed.GetHashCode()%>') {
            var buttons = '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Printing(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">详情</span>' +
                '<span class="l-btn-icon icon-print">&nbsp;</span>' +
                '</span>' +
                '</a>';

             //}
            buttons += '<input id="uploadFile-' + row.ID + '-' + index + '" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 26px" />';
            return buttons;
        }

        function ViewFile(val, row, index) {
            var fmt = "";
            if (row.Url != "" && row.Url.length > 30) {
                fmt += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px; margin-left: 10px;" onclick="ViewSAEle(\''
                    + row.Url + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return fmt;
        }

        //查看文件
        function ViewSAEle(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('doc') > 0) {
                var a = document.createElement("a");
                //a.download = name + ".xls";
                a.href = url;
                $("body").append(a); // 修复firefox中无法触发click
                a.click();
                $(a).remove();
            } else if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");

                $('#viewFileDialog').window('open').window('center');
            } else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");

                $('#viewFileDialog').window('open').window('center');
            }
        }

        function ExitStatus(val, row, index) {
            var text = "未出库";
            if (row.IsModify == '<%=Needs.Ccs.Services.Models.CgPickingExcuteStatus.Completed.GetHashCode()%>') {
                text = "已出库";
            }

            return text;
        }
    </script>
</head>
<body class="easyui-layout">
     <form id="form1" runat="server" method="post">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">订单编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="OrderID" />
                    </td>
                    <td class="lbl">客户编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="EntryNumber" />
                    </td>
                    <td class="lbl">送货类型: </td>
                    <td>
                        <input class="easyui-combobox search" id="ExitType" data-options="valueField:'Key',textField:'Value'" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="出库单" data-options="fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'center'" style="width: 16%">订单编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 10%">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 20%">客户名称</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 8%">件数</th>
                    <th data-options="field:'ExitType',align:'center'" style="width: 10%">送货类型</th>
                    <th data-options="field:'Url',align:'center',formatter:ViewFile" style="width: 8%">查看附件</th>
                    <th data-options="field:'NoticeStatus',align:'center',formatter:ExitStatus" style="width: 10%">状态</th>
                    <th data-options="field:'btnPacking',formatter:Operation,align:'center'" style="width:20%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
     <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
         </form>
</body>
</html>
