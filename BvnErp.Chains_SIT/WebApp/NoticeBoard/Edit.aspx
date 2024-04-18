<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.NoticeBoard.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="umeditor-master/themes/default/_css/umeditor.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="umeditor-master/third-party/jquery.min.js"></script>
    <script type="text/javascript" src="umeditor-master/third-party/template.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="umeditor-master/umeditor.config.js"></script>
    <script type="text/javascript" charset="utf-8" src="umeditor-master/_examples/editor_api.js"></script>
    <script type="text/javascript" src="umeditor-master/lang/zh-cn/zh-cn.js"></script>
    <style type="text/css">
        .btn {
            display: inline-block;
            *display: inline;
            padding: 4px 12px;
            margin-bottom: 0;
            *margin-left: .3em;
            font-size: 14px;
            line-height: 20px;
            color: #333333;
            text-align: center;
            text-shadow: 0 1px 1px rgba(255, 255, 255, 0.75);
            vertical-align: middle;
            cursor: pointer;
            background-color: #f5f5f5;
            *background-color: #e6e6e6;
            background-image: -moz-linear-gradient(top, #ffffff, #e6e6e6);
            background-image: -webkit-gradient(linear, 0 0, 0 100%, from(#ffffff), to(#e6e6e6));
            background-image: -webkit-linear-gradient(top, #ffffff, #e6e6e6);
            background-image: -o-linear-gradient(top, #ffffff, #e6e6e6);
            background-image: linear-gradient(to bottom, #ffffff, #e6e6e6);
            background-repeat: repeat-x;
            border: 1px solid #cccccc;
            *border: 0;
            border-color: #e6e6e6 #e6e6e6 #bfbfbf;
            border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);
            border-bottom-color: #b3b3b3;
            -webkit-border-radius: 4px;
            -moz-border-radius: 4px;
            border-radius: 4px;
            filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ffffffff', endColorstr='#ffe6e6e6', GradientType=0);
            filter: progid:DXImageTransform.Microsoft.gradient(enabled=false);
            *zoom: 1;
            -webkit-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05);
            -moz-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05);
            box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05);
        }

            .btn:hover,
            .btn:focus,
            .btn:active,
            .btn.active,
            .btn.disabled,
            .btn[disabled] {
                color: #333333;
                background-color: #e6e6e6;
                *background-color: #d9d9d9;
            }

            .btn:active,
            .btn.active {
                background-color: #cccccc \9;
            }

            .btn:first-child {
                *margin-left: 0;
            }

            .btn:hover,
            .btn:focus {
                color: #333333;
                text-decoration: none;
                background-position: 0 -15px;
                -webkit-transition: background-position 0.1s linear;
                -moz-transition: background-position 0.1s linear;
                -o-transition: background-position 0.1s linear;
                transition: background-position 0.1s linear;
            }

            .btn:focus {
                outline: thin dotted #333;
                outline: 5px auto -webkit-focus-ring-color;
                outline-offset: -2px;
            }

            .btn.active,
            .btn:active {
                background-image: none;
                outline: 0;
                -webkit-box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.15), 0 1px 2px rgba(0, 0, 0, 0.05);
                -moz-box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.15), 0 1px 2px rgba(0, 0, 0, 0.05);
                box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.15), 0 1px 2px rgba(0, 0, 0, 0.05);
            }

            .btn.disabled,
            .btn[disabled] {
                cursor: default;
                background-image: none;
                opacity: 0.65;
                filter: alpha(opacity=65);
                -webkit-box-shadow: none;
                -moz-box-shadow: none;
                box-shadow: none;
            }

        .edui-container {
            margin-left: 10px;
        }

        input, button {
            border: none;
            outline: none;
        }

        .tl-price-input {
            width: 100%;
            border: 1px solid #ccc;
            padding: 4px 0;
            background: white;
            border-radius: 3px;
            padding-left: 5px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
            box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
            -webkit-transition: border-color ease-in-out .15s,-webkit-box-shadow ease-in-out .15s;
            -o-transition: border-color ease-in-out .15s,box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s,box-shadow ease-in-out .15s
        }

            .tl-price-input:focus {
                border-color: #66afe9;
                outline: 0;
                -webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,.075),0 0 8px rgba(102,175,233,.6);
                box-shadow: inset 0 1px 1px rgba(0,0,0,.075),0 0 8px rgba(102,175,233,.6)
            }
    </style>
    <script type="text/javascript">
        var adminRole = eval('(<%=this.Model.AdminRole%>)');
        var noticeBoard = eval('(<%=this.Model.NoticeBoard%>)');
        $(function () {

            if (noticeBoard != "") {
                //$("#Titel").css("display", "none");
                //$("#listitem").css("display", "none");
                //$("#lblTitel").css("display", "block");
                //$("#lbllistitem").css("display", "block");
                $("#editTable2").css("display", "block");
                $("#editTable").css("display", "none");
                document.getElementById("lblTitel").innerHTML = noticeBoard.NoticeTitle;
                document.getElementById("lbllistitem").innerHTML = noticeBoard.RoleName;
                document.getElementById("MyDiv").innerHTML = html_decode(noticeBoard.NoticeContent);
                UM.getEditor('myEditor').setHide();

            }
            else {
                //哪些角色可见
                if (adminRole.length > 0) {
                    for (var i = 0; i < adminRole.length; i++) {
                        var item = adminRole[i];
                        $('#listitem').append('<option label="' + item.RoleName + '" value="' + item.RoleID + '"></option>');
                    }

                }
            }

            function html_decode(str) {
                var s = "";
                if (str.length == 0)
                    return "";
                s = str.replace(/&lt;/g, "<");
                s = s.replace(/&gt;/g, ">");
                s = s.replace(/&nbsp;/g, " ");
                s = s.replace(/&#39;/g, "\'");
                s = s.replace(/&quot;/g, "\"");
                s = s.replace(/<br>/g, "\n");
                s = s.replace(/&nbsp;/g, " ");
                s = s.replace(/&amp;/g, "&");
                return s;
            }
        })
    </script>
</head>
<body class="easyui-layout">
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true,closable:true,onClose:function(){$.myWindow.close();}" style="margin-top: 10px">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl" style="text-align: right">标题：</td>
                    <td style="width: 180px; float: left">
                        <input class="tl-price-input" id="Titel" type="text" />
                    </td>
                    <td class="lbl" style="text-align: right">角色：</td>
                    <td style="width: 180px; float: left">
                        <select id="listitem" class="tl-price-input">
                        </select>
                    </td>
                    <td>
                       <input type='button' class="btn" onclick="Save()" id="btnSave"  value="保存" />
                    </td>
                </tr>
            </table>
            <table id="editTable2" style="width: 100%; display: none">
                <tr>
                    <td class="lbl" style="text-align: left">标题：                       
                        <label id="lblTitel"></label>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: left">角色： 
                        <label id="lbllistitem"></label>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: left">发布内容：<div id="MyDiv" style="margin-left: 60px;">
                    </div>
                    </td>
                </tr>

            </table>
        </form>
    </div>
    <!--style给定宽度可以影响编辑器的最终宽度-->
    <script type="text/plain" id="myEditor" style="width: 1000px; height: 640px; margin-left: 10px">

    </script>
    <div class="clear"></div>
    <script type="text/javascript">

        //按钮的操作
        function Save() {
            var noticeTitle = $('#Titel').val().replace('&amp;', '&');
            //获取页面内容
            var noticeContent = UM.getEditor('myEditor').getContent();//html_encode();

            var roleName = $("#listitem").find("option:selected").attr("label");
            var roleID = $("#listitem").val();
            if (noticeTitle == "") {
                alert('公告标题不能为空！');
                return false;
            }
            else {
                if (noticeTitle.length > 50) {
                    return window.alert('公告标题长度不能超过50个字符！');
                    //
                    // return false;
                }
            }
            if (roleID == "") {
                alert('角色不能为空！');
                return false;
            }
            var success = confirm('请您确认是否提交？');
            if (success) {
                $.post('?action=Save', {
                    NoticeTitle: noticeTitle,
                    NoticeContent: noticeContent,
                    RoleID: roleID,
                    RoleName: roleName
                }, function (result) {
                    var rel = JSON.parse(result);
                    alert('发布成功！');
                });
            }
        }
        function html_encode(str) {
            var s = "";
            if (str.length == 0)
                return "";
            s = str.replace(/&/g, "&amp;");
            s = s.replace(/</g, "&lt;");
            s = s.replace(/>/g, "&gt;");
            s = s.replace(/ /g, "&nbsp;");
            s = s.replace(/\'/g, "&#39;");
            s = s.replace(/\"/g, "&quot;");
            s = s.replace(/\n/g, "<br>");
            return s;
        }

        //实例化编辑器
        window.UMEDITOR_HOME_URL = "/foricadmin/NoticeBoard/";
        var um = UM.getEditor('myEditor', {
            imageUrl: '?action=uploadImages',
            imagePath: "/foricadmin/Files/"

        });
    </script>

</body>
</html>
