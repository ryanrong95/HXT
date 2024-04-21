<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" CodeBehind="PastsHistory.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Staffs.PastsHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .l-btn-icon-left {
            margin-top: -4px !important;
        }
    </style>
    <script type="text/javascript">
        //页面加载
        $(function () {
            $("#tab1").myDatagrid({
                pagination: false,
                singleSelect: false,
                fitColumns: true,
                nowrap: false
            });
        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="text-align: left; padding: 5px">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="Close()">关闭</a>
    </div>
    <table id="tab1" title="工资项历史记录">
        <thead>
            <tr>
                <th data-options="field:'WageItem',width:300">工资项默认值</th>
                <th data-options="field:'AdminName',width:40">录入人</th>
                <th data-options="field:'CreateDate',width:40">录入日期</th>
            </tr>
        </thead>
    </table>
</asp:Content>

