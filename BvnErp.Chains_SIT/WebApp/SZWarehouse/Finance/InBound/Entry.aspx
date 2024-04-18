<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Entry.aspx.cs" Inherits="WebApp.SZWarehouse.Finance.InBound.Entry" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>入库单</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/jquery-barcode.js"></script>
    <script src="../../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        $(function () {
            InitInBound(AllData['InBound'],AllData['Date'],AllData['DecheadID']);
        });
        function InitInBound(data,date,id) {
            var str = '';
            $("#Date").html("入库日期：" + date);
            $("#DecheadNo").html(id);
            for (var index = 0; index < data.length; index++) {
                var row = data[index];
                var count = index + 1;
                //拼接表格的行和列
                str = '<tr><td>' + count + '</td><td style="text-align:left">' + row.GName + '</td><td>' + row.GoodsModel + '</td>' +
                    '<td>' + row.GUnit + '</td><td>' + row.GQty + '</td><td>' + row.DeclPrice.toFixed(4) + '</td>' +
                    '<td>' + row.Dectotal.toFixed(2) + '</td><td>' + row.Summary + '</td></tr>';
                $('#InBound').append(str);
            }
        }
        //导出文件
        function Export() {
            var id = AllData['ID'];
            //验证成功
            $.post('?action=ExportFiles', {
                ID: id,
            }, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }
        //打印
        function Print() {            
            event.preventDefault();
            $("#container").jqprint();
        }
        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <div title="入库单" style="padding: 10px;">
        <form id="form1" runat="server">
            <div style="width: 745px; margin: 10px auto">
                <a id="btnPrint" class="easyui-linkbutton" href="javascript:void(0);" data-options="iconCls:'icon-print'" onclick="Print()">打印</a>
                <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Export()">导出PDF</a>
                <a id="btnClose" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">返回</a>
            </div>
            <div id="container" style="width: 745px; margin: auto; background-color: white; padding: 10px;">
                <%-- 行内样式 --%>
                <style>
                    .title {
                        font: 14px Arial,Verdana,'微软雅黑','宋体';
                        font-weight: normal;
                    }

                    ul li {
                        list-style-type: none;
                    }

                    .border-table {
                        line-height: 15px;
                        border-collapse: collapse;
                        border: 1px solid gray;
                        width: 100%;
                    }

                        .border-table tr td {
                            font-weight: normal;
                            border: 1px solid gray;
                            text-align: center;
                        }

                        .border-table tr th {
                            font-weight: normal;
                            border: 1px solid gray;
                        }

                    .noneborder-table {
                        line-height: 20px;
                        border: none;
                        width: 100%;
                    }
                </style>
                <table id="InBound" title="入库单" class="border-table">
                    <tr>
                        <th style="width: 5%"></th>
                        <th style="width: 20%"></th>
                        <th style="width: 15%"></th>
                        <th style="width: 5%"></th>
                        <th style="width: 10%"></th>
                        <th style="width: 10%"></th>
                        <th style="width: 20%"></th>
                        <th style="width: 15%"></th>
                    </tr>
                    <tr>
                        <td colspan="8" style="font: 26px Arial,Verdana,'微软雅黑','宋体'">入库单
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"></td>
                        <td colspan="4" id="Date"></td>
                        <td>序号</td>
                        <td id="DecheadNo"></td>
                    </tr>
                    <tr>
                        <td>序号</td>
                        <td>品名</td>
                        <td>型号</td>
                        <td>单位</td>
                        <td>数量</td>
                        <td>单价</td>
                        <td>金额</td>
                        <td>备注</td>
                    </tr>
                </table>
                <div>
                    <table style="width: 100%">
                        <tr>
                            <td></td>
                            <td>主管：鲁亚慧</td>
                            <td></td>
                            <td>会计：鲁亚慧</td>
                            <td></td>
                            <td>报管员：商庆房</td>
                            <td></td>
                            <td>经手人：杨端峰</td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
