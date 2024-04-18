<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //文件地址初始化
            contractUrl = model.ContractUrl;
            invoiceUrl = model.InvoiceUrl;
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: true,
                scrollbarSize: 0,
                onLoadSuccess: function (data) {
                    AddSubtotalRow();
                }
            });

            Init();
        })
    </script>
    <script>
        function Init() {
            $('#company').textbox('setText', model.company);
            $('#beneficiary').textbox('setText', model.beneficiary);
            $('#OrderID').textbox('setText', model.LsOrder.ID);
            $('#currency').textbox('setText', model.LsOrder.Currency);
            $('#clientName').textbox('setText', model.LsOrder.ClientName);
        }
        //添加合计行
        function AddSubtotalRow() {
            $('#tab1').datagrid('appendRow', {
                CreateDate: '<span class="subtotal">合计：</span>',
                SpecID: '<span class="subtotal">--</span>',
                StartDate: '<span class="subtotal">--</span>',
                EndDate: '<span class="subtotal">--</span>',
                Quantity: '<span class="subtotal">--</span>',
                UnitPrice: '<span class="subtotal">--</span>',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
            });
        }
        //计算合计值
        function compute(colName) {
            var rows = $('#tab1').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
          <%--  <tr>
                <td style="width: 90px;">内部公司:</td>
                <td>
                    <input id="company" class="easyui-textbox" style="width: 220px" />
                </td>
                <td style="width: 90px;">受益账号:</td>
                <td colspan="5">
                    <input id="beneficiary" class="easyui-textbox" style="width: 220px" />
                </td>
            </tr>--%>
            <tr>
                <td style="width: 90px;">订单编号</td>
                <td>
                    <input id="OrderID" class="easyui-textbox" style="width: 220px" />
                </td>
                <td style="width: 90px;">客户名称</td>
                <td>
                    <input id="clientName" class="easyui-textbox" style="width: 220px" />
                </td>
                <td style="width: 90px;">订单币种:</td>
                <td>
                    <input id="currency" class="easyui-textbox" style="width: 220px" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="contract" class="easyui-linkbutton" iconcls="icon-yg-search" onclick="View(contractUrl)">查看合同</a>
                    &nbsp;&nbsp;&nbsp;
                    <a id="invoice" class="easyui-linkbutton" iconcls="icon-yg-search" onclick="View(invoiceUrl)">查看发票</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 60px;">申请日期</th>
                <th data-options="field:'SpecID',align:'center'" style="width: 60px">库位级别</th>
                <th data-options="field:'StartDate',align:'center'" style="width: 60px">租赁开始时间</th>
                <th data-options="field:'EndDate',align:'center'" style="width: 60px;">租赁结束时间</th>
                <th data-options="field:'Month',align:'center'" style="width: 60px;">租赁时长(月)</th>
                <th data-options="field:'Quantity',align:'center'" style="width: 60px;">租赁数量(个)</th>
                <th data-options="field:'UnitPrice',align:'center'" style="width: 60px">租赁单价</th>
                <th data-options="field:'TotalPrice',align:'center'" style="width: 60px">租赁总价</th>
            </tr>
        </thead>
    </table>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px;height:500px;min-width:70%;min-height:80%">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
