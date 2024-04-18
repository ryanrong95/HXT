<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SplitModel.aspx.cs" Inherits="WebApp.HKWarehouse.Sorting.SplitModel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑产地信息</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        //原产地数据
        var originData = eval('(<%=this.Model.OriginData%>)');
        var OrderItemData = eval('(<%=this.Model.OrderItemData%>)');

        $(function () {
            if (OrderItemData != null && OrderItemData.Model != null) {
                OrderItemData.Model = OrderItemData.Model.replace(/<%=this.Model.ReplaceQuotes%>/, '"');
            }

            $('#Origin').combobox({
                data: originData,
            });

            Init();
        });

        function Init() {
            if (OrderItemData != null && OrderItemData != "") {
                $('#Origin').combobox("setValue", OrderItemData.Origin);
                $('#Model').textbox('setValue', OrderItemData.Model);
                $('#Manufacturer').textbox("setValue", OrderItemData.Manufacturer);

            }
        }

        function Close() {
           $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            if ($('#Origin').combobox("getValue") == OrderItemData.Origin) {
                $.messager.alert("消息", "请变更产地");
                return;
            }
            var Qty = getQueryString('Qty');
            
            if (Number($("#Quantity").textbox("getValue")) <= 0 || Number($("#Quantity").textbox("getValue")) >= Number(Qty))
            {
                $.messager.alert("消息", "拆分数量必须介于0到" + Qty +"之间");
                return;

            }
            var data = new FormData($('#form1')[0]);
            var OrderItemID = getQueryString('OrderItemID');
           
            
            data.append("OrderItemID", OrderItemID);
            $.ajax({
                url: '?action=SplitModelData',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                }
            }).done(function (res) {
                if (res.success) {
                    $.messager.alert('提示', res.message, 'info', function () {
                      $.myWindow.close();
                    });
                } else {
                    $.messager.alert('提示', res.message);
                }
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin: 0 auto; height: 120px">
                 <tr>
                    <td class="lbl">型号：</td>
                    <td>
                        <input class="easyui-textbox input" id="Model" name="Model" disabled: disabled,
                            data-options="disabled: true, required:true,validType:'length[1,50]',height:26,width:200" />
                    </td>
                </tr>
                 <tr>
                    <td class="lbl">品牌：</td>
                    <td>
                        <input class="easyui-textbox input" id="Manufacturer" name="Manufacturer"
                            data-options="required:true,validType:'length[1,50]',height:26,width:200" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">产地：</td>
                    <td>
                        <input class="easyui-combobox input" id="Origin" name="Origin"
                            data-options="valueField:'OriginValue',textField:'OriginText',required:true,height:26,width:200,validType:'comboBoxEditValid[\'Origin\']'" />
                    </td>
                </tr>

                <tr>
                       <td class="lbl">数量：</td>
                    <td>
                        <input class="easyui-textbox" id="Quantity" name="Quantity" data-options="required:true,height:26,width:200" />
                    </td>
                </tr>
               
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">拆分</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
