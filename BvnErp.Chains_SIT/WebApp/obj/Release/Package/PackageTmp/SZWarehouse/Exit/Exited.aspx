<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exited.aspx.cs" Inherits="WebApp.SZWareHouse.Exit.Exited" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已出库-出库通知</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '出库通知(SZ)';
        gvSettings.menu = '已出库';
        gvSettings.summary = '';
    </script>--%>
    <script>
        var exitType = eval('(<%=this.Model.ExitType%>)');
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                border: false,
                nowrap: false,
                autoRowHeight: false,
                onLoadSuccess: function (data) {
                    //for (var i = 0; i < data.rows.length; i++) {
                    //    $("#upload-receipt-confirm-file-" + data.rows[i].ID).filebox({
                    //        buttonText: '上传单据',
                    //        buttonIcon:'icon-add',
                    //        buttonAlign: 'left',
                    //        accept: 'image/gif, image/jp2, image/jpeg, image/png',
                    //        multiple: false,
                    //        width: '78px',
                    //        height: '26px',
                    //        onClickButton: function () {
                    //            $("#" + this.id).filebox('setValue', '');
                    //        },
                    //        onChange: function (newValue, oldValue) {
                    //            if ($("#" + this.id).filebox('getValue') == '') {
                    //                return;
                    //            }

                    //            var formData = new FormData();
                    //            var exitNoticeId = $("#" + this.id).attr('exitnoticeid');
                    //            formData.append('exitnoticeid', exitNoticeId);

                    //            var files = $("input[id='" + this.id + "'] + span > input[type='file']").get(0).files;
                    //            for (var i = 0; i < files.length; i++) {
                    //                //文件信息
                    //                var file = files[i];
                    //                var fileType = file.type;
                    //                var fileSize = file.size / 1024;
                    //                var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];

                    //                //检查文件类型
                    //                if (imgArr.indexOf(file.type) <= -1) {
                    //                    $.messager.alert('提示', '请选择图片文件上传！');
                    //                    return;
                    //                }

                    //                if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                    //                    photoCompress(file, { quality: 0.8 }, function (base64Codes, fileName) {
                    //                        var bl = convertBase64UrlToBlob(base64Codes);
                    //                        formData.append('upload-receipt-confirm-file', bl, fileName); // 文件对象

                    //                        SubmitFile(formData);
                    //                    });
                    //                } else {
                    //                    formData.append('upload-receipt-confirm-file', file);

                    //                    SubmitFile(formData);
                    //                }

                                    
                    //            }

                    //        },
                    //    });

                    //    $("#upload-receipt-confirm-file-" + data.rows[i].ID).next().width(76);
                    //    $('#datagrid').datagrid('fixRowHeight', i);
                    //}
                },
            });

            $('#ExitType').combobox({
                data: exitType
            });
        });

        //function SubmitFile(formData) {
        //    //ajax 上传
        //    $.ajax({
        //        url: '?action=UploadReceiptConfirmFile',
        //        type: 'POST',
        //        data: formData,
        //        dataType: 'JSON',
        //        cache: false,
        //        processData: false,
        //        contentType: false,
        //        success: function (res) {
        //            if (res.success) {
        //                $('#datagrid').datagrid('reload');
        //                $.messager.alert('提示', "上传成功", 'info', function () {
        //                    //ViewReceiptConfirmFile(exitNoticeId);
        //                });
        //                //$.messager.alert('提示', "上传成功");
        //            } else {
        //                $.messager.alert('错误', res.message);
        //            }
        //        }
        //    }).done(function (res) {

        //    });
        //}

        function Search() {
            var exitNoticeID = $('#ExitNoticeID').textbox('getValue');
            var orderID = $('#OrderID').textbox('getValue');
            var entryNumber = $('#EntryNumber').textbox('getValue');
            var exitType = $('#ExitType').combobox('getValue');
            var clientName = $('#ClientName').textbox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: orderID, EntryNumber: entryNumber, ExitType: exitType, ExitNoticeID: exitNoticeID, ClientName: clientName, });
        }

        function Reset() {
            $("#ExitNoticeID").textbox('setValue', "");
            $("#OrderID").textbox('setValue', "");
            $("#EntryNumber").textbox('setValue', "");
            $("#ExitType").combobox('setValue', "");
            $("#ClientName").textbox('setValue', "");
            Search();
        }
        //详情
        function Detail(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                switch (rowdata.ExitType) {
                    case "送货上门":
                        var url = location.pathname.replace(/Exited.aspx/ig, 'DeliveryBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID + "&ExitStatus=" + 4;
                        window.location = url;
                        break;
                    case "快递":
                        var url = location.pathname.replace(/Exited.aspx/ig, 'ExpressBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID + "&ExitStatus=" + 4;
                        window.location = url;
                        break;
                    case "自提":
                        var url = location.pathname.replace(/Exited.aspx/ig, 'LadingBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID + "&ExitStatus=" + 4;
                        window.location = url;
                        break;
                }
            }

        }

        ////查看客户确认单
        //function ViewReceiptConfirmFile(exitNoticeID) {
        //    $("#confirm-file-container").html("");

        //    $.post('?action=GetReceiptConfirmFileInfo', { ExitNoticeID: exitNoticeID, }, function (res) {
        //        var resJson = JSON.parse(res);
        //        if (resJson.success) {
        //            $('<img/>',{
        //                src: resJson.exitNoticeFile.URL + '?time=' + new Date().getTime(),
        //                width: '1020px',
        //                //height: '600px',
        //                display: 'block',
        //            }).appendTo('#confirm-file-container');

        //            $('#view-receipt-confirm-file-dialog').dialog('open');
        //        } else {
        //            $.messager.alert('错误', res.message, 'info', function () {
        //                $('#datagrid').datagrid('reload');
        //            });
        //        }
        //    });
        //}

        function Operation(val, row, index) {
            var buttons = '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Detail(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">详情</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            //buttons += '<input id="upload-receipt-confirm-file-' + row.ID + '" type="text" style="width:300px" exitnoticeid="' + row.ID + '">';

            //if (row.IsHasReceiptConfirmationFile) {
            //    buttons += '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="ViewReceiptConfirmFile(\'' + row.ID + '\')" group >' +
            //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //        '<span class="l-btn-text">查看</span>' +
            //        '<span class="l-btn-icon icon-search">&nbsp;</span>' +
            //        '</span>' +
            //        '</a>';
            //}

            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">单号: </td>
                    <td>
                        <input class="easyui-textbox search" id="ExitNoticeID" />
                    </td>
                    <td class="lbl" style="padding-left: 5px;">送货类型: </td>
                    <td>
                        <input class="easyui-combobox search" id="ExitType" data-options="valueField:'Key',textField:'Value',editable:false," />
                    </td>
                    <td class="lbl" style="padding-left: 5px;">订单编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="OrderID" />
                    </td>
                    <td style="padding-left: 5px;">
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">客户编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="EntryNumber" />
                    </td>
                    <td class="lbl" style="padding-left: 5px;">客户名称: </td>
                    <td>
                        <input class="easyui-textbox search" id="ClientName" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="已出库" data-options="fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true,pageSize:20," class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'center'" style="width: 30px">单号</th>
                    <th data-options="field:'ExitType',align:'center'" style="width: 30px">送货类型</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 30px">件数</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 50px">订单编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 30px">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 100px">客户名称</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 30px">制单人</th>
                    <th data-options="field:'OutStockTime',align:'center'" style="width: 30px">出库时间</th>
                    <th data-options="field:'NoticeStatus',align:'center'" style="width: 30px">状态</th>
                    <th data-options="field:'btnPacking',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <!------------------------------------------------------------ 查看上传的单据弹框 html Begin ------------------------------------------------------------>

    <div id="view-receipt-confirm-file-dialog" class="easyui-dialog" style="width: 1100px; height: 650px;" data-options="title: '查看客户确认单', iconCls:'icon-search', resizable:false, modal:true, closed: true,">
        <div id="confirm-file-container" style="margin: 15px 20px 0 25px; text-align: center;">

        </div>
    </div>

    <!------------------------------------------------------------ 查看上传的单据弹框 html End ------------------------------------------------------------>

</body>
</html>
