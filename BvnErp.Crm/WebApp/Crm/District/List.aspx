<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.District.List" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>区域管理</title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">

        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM系统管理';
        gvSettings.menu = '区域管理';
        gvSettings.summary = '';

        var Districts = eval(<%=this.Model.TreeData%>);
        var SaleM = eval(<%=this.Model.SalesM%>);
        var Sales = eval(<%=this.Model.Sales%>);
        var MarketM = eval(<%=this.Model.MarketM%>);
        var MarketA = eval(<%=this.Model.MarketA%>);

        //页面初始化
        $(function () {

            $('#SaleManager').tree({
                data: SaleM,
            });
            $('#Sales').tree({
                data: Sales,
            });
            $('#MarketManager').tree({
                data: MarketM,
            });
            $('#MarketSales').tree({
                data: MarketA,
            });

            //区域树的初始化
            $('#districts').tree({
                data: Districts,
                onDblClick: function (node) {
                    var districtid = $(this).tree('getSelected');
                    $("#hidDistrict").val(districtid.id);
                    $("#btnLoad").click();
                },
            });
        });

        //区域维护
        function District() {
            var url = location.pathname.replace(/List.aspx/ig, 'DistrictEdit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '区域列表',
                url: url,
                onClose: function () {
                    window.location.reload();
                },
            }).open();
            return false;
        }

        //人员任命
        function Edit(Type) {
            var rowdata = $('#districts').tree('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.id + "&Type=" + Type;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '人员任命',
                    url: url,
                    onClose: function () {
                        window.location.reload();
                    },
                }).open();
            }
            else {
                $.messager.alert('提示', '请选择人员任命区域！');
            }
            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <input type="hidden" id="hidDistrict" runat="server" />
        <table style="width: 100%;">
            <tr>
                <td style="vertical-align: top; width: 40%;">
                    <div id="divDistrict">
                        <div class="easyui-panel" style="padding: 5px; height: 800px" title="区域管理">
                            <ul id="districts" class="easyui-tree" data-options="method:'get'"></ul>
                            <div>
                                <%--<asp:Button runat="server" ID="btnDistrict" Text="区域新增" OnClientClick="return District()" />--%>
                            </div>
                        </div>
                    </div>
                </td>
                <td style="vertical-align: top; width: 30%;">
                    <div id="divSale">
                        <div class="easyui-panel" title="销售人员结构" style="height: 800px">
                            <div class="easyui-panel" title="经理">
                                <ul id="SaleManager" class="easyui-tree" data-options="method:'get'"></ul>
                            </div>
                            <div class="easyui-panel" title="员工">
                                <ul id="Sales" class="easyui-tree" data-options="method:'get'"></ul>
                            </div>
                            <div style="margin-bottom: 10px">
                                <asp:Button runat="server" ID="btnEditSave" Text="销售人员任命" OnClientClick="return Edit(10)" />
                            </div>
                        </div>
                    </div>
                </td>
                <td style="vertical-align: top; width: 30%">
                    <div id="divMarket">
                        <div class="easyui-panel" title="市场人员结构" style="height: 800px">
                            <div class="easyui-panel" title="经理">
                                <ul id="MarketManager" class="easyui-tree" data-options="method:'get'"></ul>
                            </div>
                            <div class="easyui-panel" title="员工">
                                <ul id="MarketSales" class="easyui-tree" data-options="method:'get'"></ul>
                            </div>
                            <div style="margin-bottom: 10px">
                                <asp:Button runat="server" ID="btnEditMarket" Text="市场人员任命" OnClientClick="return Edit(20)" />
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
        <div style="display: none">
            <asp:Button runat="server" ID="btnLoad" Text="加载数据" OnClick="btnLoad_Click" />
        </div>
    </form>
</body>
</html>
