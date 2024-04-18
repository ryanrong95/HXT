<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Packing.aspx.cs" Inherits="WebApp.HKWarehouse.Entry.Packing" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>库房分拣</title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script type="text/javascript">
        var id = '<%=this.Model.ID%>';
        var OrderID = '<%=this.Model.OrderID%>';
        var EntryNumber = '<%=this.Model.EntryNumber%>';
        var EntryStatus = getQueryString('EntryStatus');
        $(function () {
            $('#tt').tabs({
                border: false,
                tabWidth: 120,
            });

            var isadd = id == "" ? true : false;
            addTab("分拣", "Sorting.aspx?ID=" + id + "&&OrderID=" + OrderID + "&&EntryNumber=" + EntryNumber+"&&EntryStatus="+EntryStatus, "Info", false);
            addTab("添加费用", "../Fee/OrderFeeList.aspx?OrderID=" + OrderID, "Fee", isadd);
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
                    content = '<iframe id="' + id + '" scrolling="no" frameborder="0"   src="' + href + '"style="width:100%;min-height:900px;height:auto"></iframe>';
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
