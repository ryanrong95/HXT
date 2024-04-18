<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="DoneList.aspx.cs" Inherits="Yahv.PvData.WebApp.Classify.DoneList" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/classify.ajax.js"></script>
    <script src="../scripts/pvwsorder.js"></script>
    <script>
        var admin = eval('(<%=this.Model.Admin%>)');
        var domainUrls = eval('(<%=this.Model.DomainUrls%>)');

        var getQuery = function () {
            var params = {
                action: 'data',
                orderId: $.trim($('#orderId').textbox("getText")),
                partNumber: $.trim($('#partNumber').textbox("getText")),
                name: $.trim($('#name').textbox("getText")),
                hsCode: $.trim($('#hsCode').textbox("getText")),
                startDate: $.trim($('#startDate').textbox("getText")),
                endDate: $.trim($('#endDate').textbox("getText"))
            };
            return params;
        };

        function btn_formatter(val, row, index) {
            var btns = [];
            btns.push('<span class="easyui-formatted">');
            btns.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="view(\'' + index + '\');return false;">查看</a> ');
            btns.push('</span>');
            return btns.join('');
        }

        $(function () {
            window.grid = $("#dg").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false,
                nowrap: false
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                location.reload();
                return false;
            });

            // 导出Excel
            $('#btnExport').click(function () {
                var params = {
                    orderId: $.trim($('#orderId').textbox("getText")),
                    partNumber: $.trim($('#partNumber').textbox("getText")),
                    name: $.trim($('#name').textbox("getText")),
                    hsCode: $.trim($('#hsCode').textbox("getText")),
                    startDate: $.trim($('#startDate').textbox("getText")),
                    endDate: $.trim($('#endDate').textbox("getText"))
                };

                ajaxLoading();
                $.post('?action=Export', params, function (result) {
                    ajaxLoadEnd();
                    var res = JSON.parse(result);
                    if (res.success) {
                        $.messager.alert('消息', res.message, 'info', function () {
                            if (res.success) {
                                //下载文件
                                try {
                                    let a = document.createElement('a');
                                    a.href = res.fileUrl;
                                    a.download = "";
                                    a.click();
                                } catch (e) {
                                    console.log(e);
                                }
                            }
                        });
                    } else {
                        $.messager.alert('提示', res.message)
                    }
                });

                return false;
            });
        });

        //查看归类信息
        function view(index) {
            ajaxLoading();
            var data = $("#dg").myDatagrid('getRows')[index];
            $.post('?action=GetOrderInfos', { orderId: data.OrderID }, function (infos) {
                ajaxLoadEnd();

                var step = '<%= Yahv.PvWsOrder.Services.Enums.ClassifyStep.Done.GetHashCode()%>';
                var data2 = {};
                for (var k in data) {
                    data2[k] = data[k];
                }

                data2['PIs'] = infos.PIs;
                data2['SpecialType'] = infos.SpecialType;
                data2['CallBackUrl'] = domainUrls.CallBackUrl;
                data2['PvDataApiUrl'] = domainUrls.PvDataApiUrl;
                data2['NextUrl'] = domainUrls.NextUrl;
                data2['Step'] = step;
                data2['CreatorID'] = admin.ID;
                data2['CreatorName'] = admin.RealName;

                doClassify(data2);
            });

        }

        function doClassify(data, otherOptions) {
            $.classifyAjax(data, {
                onClose: function () {
                    grid.myDatagrid('reload', getQuery());
                }
            }, otherOptions);
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">型号</td>
                <td>
                    <input id="partNumber" data-options="prompt:'型号',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">品名</td>
                <td>
                    <input id="name" data-options="prompt:'品名',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">海关编码</td>
                <td>
                    <input id="hsCode" data-options="prompt:'海关编码',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">订单编号</td>
                <td>
                    <input id="orderId" data-options="prompt:'订单编号',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">归类时间</td>
                <td>
                    <input id="startDate" data-options="prompt:'开始时间',validType:'length[1,50]',isKeydown:true" class="easyui-datebox" />
                </td>
                <td style="width: 90px;">至</td>
                <td>
                    <input id="endDate" data-options="prompt:'截止时间',validType:'length[1,50]',isKeydown:true" class="easyui-datebox" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnExport" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'">导出Excel</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="dg" title="已归类产品">
        <thead>
            <tr>
                <th data-options="field:'HSCode',align:'center',width:100">HS编码</th>
                <th data-options="field:'TariffName',align:'left',width:180">报关品名</th>
                <th data-options="field:'Elements',align:'left',width:500">申报要素</th>
                <th data-options="field:'PartNumber',align:'center',width:150">产品型号</th>
                <th data-options="field:'Manufacturer',align:'center',width:80">品牌</th>
                <th data-options="field:'Origin',align:'center',width:80">原产地</th>
                <th data-options="field:'Quantity',align:'center',width:80">申报数量</th>
                <th data-options="field:'Unit',align:'center',width:80">申报单位</th>
                <th data-options="field:'LegalUnit1',align:'center',width:80">法定第一单位</th>
                <th data-options="field:'UnitPrice',align:'center',width:80">单价</th>
                <th data-options="field:'Currency',align:'center',width:80">币种</th>
                <th data-options="field:'ImportPreferentialTaxRate',align:'center',width:80">优惠税率%</th>
                <th data-options="field:'OriginATRate',align:'center',width:80">加征税率%</th>
                <th data-options="field:'VATRate',align:'center',width:80">增值税率%</th>
                <th data-options="field:'ExciseTaxRate',align:'center',width:80">消费税率%</th>
                <th data-options="field:'CIQCode',align:'center',width:100">检验检疫编码</th>
                <th data-options="field:'LockStatus',align:'center',width:80">锁定状态</th>
                <th data-options="field:'Locker',align:'center',width:100">锁定人</th>
                <th data-options="field:'LockTime',align:'center',width:100">锁定时间</th>
                <th data-options="field:'CreateDate',align:'center',width:150">创建时间</th>
                <th data-options="field:'CompleteDate',align:'center',width:150">归类完成时间</th>
                <th data-options="field:'ClassifyFirstOperatorName',align:'center',width:100">预处理一人员</th>
                <th data-options="field:'ClassifySecondOperatorName',align:'center',width:100">预处理二人员</th>
                <th data-options="field:'OrderStatus',align:'center',width:100">订单状态</th>
            </tr>
        </thead>
        <thead data-options="frozen:true">
            <tr>
                <th data-options="field:'Btn',align:'left',width:150,formatter:btn_formatter">操作</th>
                <th data-options="field:'OrderID',align:'left'" style="width: 150px;">订单编号</th>
                <th data-options="field:'ClientCode',align:'left'" style="width: 70px;">客户入仓号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 200px;">客户名称</th>
            </tr>
        </thead>
    </table>
</asp:Content>
