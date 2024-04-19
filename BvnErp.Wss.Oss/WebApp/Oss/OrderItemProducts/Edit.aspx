<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Oss.OrderItemProducts.Edit" %>

<%@ Import Namespace="Needs.Utils.Serializers" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑产品项</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <script>
        $(function () {
            var model = eval('(<%=(this.Model as NtErp.Wss.Oss.Services.Models.OrderItem).Json()%>)');
            $('#form1').form('load', model);
            <%-- $('#_price').numberbox('setValue', '<%=OrderItem.Price %>');
            $('#_count').numberbox('setValue', '<%=OrderItem.Count %>');--%>
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <%var model = this.Model as NtErp.Wss.Oss.Services.Models.OrderItem; %>
        <div class="easyui-panel" title="产品基本信息" style="width: 100%;" data-options="border:false">
            <table class="liebiao">
                <tr>
                    <th style="width: 66px">型号</th>
                    <td>
                        <input type="text" style="width: 200px;" name="txtProductName" class="easyui-textbox"
                            value="<%=model?.Product?.Name %>"
                            data-options="required:true,validType:'length[0,200]'" />
                    </td>
                </tr>
                <tr>
                    <th>制造商</th>
                    <td>
                        <input type="text" style="width: 200px;" name="txtManufacturerName" class="easyui-textbox"
                            value="<%=model?.Product?.Manufacturer?.Name %>"
                            data-options="required:true,validType:'length[0,200]'" />
                    </td>
                </tr>
                <tr>
                    <th>批号</th>
                    <td>
                        <input type="text" style="width: 200px;" name="txtDateCode" class="easyui-textbox"
                            value="<%=model?.Product?.DateCode %>"
                            data-options="required:true,validType:'length[0,200]'" />
                    </td>
                </tr>

                <tr>
                    <th>包装</th>
                    <td>
                        <input type="text" name="txtPackaging" style="width: 200px;" class="easyui-textbox"
                            value="<%=model?.Product?.Packaging %>"
                            data-options="validType:'length[0,200]'" />
                    </td>
                </tr>
                <tr>
                    <th>封装</th>
                    <td>
                        <input type="text" name="txtPackageCase" style="width: 200px;" class="easyui-textbox"
                            value="<%=model?.Product?.PackageCase %>"
                            data-options="validType:'length[0,200]'" />
                    </td>
                </tr>

            </table>
        </div>
        <div class="easyui-panel" title="订单项信息" style="width: 100%;" data-options="border:false">
            <table class="liebiao">
                <tr>
                    <th style="width: 66px">供应商</th>
                    <td>
                        <input type="text" style="width: 200px;" name="txtSupplierName" class="easyui-textbox"
                            value="<%=model?.Supplier?.Name %>"
                            data-options="required:true,validType:'length[0,200]'" />
                    </td>

                </tr>
                <tr>
                    <th>价格</th>
                    <td>
                        <input type="text" id="txtUnitPrice" name="txtUnitPrice" style="width: 200px;" class="easyui-numberbox"
                            value="<%=model?.UnitPrice %>"
                            data-options="precision:4,required:true,max:100000,min:0" />
                    </td>

                </tr>
                <tr>
                    <th>数量</th>
                    <td>
                        <input type="text" id="txtQuantity" name="txtQuantity" style="width: 200px;" class="easyui-numberbox"
                            value="<%=model?.Quantity %>"
                            data-options="precision:0,required:true,min:1,max:10000000," />
                    </td>

                </tr>
                <tr>
                    <th>货期</th>
                    <td>
                        <input type="text" name="txtLeadtime" style="width: 200px;" class="easyui-textbox"
                            value="<%=model?.Leadtime %>"
                            data-options="required:true,validType:'length[0,200]'" />
                    </td>

                </tr>
                <tr>
                    <th>备注</th>
                    <td>
                        <input name="txtNote" type="text" style="width: 300px; height: 100px;" class="easyui-textbox"
                            value="<%=model?.Note %>"
                            data-options="multiline:true,validType:'length[0,300]'" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center;">
                        <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" Text="保存" OnClick="btnSubmit_Click" />
                        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$.myWindow.close();">关闭</a>
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <input runat="server" type="hidden" id="hSuccess" value="保存成功" />
</body>
</html>
