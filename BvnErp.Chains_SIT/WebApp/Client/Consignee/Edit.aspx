<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.Consignee.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="http://fix.szhxd.net/My/Scripts/area.data.js"></script>
    <script src="http://fix.szhxd.net/My/Scripts/areacombo.js"></script>
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/address-parse.js"></script>
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script type="text/javascript">
        debugger;
        var ID = '<%=this.Model.ID%>';
        var ClientConsigneeID = '<%=this.Model.ClientConsigneeID%>';
        var Count = '<%=this.Model.Count%>';

        if ( Count == 0 ) {
            var ClientInfoData = eval('(<%=this.Model.ClientInfoData != null ? this.Model.ClientInfoData:"" %>)');
            var results = AddressParse.parse(ClientInfoData.Address);
            results = CheckAddressParse(results);
        }

        if (ClientConsigneeID != '') {
            ClientConsigneeData = eval('(<%=this.Model.ClientConsigneeData != null ? this.Model.ClientConsigneeData:""%>)');
        }

        $(function () {
            if (Count == 0) {
                $("#Name").textbox("setValue", ClientInfoData.CompanyName);
                $("#Address").area("setValue", results);//ClientInfoData.Address
                $("#ContactName").textbox("setValue", ClientInfoData.ContactName);
                $("#Mobile").textbox("setValue", ClientInfoData.Mobile);
                $('#IsDefault').prop('checked', ClientInfoData.IsDefault);
            }
            if (ClientConsigneeID != '') {
                $("#Name").textbox("setValue", ClientConsigneeData.Name);
                $("#CompanyName").textbox("setValue", ClientConsigneeData.CompanyName);
                $("#Address").area("setValue", ClientConsigneeData.Address);
                $("#ContactName").textbox("setValue", ClientConsigneeData.ContactName);
                $("#Mobile").textbox("setValue", ClientConsigneeData.Mobile);
                $("#Email").textbox("setValue", ClientConsigneeData.Email);
                $("#Summary").textbox("setValue", ClientConsigneeData.Summary);
                $('#IsDefault').prop('checked', ClientConsigneeData.IsDefault);
            }

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                if ($("#Address").area("getValue").split(' ').length < 3) {
                    $.messager.alert("消息", "请选择收件地址");
                    return;
                }

                var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
                values['ClientID'] = ID;//会员ID
                values['ClientConsigneeID'] = ClientConsigneeID;//收件地址ID
                //提交后台
                $.post('?action=SaveClientConsignee', { Model: JSON.stringify(values) }, function (res) {
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {
                            closeWin();
                        }
                    });
                });
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });

        });

        function closeWin() {
            $.myWindow.close();
        }
          //地址智能解析
        function CheckAddressParse(results) {
            if (results[0].province.indexOf("省") != -1) {
                results[0].province = results[0].province.replace("省", '');
            }
            else if (results[0].province.indexOf("区") != -1) {
                if (results[0].province == "内蒙古自治区" || results[0].province == "西藏自治区") {
                    results[0].province = results[0].province.replace("自治区", '');
                }
                else if (results[0].province == "新疆维吾尔自治区") {
                    results[0].province = results[0].province.replace("维吾尔自治区", '');
                }
                else if (results[0].province == "广西壮族自治区") {
                    results[0].province = results[0].province.replace("壮族自治区", '');
                }
                else if (results[0].province == "宁夏回族自治区") {
                    results[0].province = results[0].province.replace("回族自治区", '');
                }
            }
            if (results[0].city.indexOf("市") != -1) {
                results[0].city = results[0].city.replace("市", '');
            }
            if (results[0].province == results[0].city) {
                if (results[0].area.indexOf("区") !=-1 ) {
                results[0].area = results[0].area.replace("区",'');
                }
                results = results[0].province + ' ' + results[0].area + ' ' + results[0].details;
            }
            else {
                results = results[0].province + ' ' + results[0].city + ' ' + results[0].area + ' ' + results[0].details;
            }
            return results; 
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table class="irtbwrite" style="width: 100%;">
                <tr>
                    <td class="lbl">收货单位：</td>
                    <td>
                        <input class="easyui-textbox" id="Name" data-options="required:true,validType:'length[1,200]',tipPosition:'bottom'" style="width: 460px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收件人：</td>
                    <td>
                        <input class="easyui-textbox" id="ContactName" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom'" style="width: 460px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">电话：</td>
                    <td>
                        <input class="easyui-textbox" id="Mobile" data-options="required:true,tipPosition:'bottom'" style="width: 460px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">地址：</td>
                    <td>
                        <table class="irtbaddress">
                            <tr>
                                <td>
<%--                                    <div class="easyui-area" data-options="country:'中国',newline:'true',tipPosition:'bottom',newlinewidth:'460',width:'460px'" id="Address" name="Address"></div>--%>
                                     <div class="easyui-area" data-options="country:'中国',newline:true,newlinewidth:460" id="Address" name="Address"></div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">电子邮箱：</td>
                    <td>
                        <input class="easyui-textbox" id="Email" data-options="validType:'email',tipPosition:'bottom'" style="width: 460px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" data-options="validType:'length[1,400]',tipPosition:'bottom',multiline:true" style="width: 460px;height: 60px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl"></td>
                    <td style="text-align: left">
                        <input type="checkbox" id="IsDefault" name="IsDefault" checked="checked" /><label for="IsDefault" style="margin-right: 30px">默认收件地址</label>
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" >取消</a>
    </div>
</body>
</html>
