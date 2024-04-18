<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Risk.aspx.cs" Inherits="WebApp.Client.Risk" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../Content/Ccs.css" rel="stylesheet" />
    <script type="text/javascript">
        var id = '<%=this.Model.ID%>';
        var Source = '<%=this.Model.Source%>';
        var CompanyName = '<%=this.Model.CompanyName%>';
        if ('<%=this.Model.entity != null%>' == 'True') {
          var  entity = <%=this.Model.entity != null ? this.Model.entity:""%>;
        }

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

                        //if (title == "补充协议" || title == "发票信息") {
                        if (title != "") {  //所有标签都这么做
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

            var isadd = id == "" || CompanyName.indexOf("reg-") != -1 ? true : false;
            if (!isadd) {
                if (entity.ClientStatus ==<%=Needs.Ccs.Services.Enums.ClientStatus.Auditing.GetHashCode()%>)
                {
                    addTab("基本信息", "Edit.aspx?ID=" + id + '&Source=RiskView', "Info", false);
                }
                else {

                      switch (entity.ServiceType) {
                    case <%=Needs.Ccs.Services.Enums.ServiceType.Warehouse.GetHashCode()%>:
                        addTab("基本信息", "Edit.aspx?ID=" + id + '&Source=RiskView', "Info", false);
                        addTab("代仓储协议", "WsAgreement.aspx?ID=" + id, "WsAgreement", false);
                        break;
                    case <%=Needs.Ccs.Services.Enums.ServiceType.Customs.GetHashCode()%>:
                        addTab("基本信息", "Edit.aspx?ID=" + id + '&Source=RiskView', "Info", false);
                        addTab("协议信息", "Control/Edit.aspx?ID=" + id + '&Source=RiskView', "Agreement", false);
                        break;
                    case <%=Needs.Ccs.Services.Enums.ServiceType.Both.GetHashCode()%>:
                        addTab("基本信息", "Edit.aspx?ID=" + id + '&Source=RiskView', "Info", false);
                        addTab("协议信息", "Control/Edit.aspx?ID=" + id + '&Source=RiskView', "Agreement", false);
                        addTab("代仓储协议", "WsAgreement.aspx?ID=" + id, "WsAgreement", false);
                        break;
                }
                }

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
    <script>

        //约定示例
        top.$.backLiebiao = {
            kkk: 1,
            Ruturn: function () {
                //var source = window.parent.frames.Source;//$('#ClientInfo').data('source');

                var url = location.pathname.replace(/Risk.aspx/ig, 'New/AllList.aspx');
                window.location = url;
            }
        };

    </script>

</head>
<body>
    <div>
        <div id="tt" class="easyui-tabs">
        </div>
    </div>
</body>
</html>
