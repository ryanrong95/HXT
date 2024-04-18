<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebApp.GeneralManage.Profit.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script type="text/javascript">

        var Source = "Add";
        var tabInfos = []; //用于保存tab信息, 如果需要强制刷新, 则从这里取出信息

        $(function () {
            $('#tt').tabs({
                border: false,
                tabWidth: 120,
                onSelect: function (title, index) {
                    var userAgent = navigator.userAgent;
                    if (userAgent.indexOf("Firefox") > -1) {
                        for (var i = 0; i < tabInfos.length; i++) {
                            if (tabInfos[i].title == title) {
                                tabInfos[i].mandot = tabInfos[i].mandot + 1;
                                break;
                            }
                        }

                        if (title != "") {
                            var themandot = 100;
                            var thecontent = "";

                            for (var i = 0; i < tabInfos.length; i++) {
                                if (tabInfos[i].title == title) {
                                    themandot = tabInfos[i].mandot;
                                    thecontent = tabInfos[i].content;
                                    break;
                                }
                            }

                            if (themandot <= 1) {
                                var tab = $('#tt').tabs('getTab', index);
                                var content = thecontent;

                                $('#tt').tabs('update', {
                                    tab: tab,
                                    options: {
                                        title: title,
                                        content: content,
                                    }
                                });
                            }
                        }
                    }
                },
            });

            var isadd = false;
            if (!isadd) {
                addTab("业务员提成", "List.aspx", "ServiceManager", false);
                addTab("跟单员提成", "MerchandiserList.aspx", "Merchandiser", isadd);
            }
            else {
                addTab("业务员提成", "List.aspx", "ServiceManager", true);
                addTab("跟单员提成", "MerchandiserList.aspx", "Merchandiser", isadd);
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
                    content = '<iframe id="' + id + '" scrolling="no" frameborder="0" data-source="' + Source + '"  src="' + href + '"style="width:100%;height:900px;"></iframe>';
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

                //将tab信息添加到 tabInfos 中
                tabInfos.push({
                    title: title,
                    content: content,
                    mandot: 0,
                });
            }
        }
    </script>
</head>
<body>
    <div>
        <div id="tt" class="easyui-tabs">
        </div>
    </div>
</body>
</html>
