<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Anomaly.aspx.cs" Inherits="WebApp.Classify.Product.Anomaly" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>归类异常</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script type="text/javascript">
        $(function () {

        });


        function Save() {
            if (!Valid()) {
                return;
            }

            var id = getQueryString('ID');
            var from = getQueryString('From');
            var reason = $('#reason').textbox('getValue');
            MaskUtil.mask();
            $.post('?action=ClassifyAnomaly', { ID: id, From: from, Reason: reason }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert('提示', result.message, 'info', function () {
                        if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.Step1.GetHashCode()%>') {
                            url = location.pathname.replace(/Anomaly.aspx/ig, 'FirstList.aspx');
                        } else if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.Step2.GetHashCode()%>') {
                            url = location.pathname.replace(/Anomaly.aspx/ig, 'SecondList.aspx');
                        } else if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.ReClassify.GetHashCode()%>') {
                            url = location.pathname.replace(/Anomaly.aspx/ig, '../ProductChange/UnProcessList.aspx');
                        } else if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.DoneEdit.GetHashCode()%>') {
                            url = location.pathname.replace(/Anomaly.aspx/ig, 'DoneList.aspx');
                        }
                        window.parent.location = url;
                    });
                } else {
                    $.messager.alert('归类', result.message);
                }
            });
        }

        //关闭
        function Close() {
            self.parent.$('iframe').parent().window('close');
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true" style="margin-top: 10px">
        <form id="form1" runat="server" method="post">
            <div data-options="region:'center',border:false" style="text-align: center">
                <input id="reason" class="easyui-textbox" data-options="multiline:true,required:true,tipPosition:'bottom',prompt:'请填写归类异常原因',validType:'length[1,400]'" style="width: 350px; height: 150px" />
            </div>

            <div id="divSave" style="text-align: center; margin-top: 40px">
                <a id="btnAnomaly" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
                <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
            </div>
        </form>
    </div>
</body>
</html>
