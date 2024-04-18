<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditEnquiry.aspx.cs" Inherits="WebApp.Crm.ProjectManagement.EditEnquiry" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>询价编辑</title>
    <uc:EasyUI runat="server" />
    <style>
        .normal_a {
            text-decoration: underline;
            color: blue;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <%
            var enquiry = this.Model.Enquiry as NtErp.Crm.Services.Models.Enquiry;
        %>
        <table id="table1" style="width: 100%">
            <tr>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
            </tr>
            <tr>
                <td class="lbl">报备时间</td>
                <td>
                    <input class="easyui-datebox" id="reportDate" name="reportDate" data-options="required:true" style="width: 95%" />
                </td>
                <td class="lbl">原厂批复凭证</td>
                <td>
                    <asp:FileUpload ID="OriginUpload" runat="server" />
                </td>
                <td class="lbl" id="tdorigin">已上传凭证</td>
                <td>
                    <div style="word-break: break-all; word-wrap: break-word; width: 90%">
                        <label id="OriginFile">
                            <a target="_blank" class="normal_a" href="<%=enquiry?.Voucher?.Url %>"><%=enquiry?.Voucher?.Name %></a>
                        </label>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="lbl">批复时间</td>
                <td>
                    <input class="easyui-datebox" id="replyDate" name="replyDate" data-options="required:true" style="width: 95%" />
                </td>
                <td class="lbl">批复单价</td>
                <td>
                    <input class="easyui-numberbox" id="replyPrice" name="replyPrice"
                        data-options="min:0,precision:5,validType:'length[1,15]',required:true" style="width: 95%" />
                </td>
                <td class="lbl">原厂RFQ号</td>
                <td>
                    <input class="easyui-textbox" id="rfq" name="rfq" data-options="validType:'length[1,50]'," style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">原厂型号</td>
                <td>
                    <input class="easyui-textbox" id="originModel" name="originModel" data-options="validType:'length[1,50]',required:true" style="width: 95%" />
                </td>
                <td class="lbl">最小起订量(MOQ)</td>
                <td>
                    <input class="easyui-numberbox" id="moq" name="moq" data-options="min:0,validType:'length[1,10]',required:true," style="width: 95%" />
                </td>
                <td class="lbl">最小包装量(MPQ)</td>
                <td>
                    <input class="easyui-numberbox" id="mpq" name="mpq" data-options="min:0,validType:'length[1,10]',required:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">币种</td>
                <td>
                    <input class="easyui-combobox" id="currency" name="currency" style="width: 95%" />
                </td>
                <td class="lbl">汇率</td>
                <td>
                    <input class="easyui-numberbox" id="exchangeRate" name="exchangeRate"
                        data-options="min:0,precision:5,validType:'length[1,15]'" style="width: 95%" />
                </td>
                <td class="lbl">税率(%)</td>
                <td>
                    <input class="easyui-numberbox" id="taxRate" name="taxRate"
                        data-options="min:0,precision:5,validType:'length[1,15]'" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">关税点(%)</td>
                <td>
                    <input class="easyui-numberbox" id="tariff" name="tariff"
                        data-options="min:0,precision:5,validType:'length[1,15]', required:true" style="width: 95%" />
                </td>
                <td class="lbl">其他附加点(%)</td>
                <td>
                    <input class="easyui-numberbox" id="otherRate" name="otherRate"
                        data-options="min:0,precision:5,validType:'length[1,15]', required:true" style="width: 95%" />
                </td>
                <td class="lbl">含税人民币成本价</td>
                <td>
                    <input class="easyui-numberbox" id="cost" name="cost"
                        data-options="min:0,precision:5,validType:'length[1,15]'" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">有效时间</td>
                <td>
                    <input class="easyui-datebox" id="validity" name="validity" data-options="required:true" style="width: 95%" />
                </td>
                <td class="lbl">有效数量</td>
                <td>
                    <input class="easyui-numberbox" id="validityCount" name="validityCount"
                        data-options="min:0,validType:'length[1,10]',required:true" style="width: 95%" />
                </td>
                <td class="lbl">参考售价</td>
                <td>
                    <input class="easyui-numberbox" id="salePrice" name="salePrice"
                        data-options="min:0,precision:5,validType:'length[1,15]',required:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">特殊备注</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="summary" name="summary" data-options="multiline:true,validType:'length[1,500]',tipPosition:'bottom'," style="width: 99%; height: 80px" />
                </td>
            </tr>

        </table>
        <div id="divSave" style="text-align: center; margin-top: 20px; margin-bottom: 10px">
            <asp:Button ID="btnSave" Text="保存" runat="server" OnClientClick="return save();" OnClick="btnSave_Click" />
        </div>
        <p>含税人民币成本价计算公式=[批复单价]*[汇率]*(1+关税点比)*(1+其他附加点比)</p>
    </form>
    <script>
        <%
        var enquiry = this.Model.Enquiry as NtErp.Crm.Services.Models.Enquiry;
        %>

        var currencyData = eval('<%=this.Model.CurrencyData%>');

        function save() {
            //校验必填项
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }

            //文件大小检验
            if ($("#OriginUpload")[0].files.length > 0 && $("#OriginUpload")[0].files[0].size > 4096 * 1024) {
                alert("上传的文件凭证超过4M!请重新选择文件上传!");
                return false;
            }
        }
        $(function () {
            $('#currency').combobox({
                data: currencyData,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                required: true,
                editable: false,
                onChange: function (newValue, oldValue) {

                }
            });
            $('#currency').combobox('select', eval('<%=(int)NtErp.Crm.Services.Enums.CurrencyType.CNY%>'));


            //汇率
            $('#exchangeRate').numberbox({
                onChange: function (newValue, oldValue) {
                    var replayPrice = $('#replyPrice').numberbox('getValue');
                    var tariff = $('#tariff').numberbox('getValue');
                    var otherRate = $('#otherRate').numberbox('getValue');
                    $('#cost').numberbox('setValue', newValue * replayPrice * (1 + tariff / 100) * (1 + otherRate / 100));
                }
            });
            //批复单价
            $('#replyPrice').numberbox({
                onChange: function (newValue, oldValue) {
                    var exchangeRate = $('#exchangeRate').numberbox('getValue');
                    var tariff = $('#tariff').numberbox('getValue');
                    var otherRate = $('#otherRate').numberbox('getValue');
                    $('#cost').numberbox('setValue', newValue * exchangeRate * (1 + tariff / 100) * (1 + otherRate / 100));
                }
            });
            //关税点
            $('#tariff').numberbox({
                onChange: function (newValue, oldValue) {
                    var exchangeRate = $('#exchangeRate').numberbox('getValue');
                    var replayPrice = $('#replyPrice').numberbox('getValue');
                    var otherRate = $('#otherRate').numberbox('getValue');
                    $('#cost').numberbox('setValue', replayPrice * exchangeRate * (1 + newValue / 100) * (1 + otherRate / 100));
                }
            });
            // 其他附加点
            $('#otherRate').numberbox({
                onChange: function (newValue, oldValue) {
                    var exchangeRate = $('#exchangeRate').numberbox('getValue');
                    var replayPrice = $('#replyPrice').numberbox('getValue');
                    var tariff = $('#tariff').numberbox('getValue');
                    $('#cost').numberbox('setValue', replayPrice * exchangeRate * (1 + tariff / 100) * (1 + newValue / 100));
                }
            });

            //表单赋值
            if ('<%=enquiry!=null%>') {
                $('#form1').form('load', {
                    reportDate: '<%=enquiry.ReportDate==new DateTime(1,1,1)?"":enquiry.ReplyDate.ToShortDateString()%>',
                    replyPrice: '<%=enquiry.ReplyPrice%>',
                    replyDate: '<%=enquiry.ReplyDate==new DateTime(1,1,1)?"":enquiry.ReplyDate.ToShortDateString()%>',
                    rfq: '<%=enquiry.RFQ%>',
                    originModel: '<%=enquiry.OriginModel%>',
                    moq: '<%=enquiry.MOQ%>',
                    mpq: '<%=enquiry.MPQ%>',
                    exchangeRate: '<%=enquiry.ExchangeRate.HasValue?enquiry.ExchangeRate:1%>',
                    taxRate: '<%=enquiry.TaxRate.HasValue?enquiry.TaxRate:13M%>',
                    tariff: '<%=enquiry.Tariff%>',
                    otherRate: '<%=enquiry.OtherRate%>',
                    cost: '<%=enquiry.Cost%>',
                    validity: '<%=enquiry.Validity==new DateTime(1,1,1)?"":enquiry.ReplyDate.ToShortDateString()%>',
                    validityCount: '<%=enquiry.ValidityCount%>',
                    salePrice: '<%=enquiry.SalePrice%>',
                    summary: '<%=enquiry.Summary%>',
                });
            }
        });
    </script>
</body>
</html>
