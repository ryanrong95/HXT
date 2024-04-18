<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportShow.aspx.cs" Inherits="WebApp.Crm.Project_bak.ReportShow" %>

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
        var ProjectName = '<%=this.Model.ProjectName%>';
        var ProjectOwner = '<%=this.Model.ProjectOwner%>';
        var stringhtml = "";

        $(function () {
            InitTable();
        });


        //加载表格
        function InitTable() {
            $("#ProjectName").html(ProjectName);
            $("#ProjectOwner").html(ProjectOwner);
            if (AllData != null) {
                for (var i = 0; i < AllData.length; i++) {
                    var Context = escape2Html(AllData[i].Context);
                    var Plan = escape2Html(AllData[i].Plan);
                    var SDate = "", SNextDate = "";
                    if (isNaN(Date(AllData[i].Date))) {
                        SDate = AllData[i].Date.replace(/T.+$/, '');
                    } else {
                        SDate = new Date(AllData[i].Date).toDateStr();
                    }
                    if (isNaN(Date(AllData[i].NextDate))) {
                        SNextDate = AllData[i].NextDate.replace(/T.+$/, '');
                    } else {
                        SNextDate = new Date(AllData[i].NextDate).toDateStr();
                    }
                    stringhtml += "<tr><th>跟进日期</th><td>" + SDate + "</td><th>跟进人</th><td>" + AllData[i].AdminName + "</td><th>跟进方式</th><td>" +
                        AllData[i].TypeName + "</td><th>原厂陪同人员</th><td>" + AllData[i].OriginalStaffs + "</td></tr>";
                    stringhtml += "<tr><th>下次跟进日期</th><td>" + SNextDate + "</td><th>下次跟进方式</th><td>" + AllData[i].NextTypeName + "</td><th>附件</th><td  colspan='3'>";
                    if (AllData[i].File.length != 0) {
                        for (var j = 0; j < AllData[i].File.length; j++) {
                            if (j > 0) {
                                stringhtml += "<br>";
                            }
                            stringhtml += "<a download='" + AllData[i].File[j].Name + "'  href='" + AllData[i].File[j].URL + "' class='easyui-linkbutton'>" + AllData[i].File[j].Name + "</a>";
                        }
                    }
                    stringhtml += "</td></tr > <tr><th>跟踪结果";
                    if (!AllData[i].IsOwner) {
                        stringhtml += "<input style='visibility:visible' value='点评' type='button' id='Comment' onclick='Comment(\"" + AllData[i].ID + "\")' reportid=" + AllData[i].ID + ">";
                    }
                    stringhtml += "</th><td colspan='7' style='text-align:justify' escape=false>跟踪结果" + Context + "<br><br>行动计划:" + Plan + "<br><br><br>";
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

        //点评
        function Comment(id) {
            var reportid = id; //跟踪记录id
            var url = location.pathname.replace(/Project/ig, 'WorksWeekly');
            url = url.replace(/ReportShow.aspx/ig, 'WeeklyComment.aspx') + "?ID=" + reportid + "&Type=20";
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '点评',
                width: "420px",
                height: "220px",
                onClose: function () {
                    window.location.reload(); //刷新当前页面
                }
            }).open();
        }
    </script>

</head>
<body>
    <table style="width: 100%; height: auto" id="mytable">
        <tr>
            <th>销售机会名称</th>
            <td colspan="3" id="ProjectName" style="background: #F5FAFA;"></td>
            <th>销售机会所有人</th>
            <td colspan="3" id="ProjectOwner" style="background: #F5FAFA;"></td>
        </tr>
    </table>
</body>
</html>
