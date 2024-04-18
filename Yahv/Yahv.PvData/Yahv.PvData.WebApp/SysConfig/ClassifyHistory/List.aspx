<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.ClassifyHistory.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/classifyHistory.ajax.js"></script>
    <script>
        var setWindow = 'SecondList_' + Math.floor(Math.random() * 10000);
        $.myWindow.setMyWindow(setWindow, window);
        var admin = eval('(<%=this.Model.Admin%>)');
        var domainUrls = eval('(<%=this.Model.DomainUrls%>)');

        var getQuery = function () {
            var params = {
                partNumber: $('#partNumber').textbox('getText'),
                manufacturer: $('#manufacturer').textbox('getText'),
                hsCode: $('#hsCode').textbox('getText'),
                name: $('#name').textbox('getText'),
            };
            return params;
        }

        function btn_formatter(value, row, index) {
            var buttons = [];
            var btn = '<span class="easyui-formatted" style="display:inline-block;">'
                + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + index + '\')">编辑</a>'
                + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="remark(\'' + row.ID + '\')">备注信息</a>'
                + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-quote\'" onclick="quote(\'' + row.PartNumber + '\')">历史报价</a>'
                + '</span>';

            buttons.push(btn);
            return buttons.join('');
        }

        // 编辑归类历史数据
        function edit(index) {
            var data = $("#dg").myDatagrid('getRows')[index];
            var data2 = {};
            for (var k in data) {
                data2[k] = data[k];
            }

            data2['PvDataApiUrl'] = domainUrls.PvDataApiUrl;
            data2['CreatorID'] = admin.ID;
            data2['CreatorName'] = admin.RealName;
            data2['SetWindow'] = setWindow;//将本页面的window传递给编辑页面
            doClassify(data2);

            return false;
        }

        function doClassify(data) {
            $.classifyAjax(data, {
                onClose: function () {
                    grid.myDatagrid('reload', getQuery());
                }
            });
        }

        // 备注信息
        function remark(id) {
            $.myDialog({
                title: '备注信息',
                url: 'Remark.aspx?id=' + id,
                width: '500px',
                height: '300px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });

            return false;
        }

        function quote(partNumber) {
            $.myWindow({
                title: '历史报价',
                url: 'Quote.aspx?partNumber=' + partNumber,
                width: '500px',
                height: '300px',
            });
        }

        $(function () {
            window.grid = $('#dg').myDatagrid({
                rownumbers: true,
                pagination: true,
                nowrap: false,
                queryParams: getQuery(),
                toolbar: '#topper'
            });

            $('#btnSearch').click(function () {
                window.grid.myDatagrid('search', getQuery());
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
                    <td style="width: 90px">产品型号:</td>
                    <td>
                        <input id="partNumber" class="easyui-textbox" />
                    </td>

                    <td style="width: 90px">品牌:</td>
                    <td>
                        <input id="manufacturer" class="easyui-textbox" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 90px">海关编码:</td>
                    <td>
                        <input id="hsCode" class="easyui-textbox" />
                    </td>

                    <td style="width: 90px">报关品名:</td>
                    <td>
                        <input id="name" class="easyui-textbox" />
                    </td>
                </tr>
                <tr>
                    <td colspan="8">
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
                <th data-options="field:'Elements',align:'left',width:500">申报要素</th>
                <th data-options="field:'TaxCode',align:'center',width:150">税务编码</th>
                <th data-options="field:'TaxName',align:'center',width:150">税务名称</th>
                <th data-options="field:'ImportPreferentialTaxRate',align:'center',width:80">优惠税率</th>
                <th data-options="field:'VATRate',align:'center',width:80">增值税率</th>
                <th data-options="field:'ExciseTaxRate',align:'center',width:80">消费税率</th>
                <th data-options="field:'LegalUnit1',align:'center',width:80">法定第一单位</th>
                <th data-options="field:'LegalUnit2',align:'center',width:80">法定第二单位</th>
                <th data-options="field:'CIQCode',align:'center',width:100">检验检疫编码</th>
                <th data-options="field:'SpecialTypes',align:'center',width:150">特殊类型</th>
                <th data-options="field:'Summary',align:'center',width:150">备注信息</th>
            </tr>
        </thead>
        <thead data-options="frozen:true">
            <tr>
                <th data-options="field:'Btn',align:'left',width:250,formatter:btn_formatter">操作</th>
                <th data-options="field:'PartNumber',align:'left',width:150">产品型号</th>
                <th data-options="field:'Manufacturer',align:'center',width:150">品牌</th>
                <th data-options="field:'HSCode',align:'center',width:100">HS编码</th>
                <th data-options="field:'TariffName',align:'left',width:100">报关品名</th>
            </tr>
        </thead>
    </table>
</asp:Content>
