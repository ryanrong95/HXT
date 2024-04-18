<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.StandardProducts.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var vendorData = eval('(<%=this.Model.VendorData%>)');
        //页面加载时
        $(function () {
            //下拉框加载数据
            $("#VendorID").combobox({
                data: vendorData,
                onHidePanel: function () {
                    var val = $(this).combobox("getValue");  //当前combobox的值
                    var allData = $(this).combobox("getData");   //获取combobox所有数据
                    var index = $.easyui.indexOfArray(allData, "ID", val);
                    if (index < 0) {
                        $(this).combobox("clear");
                    }
                }
            });

            //校验输入框内容
            $("#VendorID").combobox("textbox").bind("blur", function () {
                var value = $("#VendorID").combobox("getValue");
                var data = $("#VendorID").combobox("getData");
                var valuefiled = $("#VendorID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#VendorID").combobox("clear");
                }
            });
        });

        //关闭当前弹框
        function Close() {
            $.myWindow.close();
        }

    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto">
            <tr style="height: 30px">
                <td class="lbl" style="text-align: center; width: 70px">型号</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name"
                        data-options="required:true,validType:'length[1,150]'" style="width: 150px" />
                </td>
                <td class="lbl" style="text-align: center; width: 70px">品牌</td>
                <td>
                    <input class="easyui-combobox" id="VendorID" name="VendorID"
                        data-options="valueField:'ID',textField:'Name',required:true, panelMaxHeight:'100px'," style="width: 150px" />
                </td>
                <td class="lbl" style="text-align: center; width: 70px">原产地</td>
                <td>
                    <input class="easyui-textbox" id="Origin" name="Origin"
                        data-options="validType:'length[1,150]'" style="width: 150px" />
                </td>
            </tr>
            <tr style="height: 30px">
                <td class="lbl" style="text-align: center; width: 70px">包装</td>
                <td>
                    <input class="easyui-textbox" id="Packaging" name="Packaging"
                        data-options="validType:'length[1,50]'" style="width: 150px" />
                </td>
                <td class="lbl" style="text-align: center; width: 70px">封装</td>
                <td>
                    <input class="easyui-textbox" id="PackageCase" name="PackageCase"
                        data-options="validType:'length[1,50]'" style="width: 150px" />
                </td>
                <td class="lbl" style="text-align: center; width: 70px">批次</td>
                <td>
                    <input class="easyui-textbox" id="Batch" name="Batch"
                        data-options="validType:'length[1,50]'" style="width: 150px" />
                </td>
            </tr>
            <tr style="height: 30px">
                <td class="lbl" style="text-align: center; width: 70px">封装批次</td>
                <td>
                    <input class="easyui-textbox" id="DateCode" name="DateCode"
                        data-options="validType:'length[1,50]'" style="width: 150px" />
                </td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <asp:Button ID="btnSumit" Text="保存" runat="server" OnClientClick="return Valid();" OnClick="btnSave_Click" />
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()" />
        </div>
    </form>
</body>
</html>
