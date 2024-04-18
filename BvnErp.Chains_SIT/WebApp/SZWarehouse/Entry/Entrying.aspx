<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Entrying.aspx.cs" Inherits="WebApp.SZWarehouse.Entry.Entrying" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <title>入库</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                nowrap: false
            });
        });

        //返回
        function Back() {
            var url = location.pathname.replace(/Entrying.aspx/ig, 'UnEntryList.aspx');
            window.location = url;
        }

        //确认入库
        function Entry() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                return;
            }
            //验证成功
            var ID = getQueryString('ID');
            var StockCode = $("#StockCode").textbox('getValue');
            MaskUtil.mask();
            $.post('?action=Entry', {
                ID: ID,
                StockCode: StockCode,
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    Back();
                });
            })
        }

        //合并单元格
        function onLoadSuccess(data) {
            var mark = 1;
            for (var i = 1; i < data.rows.length; i++) {
                if (data.rows[i]['CaseNumber'] == data.rows[i - 1]['CaseNumber']) {
                    mark += 1;
                    $("#datagrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'CaseNumber',
                        rowspan: mark
                    });
                }
                else {
                    mark = 1;
                }
            }
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnDetails" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Details(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">详情</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'center',border:false,title:'待入库产品'">
        <form id="form1" style="padding: 6px 0">
            <table>
                <tr>
                    <td class="lbl">库位号: </td>
                    <td>
                        <input class="easyui-textbox search" id="StockCode"
                            data-options="required:true,validType:'length[1,50]',height:26,width:150,tipPosition:'bottom',missingMessage:'请输入库位号'" />
                    </td>
                    <td>
                        <a id="btnEntry" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Entry()">确认入库</a>
                    </td>
                    <td>
                        <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                    </td>
                </tr>
            </table>
        </form>
        <table id="datagrid" class="easyui-datagrid" title="" data-options="
            fitColumns:true,
            scrollbarSize:0,
            pagination: true, 
            rownumbers: true, 
            multiSort: true, 
            nowrap: false,
            singleSelect:true,
            onLoadSuccess: onLoadSuccess">
            <thead>
                <tr>
                    <th field="CaseNumber" data-options="align:'center'" style="width: 50px">箱号</th>
                    <th field="Model" data-options="align:'center'" style="width: 50px">型号</th>
                    <th field="ProductName" data-options="align:'left'" style="width: 100px">报关品名</th>
                    <th field="Manufactor" data-options="align:'center'" style="width: 50px">品牌</th>
                    <th field="Quantity" data-options="align:'center'" style="width: 50px">数量</th>
                    <th field="NetWeight" data-options="align:'center'" style="width: 50px">净重</th>
                    <th field="GrossWeight" data-options="align:'center'" style="width: 50px">毛重</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
