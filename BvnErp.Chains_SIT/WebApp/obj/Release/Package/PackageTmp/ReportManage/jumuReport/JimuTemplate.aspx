<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JimuTemplate.aspx.cs" Inherits="WebApp.ReportManage.jumuReport.JimuTemplate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <style>
        .excel-list-add a{
            color: #00BFFF;
            flex-direction: column;
            display: flex;
            justify-content: center;
            align-items: center;
        }
        .excel-list-add a:hover{
            color:#2d8cf0;
        }
        .excel-view-item{
            position: relative;
            margin: 16px;
            display: flex;
            flex-direction: column;
            width: 229px;
            height: 175px;
            -webkit-box-shadow: 0 0 6px #000000;
            box-shadow: 0 0 6px #000000;
            border-radius: 5px;
            overflow: hidden;
            float:left;
        }
        .imgbg {
        max-width:100%;
        max-height:100%;
        }
    </style>
    <script>
        $(function () {
            var info = eval('<%=this.Model.urls%>');             
            var str = '';
            for (i = 0; i < info.length; i++) {               
                str += '<div class="excel-view-item excel-list-add">';
                str += '<img class="imgbg" src="../../Content/images/excel.jpg">';
                str += '<a href="' + info[i].reportUrl + '" target="_blank">' + info[i].reportName + '</a>';
                str += '</div>';               
             }
             $('#report').append(str);
         });
    </script>
</head>
<body>
    <div id="report">
    </div>
</body>
</html>
