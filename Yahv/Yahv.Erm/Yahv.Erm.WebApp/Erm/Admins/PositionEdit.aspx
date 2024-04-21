<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" CodeBehind="PositionEdit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Admins.PositionEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //区域树的初始化
            $('#leagues').tree({
                data: model.treedata,
                checkbox: true,
                onlyLeafCheck: true,
                onLoadSuccess: function (node, data) {
                    var fathernode = $(this).tree("find", data[0].id);
                    var nodes = $(this).tree("getChildren", fathernode.target);
                    for (var i = 0; i < nodes.length; i++) {
                        //判断是否为叶节点
                        if ($(this).tree("isLeaf", nodes[i].target)) {
                            if (model.leagueids.indexOf(nodes[i].id) >= 0) {
                                $(this).tree("check", nodes[i].target);
                            }
                        }
                    }
                },
            });
        });


        //保存
        function Save() {
            var array = $("#leagues").tree("getChecked");
            var ids = array.map(function (element, index) {
                debugger;
                return element.id;
            }).join(',');
            $("#hidids").val(ids);
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <ul id="leagues" class="easyui-tree" data-options="method:'get'"></ul>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="return Save();" OnClick="btnSubmit_Click" />
        </div>
    </div>
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />
    <input type="hidden" runat="server" id="hidids" />
</asp:Content>
