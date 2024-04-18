<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Trace.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <style>
        table {
            border-collapse: collapse;
            height: 100%;
            margin: 0 auto;
            text-align: center;
        }

        table, td, th {
            border: 1px solid black;
            font-size: larger;
        }

        th {
            width: 90px;
            color: #666;
            background-color: #CCE8EB;
        }
    </style>
    <script>
        var AllData = eval('(<%=this.Model.AllData%>)');
        var ClientName = '<%=this.Model.ClientName%>';
        var isload = false;
        var stringhtml = "";
        var nextDate = "";

        $(function () {
            InitTable();
            var isadd = getQueryString("isadd"); //隐藏新增按钮
            if (isadd) {
                $("#btnAdd").hide();
            }
            window.onload = function () {
                isload = true;
            }
        });

        //加载表格
        function InitTable() {
            $("#ClientName").html(ClientName);
            if (AllData != null) {
                for (var i = 0; i < AllData.length; i++) {
                    var Context = escape2Html(AllData[i].Context);
                    var SDate = "", SNextDate = "",CreateDate = "";
                    if (AllData[i].Date != null) {
                        SDate = new Date(AllData[i].Date).toDateStr();
                    }
                    if (AllData[i].NextDate != null) {
                        SNextDate = new Date(AllData[i].NextDate).toDateStr();
                    }
                    if (AllData[i].CreateDate != null) {
                        CreateDate = new Date(AllData[i].CreateDate).toDateStr();
                    }
                    debugger;
                    stringhtml += "<tr><th>跟进日期</th><td>" + SDate + "</td><th>跟进人</th><td>" + AllData[i].AdminName + "</td><th>跟进方式</th><td>" + AllData[i].TypeName + "</td></tr>";
                    stringhtml += "<tr><th>原厂陪同人员</th><td>" + AllData[i].OriginalStaffs + "</td><th>下次跟踪日期</th> <td colspan='3'>" + SNextDate + "</td></tr><tr><th>创建日期</th>";
                    if (AllData[i].IsLight) {
                        stringhtml += "<td style='color:red'>" + CreateDate + "</td><th>附件</th> <td colspan='3'>";
                    } else {
                        stringhtml += "<td>" + CreateDate + "</td><th>附件</th> <td colspan='3'>";
                    }
                    
                    if (AllData[i].File.length != 0) {
                        for (var j = 0; j < AllData[i].File.length; j++) {
                            if (j > 0) {
                                stringhtml += "<br>";
                            }
                            stringhtml += "<a download='" + AllData[i].File[j].Name + "'  href='" + AllData[i].File[j].URL + "' class='easyui-linkbutton'>" + AllData[i].File[j].Name + "</a>";
                        }
                    }
                    stringhtml += "</td></tr><tr><th> ";
                    if (AllData[i].Status == 100) {
                        stringhtml += "<span style='color:red'>暂存</span></br>";
                    }
                    stringhtml += "跟踪结果</br>";
                    if (!AllData[i].IsOwner) {
                        stringhtml += " <input style='visibility: visible' value='点评' type='button' id='Comment' onclick='Comment(\"" + AllData[i].ID + "\")' reportid=" + AllData[i].ID + ">";
                    }
                    if (AllData[i].IsEdit) {
                        stringhtml += "<input style='visibility: visible' value='编辑' type='button' id='Edit' onclick='Edit(\"" + AllData[i].ID + "\")' reportid=" + AllData[i].ID + ">";
                    }
                    stringhtml += "</th><td colspan='9'  style='text-align:justify;word-break: break-all;' escape=false>" + Context + "<br><br><br>";
                    if (AllData[i].Reply.length > 0) {
                        for (var j = 0; j < AllData[i].Reply.length; j++) {
                            stringhtml += "<hr>点评人:" + AllData[i].Reply[j].RealName + "&nbsp&nbsp&nbsp点评时间:" + AllData[i].Reply[j].UpdateDate.replace(/T/, ' ') + "<br><br>点评内容:" + AllData[i].Reply[j].Context;
                        }
                    }
                    stringhtml += "</td></tr>";
                }
                $("#mytable").append(stringhtml);
            }
        }

        //新增
        function Add() {
            if (!isload) {
                alert("页面加载中,请稍后操作！");
                return;
            }
            var ClientID = getQueryString("ClientID");
            var url = location.pathname.replace(/List.aspx/ig, 'Add.aspx') + "?ClientID=" + ClientID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '跟踪记录新增',
                width: '850px',
                height: '420px',
                IsEsc: false,
                onBeforeClose: function () {
                    var iframes = top.window.document.getElementsByTagName("iframe");
                    var addwindow = iframes[iframes.length - 1].contentWindow;
                    if (addwindow.Istempsave) {
                        addwindow.win.window('open');
                        return false;
                    }
                },
                onClose: function () {
                    isload = false;
                    window.location.reload();//当前页面 
                }
            }).open();
        }

        //编辑
        function Edit(id) {
            if (!isload) {
                alert("页面加载中,请稍后操作！");
                return;
            }
            var reportid = id; //跟踪记录id
            var ClientID = getQueryString("ClientID");
            var url = location.pathname.replace(/List.aspx/ig, 'Add.aspx') + "?ID=" + reportid + "&ClientID=" + ClientID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                title: "跟踪记录编辑",
                width: '850px',
                height: '420px',
                noheader: false,
                IsEsc: false,
                onBeforeClose: function () {
                    var iframes = top.window.document.getElementsByTagName("iframe");
                    var addwindow = iframes[iframes.length - 1].contentWindow;
                    if (addwindow.Istempsave) {
                        addwindow.win.window('open');
                        return false;
                    }
                },
                onClose: function () {
                    isload = false;
                    window.location.reload(); //刷新当前页面
                }
            }).open();
        }

        //点评
        function Comment(id) {
            if (!isload) {
                alert("页面加载中,请稍后操作！");
                return;
            }
            var reportid = id; //跟踪记录id
            var url = location.pathname.replace(/Trace/ig, 'WorksWeekly');
            url = url.replace(/List.aspx/ig, 'WeeklyComment.aspx') + "?ID=" + reportid + "&Type=30";
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '点评',
                width: "450px",
                height: "280px",
                onClose: function () {
                    isload = false;
                    window.location.reload(); //刷新当前页面
                }
            }).open();
        }
    </script>

</head>
<body>
    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
    <table style="width: 100%; height: auto" id="mytable">
        <tr>
            <th>客户名称</th>
            <td colspan="3" id="ClientName" style="background: #F5FAFA;"></td>
            <th></th>
            <td id="ClientOwner" style="background: #F5FAFA;"></td>
        </tr>
    </table>
</body>
</html>
