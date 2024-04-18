<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Invoice.aspx.cs" Inherits="WebApp.Client.Invoice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="http://gerpfixed.for-ic.net/My/Scripts/area.data.js"></script>
    <script src="http://gerpfixed.for-ic.net/My/Scripts/areacombo.js"></script>
    <link href="../Content/Ccs.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/address-parse.js"></script>
    <style type="text/css">
        .lbl {
            text-align: center;
        }
    </style>
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        var DeliveryType = eval('(<%=this.Model.DeliveryType%>)');

        if (ID != "") {
            var ClientInfoData = eval('(<%=this.Model.ClientInfoData != null ? this.Model.ClientInfoData:""%>)');
            var results = AddressParse.parse(ClientInfoData.Address);
            results = CheckAddressParse(results);
        }

        $(function () {
            $("#DeliveryType").combobox({
                data: DeliveryType
            });

            if ('<%=this.Model.ClientInvoiceData != null%>' == 'True') {
                ClientInvoiceData = eval('(<%=this.Model.ClientInvoiceData != null ? this.Model.ClientInvoiceData:""%>)');
                $("#Name").textbox("setValue", ClientInvoiceData["Name"]);
                $("#Taxpayer").textbox("setValue", ClientInvoiceData["Taxpayer"]);
                $("#Address").textbox("setValue", ClientInvoiceData["Address"]);
                $("#Tel").textbox("setValue", ClientInvoiceData["Tel"]);
                $("#BankName").textbox("setValue", ClientInvoiceData["BankName"]);
                $("#BankAccount").textbox("setValue", ClientInvoiceData["BankAccount"]);
                $("#DeliveryType").combobox("setValue", ClientInvoiceData["DeliveryType"]);

                $("#ConsigneeName").textbox("setValue", ClientInvoiceData.ConsigneeName);
                $("#ConsigneeMobile").textbox("setValue", ClientInvoiceData.ConsigneeMobile);
                $("#ConsigneeTel").textbox("setValue", ClientInvoiceData.ConsigneeTel);
                $("#ConsigneeEmail").textbox("setValue", ClientInvoiceData.ConsigneeEmail);
                $("#ConsigneeAddress").area("setValue", ClientInvoiceData.ConsigneeAddress);
            }
            else {
                if (ID != "") {
                    //set invoice default
                    $("#Name").textbox("setValue", ClientInfoData.CompanyName);
                    $("#Taxpayer").textbox("setValue", ClientInfoData.CompanyCode);
                    $("#Address").textbox("setValue", ClientInfoData.Address);
                    $("#Tel").textbox("setValue", ClientInfoData.Tel);
                    $("#DeliveryType").combobox("setValue", '1');

                    //地址默认信息
                    $("#ConsigneeName").textbox("setValue", ClientInfoData.ContactName);
                    $("#ConsigneeMobile").textbox("setValue", ClientInfoData.Mobile);
                    $("#ConsigneeTel").textbox("setValue", ClientInfoData.Tel);
                    $("#ConsigneeEmail").textbox("setValue", ClientInfoData.Email);

                    $("#ConsigneeAddress").area("setValue", results);//ClientInfoData.Address
                }
            }

            //
            $('#btnSave').on('click', function () {
                if (!Valid("form1")) {
                    return;
                }
                var name = $('#Name').textbox('getValue');
                if (name.indexOf("reg-") != -1) {
                    $.messager.alert("消息", "企业名称不正确，请先修改企业名称");
                    return;
                }

                var address = $('#ConsigneeAddress').area('getValue');
                if (!CheckAddress($.trim(address))) {
                    $.messager.alert("消息", "请选择发票邮寄地址");
                    return;
                }

                MaskUtil.mask();//遮挡层
                var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
                values['ClientID'] = ID;//会员ID
                //提交后台
                $.post('?action=SaveClientInvoice', { Model: JSON.stringify(values) }, function (res) {
                    MaskUtil.unmask();//关闭遮挡层
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {

                        }
                    });
                });
            });

            InitClientPage();
        });

        function CheckAddress(address) {
            if (address == "") {
                return false;
            }
            var ar = address.split(" ");
            if (ar[0] == "北京" || ar[0] == "上海" || ar[0] == "天津" || ar[0] == "重庆") {
                if (ar.length != 3) {
                    return false;
                }
            }
            else {
                if (ar.length != 4) {
                    return false;
                }
            }
            return true;
        }

        function Return() {
            var source = window.parent.frames.Source;
            var u = "";
            switch (source) {
                case 'Add':
                    u = 'New/List.aspx';
                    break;
                case 'Assign':
                    u = 'Approval/List.aspx';
                    break;
                case 'ClerkView':
                    u = 'New/List.aspx';
                    break;
                case 'ApproveView':
                    u = 'Approval/List.aspx';
                    break;
                case 'QualifiedView':
                    u = 'Control/QualifiedList.aspx';
                    break;
                case 'ServiceManagerView':
                    u = 'ServiceManagerView/List.aspx';
                    break;
                default:
                    u = 'View/List.aspx';
                    break;
            }
            var url = location.pathname.replace(/Invoice.aspx/ig, u);
            window.parent.location = url;
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
                if (results[0].area.indexOf("区") != -1) {
                    results[0].area = results[0].area.replace("区", '');
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
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server">
            <div style="margin: 8px;"> 
                <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'">保存</a>
                <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">返回</a>
            </div>
            <div style="margin-left:10px; margin-right:10px">
                <div id="info" class="easyui-panel" title="发票信息" style="width: 100%;""
                    data-options="closable:false,collapsible:false,minimizable:false,maximizable:false">
                    <table class="irtbwrite">
                        <tr>
                            <td class="lbl">名称：</td>
                            <td>
                                <input class="easyui-textbox" id="Name" style="width:350px" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',editable:false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">纳税人识别号：</td>
                            <td>
                                <input class="easyui-textbox" id="Taxpayer"  style="width:350px" data-options="required:true,validType:'length[1,50]',tipPosition:'bottom'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">地址：</td>
                            <td>
                                <input class="easyui-textbox" id="Address"  style="width:350px"  data-options="required:true,validType:'length[1,250]',tipPosition:'bottom'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">电话：</td>
                            <td>
                                <input class="easyui-textbox" id="Tel"  style="width:350px" data-options="required:true,validType:'length[1,50]',tipPosition:'bottom'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">开户行：</td>
                            <td>
                                <input class="easyui-textbox" id="BankName"  style="width:350px" data-options="required:true,validType:'length[1,250]',tipPosition:'bottom'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">账号：</td>
                            <td>
                                <input class="easyui-textbox" id="BankAccount"  style="width:350px" data-options="required:true,validType:'length[1,50]',tipPosition:'bottom'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">交付方式：</td>
                            <td>
                                <input class="easyui-combobox" id="DeliveryType" 
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true" style="width:350px" />
                            </td>
                        </tr>
                    </table>
                </div>
                <br />
                <div id="consignee" class="easyui-panel" title="发票收件地址" style="width: 100%;"
                    data-options="closable:false,collapsible:false,minimizable:false,maximizable:false">
                    <table class="irtbwrite">
                        <tr>
                            <td class="lbl">收件人：</td>
                            <td>
                                <input class="easyui-textbox" id="ConsigneeName" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom'" style="width:350px"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">地址：</td>
                            <td>
                                <table class="irtbaddress">
                                    <tr>
                                        <td>
                                            <div class="easyui-area" data-options="country:'中国',newline:true,newlinewidth:350" id="ConsigneeAddress" name="ConsigneeAddress"></div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">手机：</td>
                            <td>
                                <input class="easyui-textbox" id="ConsigneeMobile" data-options="required:true,validType:'mobile',tipPosition:'bottom'"  style="width:350px"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">电话：</td>
                            <td>
                                <input class="easyui-textbox" id="ConsigneeTel" data-options="validType:'length[1,50]',tipPosition:'bottom'"  style="width:350px"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">邮箱：</td>
                            <td>
                                <input class="easyui-textbox" id="ConsigneeEmail" data-options="validType:'length[1,50]',tipPosition:'bottom'" style="width:350px" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
