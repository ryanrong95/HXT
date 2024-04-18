<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Declare.aspx.cs" Inherits="WebApp.Declaration.Declare.Declare" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var id = '<%=this.Model.ID%>';
        var Source = '<%=this.Model.Source%>';
        var SourcePage = "";
        $(function () {
            $('#tt').tabs({
                border: false,
                tabWidth: 120,
            });

            var isadd = id == "" ? true : false;
            var OrderId = getQueryString("OrderID");
            var ClientID = getQueryString("ClientID");
            var NoticeID = getQueryString("NoticeID");
            var TotalPack = getQueryString("TotalPack");
            SourcePage = getQueryString("SourcePage");          
            if (Source == "Add") {
                addTab("报关单表头", "DecHead.aspx?ID=" + id + "&OrderId=" + OrderId + "&ClientID=" + ClientID + "&NoticeID=" + NoticeID + "&SourcePage=" + SourcePage + "&Source=" + Source + "&TotalPack=" + TotalPack, "DecHead", false);
            }
            else {
                addTab("报关单表头", "DecHead.aspx?ID=" + id + "&OrderId=" + OrderId + "&ClientID=" + ClientID + "&NoticeID=" + NoticeID + "&SourcePage=" + SourcePage + "&Source=" + Source, "DecHead", false);
                addTab("报关单表体", "DecList.aspx?ID=" + id, "List", isadd);
                addTab("检验检疫", "DecCIQ.aspx?ID=" + id, "CIQ", isadd);
                addTab("电子随附单据", "DecEdocRealation.aspx?ID=" + id, "Edoc", isadd);
                addTab("其它随附单证", "DecRequestCert.aspx?ID=" + id, "RequestCert", isadd);
                addTab("集装箱", "DecContainer.aspx?ID=" + id, "Container", isadd);
                addTab("报关单附件", "DecFile.aspx?ID=" + id, "File", isadd);
                addTab("操作记录", "DecTrace.aspx?ID=" + id, "Trace", isadd);
            }


            $('#tt').tabs('select', 0);//第一个选中
        });

        function addTab(title, href, id, dis) {
            var tt = $('#tt');
            if (tt.tabs('exists', title)) {
                //如果tab已经存在,则选中并刷新该tab                   
                tt.tabs('select', title);
                refreshTab({ tabTitle: title, url: href });
            } else {
                var content = "";
                if (href) {
                    content = '<iframe id="' + id + '" scrolling="yes" frameborder="0" data-source="' + Source + '"  src="' + href + '"style="width:100%;height:100%;"></iframe>';
                }
                else {
                    content = '未实现';
                }
                tt.tabs('add', {
                    title: title,
                    closable: false,
                    content: content,
                    disabled: dis,
                });

                $('#tt').find('.panel-body').css("overflow-y", "hidden");
            }
        }
    </script>

    <script>

        //约定示例
        top.$.backLiebiao = {
            kkk: 1,
            Ruturn: function () {
                var url = "";

                if (id == "" ) {
                    url = 'Notice/List.aspx';
                }
                else {
                    if (SourcePage == "Maked") {
                        url = "Declare/MakedList.aspx";                       
                    } else if (SourcePage == "Imported") {
                        url = 'Declare/ImportedList.aspx';
                    } else if (SourcePage == "Excel") {
                        url = 'Declare/ExcelList.aspx';
                    } else if (SourcePage == "Checking") {
                        url = 'Declare/CheckingList.aspx';
                    }else if (SourcePage == "Checked") {
                        url = 'Declare/CheckedList.aspx';
                    }else {
                        url = 'Declare/List.aspx';
                    }
                }
                var u = location.pathname.replace(/Declare\/Declare.aspx/ig, url);
                window.location = u;
            }
        };

    </script>
</head>
<body>
    <div>
        <div id="tt" class="easyui-tabs" style="height: 630px; overflow: hidden;">
        </div>
    </div>
</body>
</html>
