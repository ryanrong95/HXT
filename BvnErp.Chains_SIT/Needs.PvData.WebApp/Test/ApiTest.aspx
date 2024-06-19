<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApiTest.aspx.cs" Inherits="Needs.PvData.WebApp.Test.ApiTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />

    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>

    <link href="http://fix.szhxd.net/frontframe/customs-easyui/fonts/iconfont.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/customs-easyui/Styles/reset.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/customs-easyui/Styles/main.css" rel="stylesheet" />

    <link href="http://fix.szhxd.net/frontframe/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script>
        function submitToSubSystem() {
            $.ajax({
                url: 'http://apidev.for-ic.net/Classify/SubmitClassified',
                type: 'post',
                data: {
                    ItemID: 'OrderItem20190917001',
                    //其他需要提交给子系统的字段
                    Step: 1,
                    CreatorID: 'Admin00536',
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == "100") {

                    } else if (data.code == "200") {

                    } else if (data.code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function continueClassify() {
            $.ajax({
                url: 'http://apidev.for-ic.net/Classify/GetNext',
                type: 'get',
                data: {
                    step: '1',
                    creatorId: 'Admin00536'
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == "100") {

                    } else if (data.code == "200") {

                    } else if (data.code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin-top: 10px">
            <a id="btnSubmit" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'" onclick="submitToSubSystem()">提交至子系统</a>
            <a id="btnNext" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'" onclick="continueClassify()">继续归类</a>
        </div>
    </form>
</body>
</html>
