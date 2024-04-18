<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Declare.aspx.cs" Inherits="WebApp.Client.Message.Declare" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        var ConfigInfo = eval('(<%=this.Model.ConfigInfo%>)');
        var Spot = eval('(<%=this.Model.SpotName%>)');
        $(function () {           
            var str = '';
            str += '<table  class="border-table" style="margin-top: 5px; font-size: 12px;">';
            for (i = 0; i < Spot.length; i++){    
                str += '<tr>';
                str += '<td>';
                str += '<input type="checkbox" id="ch' + Spot[i].TypeValue + '" name="ch' + Spot[i].TypeValue + '"  class="checkbox" /><label for="ch' + Spot[i].TypeValue + '" style="margin-right: 20px">' + Spot[i].Name + '</label>';
                str += '</td>';
                str += '<td>';
                str += '推送内容';
                str += '</td>';
                str += '<td>';
                str += '<input id="Msg'+Spot[i].TypeValue+'" class="easyui-textbox" data-options="required:true, multiline:true, validType:\'length[0,200]\'," style="width: 300px; height: 62px;" />';
                str += '</td>';
                str += '</tr>';

                str += '<tr>';
                str += '<td>';               
                str += '</td>';                
                str += '<td>';            
                str += '<input type="checkbox" id="chSMS' + Spot[i].TypeValue + '" name="chSMS' + Spot[i].TypeValue + '"  class="checkbox" /><label for="chSMS' + Spot[i].TypeValue + '" style="margin-right: 20px">推送手机</label>';
                str += '</td>';
                str += '<td>';
                str += '<input id="txtSMS'+Spot[i].TypeValue+'" class="easyui-textbox"  style="width: 300px; " />';
                str += '</td>';     
                str += '</tr>';

                str += '<tr>';
                str += '<td>';               
                str += '</td>';                
                str += '<td>';            
                str += '<input type="checkbox" id="chMail' + Spot[i].TypeValue + '" name="chMail' + Spot[i].TypeValue + '"  class="checkbox" /><label for="chMail' + Spot[i].TypeValue + '" style="margin-right: 20px">推送邮箱</label>';
                str += '</td>';
                str += '<td>';
                str += '<input id="txtMail'+Spot[i].TypeValue+'" class="easyui-textbox" style="width: 300px; "  />';
                str += '</td>';     
                str += '</tr>';

                str += '<tr>';
                str += '<td>';               
                str += '</td>';                
                str += '<td>';            
                str += '<input type="checkbox" id="chWeChat' + Spot[i].TypeValue + '" name="chWeChat' + Spot[i].TypeValue + '"  class="checkbox" /><label for="chWeChat' + Spot[i].TypeValue + '" style="margin-right: 20px">推送微信</label>';
                str += '</td>';
                str += '<td>';
                str += '<input id="txtWeChat'+Spot[i].TypeValue+'" class="easyui-textbox"  style="width: 300px; " />';
                str += '</td>';     
                str += '</tr>';

                str += '<tr>';
                str += '<td>';   
                str += '&nbsp';
                str += '</td>';   
                str += '</tr>';
            }
            str += '</table>';


            $('#configInfo').append(str);

            //渲染ne
            $.parser.parse($('#configInfo'));

            for (i = 0; i < ConfigInfo.length; i++) {                        
                $('#ch' + ConfigInfo[i].TypeValue).attr("checked", true);                
                $('#Msg' + ConfigInfo[i].TypeValue).val(ConfigInfo[i].SendMsg);
                if (ConfigInfo[i].iMobile) {
                    $('#chSMS' + ConfigInfo[i].TypeValue).attr("checked", true);
                    $('#txtSMS' + ConfigInfo[i].TypeValue).val(ConfigInfo[i].Mobile);
                }
                if (ConfigInfo[i].iEmail) {
                    $('#chMail' + ConfigInfo[i].TypeValue).attr("checked", true);
                    $('#txtMail' + ConfigInfo[i].TypeValue).val(ConfigInfo[i].Email);
                }
                if (ConfigInfo[i].iWeChat) {
                    $('#chWeChat' + ConfigInfo[i].TypeValue).attr("checked", true);
                    $('#txtWeChat' + ConfigInfo[i].TypeValue).val(ConfigInfo[i].WeChatID);
                }
            }
        });


        function SaveMessage() {

            var values = FormValues("form1");
            values["ID"] = "aaaa";
            values["reason"] = "bbbb";

            MaskUtil.mask();//遮挡层
            $.post('?action=SaveMessage', { Model: JSON.stringify(values) }, function (res) {
                    MaskUtil.unmask();//关闭遮挡层
                    var result = JSON.parse(res);//
                    
                });
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'" onclick="SaveMessage()">保存</a>
        </div>
        <div id="configInfo"></div>

    </form>
</body>
</html>
