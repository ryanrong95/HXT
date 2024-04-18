<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.InfoSearch.Embargo.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
        <script>
        var getQuery = function () {
            var params = {
                partNumber: $('#partNumber').textbox('getText'),
            };
            return params;
        }

        var eccnFormatter = function (value, row, index) {
            var text = ''
            var eccns = row.Eccns;
            if (eccns.length > 0) {
                for(var i = 0; i < eccns.length; i++) {
                    text += (eccns[i].Manufacturer != null ? '品牌: ' + eccns[i].Manufacturer : '') + ' 编码: ' + eccns[i].Code + '<br>';
                }
            }
            return text;
        }

        $(function () {
            window.grid = $('#dg').myDatagrid({
                rownumbers: true,
                pagination: true,
                nowrap: false,
                queryParams: getQuery(),
                toolbar: '#topper',
                loadEmpty: true
            });

            $('#btnSearch').click(function () {
                var partNumber = $('#partNumber').textbox('getText');
                if (partNumber.trim() == '') {
                    $.messager.alert('提示', '请输入产品型号!');
                } else {
                    window.grid.myDatagrid('search', getQuery());
                }
                return false;
            });

            $('#btnClear').click(function () {
                location.replace(location.href);
                return false;
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao-compact">
            <tbody>
                <tr>
                    <td style="width: 100px">产品型号:</td>
                    <td>
                        <input id="partNumber" class="easyui-textbox" data-options="validType:'length[1,100]'" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'PartNumber',align:'left',width:150">产品型号</th>
                <th data-options="field:'Manufacturer',align:'center',width:150">品牌</th>
                <th data-options="field:'Embargo',align:'center',width:100">禁运</th>
                <th data-options="field:'Eccns',align:'left',formatter:eccnFormatter,width:250">Eccn</th>
            </tr>
        </thead>
    </table>
</asp:Content>
