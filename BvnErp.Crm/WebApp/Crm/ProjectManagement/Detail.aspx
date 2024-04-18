<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Crm.ProjectManagement.Detail" %>

<%@ Import Namespace="Needs.Utils.Descriptions" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>销售机会产品详情</title>
    <uc:EasyUI runat="server" />
    <style>
        .normal_a {
            text-decoration: underline;
            color: blue;
        }
    </style>
</head>
<body>
    <div data-options="region:'north',border:false">
        <form id="form1" runat="server" method="post">
            <%
                var product = this.Model.Product as NtErp.Crm.Services.Models.Projects.ProjectProductItem;
            %>
            <table id="table1" style="width: 100%">
                <tr>
                    <th style="width: 11%"></th>
                    <th style="width: 22%"></th>
                    <th style="width: 11%"></th>
                    <th style="width: 22%"></th>
                    <th style="width: 11%"></th>
                    <th style="width: 22%"></th>
                </tr>
                <tr>
                    <td style="height: 30px; font-size: 20px;">项目信息</td>
                </tr>
                <tr>
                    <td class="lbl">客户</td>
                    <td>
                        <input type="hidden" id="id" value="<%=product.ProductItem.ID %>" />
                        <input class="easyui-textbox" id="clientName" name="clientName" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">合作公司</td>
                    <td>
                        <input class="easyui-textbox" id="companyName" name="companyName" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">币种</td>
                    <td>
                        <input class="easyui-textbox" id="currencyName" name="currencyName" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">项目名称</td>
                    <td>
                        <input class="easyui-textbox" id="name" name="name" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">产品名称</td>
                    <td>
                        <input class="easyui-textbox" id="productName" name="productName" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">应用行业</td>
                    <td>
                        <input class="easyui-textbox" id="industryName" name="industryName" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px"></td>
                </tr>
                <tr>
                    <td style="height: 30px; font-size: 20px;">用料信息</td>
                </tr>
                <tr>
                    <td class="lbl">产品型号</td>
                    <td>
                        <input class="easyui-textbox" id="itemName" name="itemName" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">型号全称</td>
                    <td>
                        <input class="easyui-textbox" id="itemOrigin" name="itemOrigin" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">品牌</td>
                    <td>
                        <input class="easyui-textbox" id="itemMF" name="itemMF" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">单机用量</td>
                    <td>
                        <input class="easyui-numberbox" id="refUnitQuantity" name="refUnitQuantity" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">项目用量</td>
                    <td>
                        <input class="easyui-numberbox" id="refQuantity" name="refQuantity" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">参考单价(CNY)</td>
                    <td>
                        <input class="easyui-numberbox" id="refUnitPrice" name="refUnitPrice" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">状态</td>
                    <td>
                        <input class="easyui-textbox" id="status" name="status" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">预计成交概率(%)</td>
                    <td>
                        <input class="easyui-numberbox" id="expectRate" name="expectRate" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>

                </tr>
                <tr>
                    <td class="lbl">预计成交量</td>
                    <td>
                        <input class="easyui-numberbox" id="expectQuantity" name="expectQuantity" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">预计成交总额（CNY）</td>
                    <td>
                        <span><%=product.ProductItem.ExpectTotal %></span>
                    </td>
                    <td class="lbl">预计成交日期</td>
                    <td>
                        <input class="easyui-textbox" id="expectDate" name="expectDate" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>

                </tr>
                <tr>
                    <td class="lbl">竞品型号</td>
                    <td>
                        <input class="easyui-textbox" id="competeModel" name="competeModel" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">竞品厂商</td>
                    <td>
                        <input class="easyui-textbox" id="competeManu" name="competeManu" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">竞品单价</td>
                    <td>
                        <input class="easyui-numberbox" id="competePrice" name="competePrice" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">型号备注</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="summary" name="summary" data-options="multiline:true,validType:'length[0,200]',readonly:true,disabled:true" style="width: 99%; height: 60px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">凭证</td>
                    <td>
                        <a target="_blank" class="normal_a" href="<%=product.ProductItem.Voucher?.Url %>"><%=product.ProductItem.Voucher?.Name %></a>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px; font-size: 20px;">人员信息</td>
                </tr>
                <tr>
                    <td class="lbl">销售</td>
                    <td>
                        <input class="easyui-textbox" id="saleAdmin" name="saleAdmin" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">销售助理</td>
                    <td>
                        <input class="easyui-textbox" id="assistantAdmin" name="assistantAdmin" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">采购助理</td>
                    <td>
                        <input class="easyui-textbox" id="purchaseAdmin" name="purchaseAdmin" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">PM</td>
                    <td>
                        <input class="easyui-textbox" id="pmAdmin" name="pmAdmin" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                    <td class="lbl">FAE</td>
                    <td>
                        <input class="easyui-textbox" id="faeAdmin" name="faeAdmin" data-options="readonly:true,disabled:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px; font-size: 20px;">送样信息</td>
                </tr>
                <tr>
                    <td class="lbl">是否送样</td>
                    <td>
                        <input type="radio" id="sampleOn" name="IsSample" value="true" />是
                        <input type="radio" id="sampleOff" name="IsSample" value="false" />否
                    </td>
                </tr>
                <tr>
                    <td class="lbl">送样类型</td>
                    <td>
                        <input class="easyui-combobox" id="sampleType" name="sampleType" style="width: 95%" />
                    </td>
                    <td class="lbl">送样数量</td>
                    <td>
                        <input class="easyui-numberbox" id="sampleQuantity" name="sampleQuantity" data-options="min:0,validType:'length[1,10]',required:true" style="width: 95%" />
                    </td>
                    <td class="lbl">送样单价</td>
                    <td>
                        <input class="easyui-numberbox" id="samplePrice" name="samplePrice" data-options="min:0,precision:5,validType:'length[1,15]',required:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">送样时间</td>
                    <td>
                        <input class="easyui-datebox" id="sampleDate" name="sampleDate" data-options="required:true" style="width: 95%" />
                    </td>
                    <td class="lbl">送样联系人</td>
                    <td>
                        <input class="easyui-textbox" id="sampleContactor" name="sampleContactor" data-options="validType:'length[1,50]',required:true" style="width: 95%" />
                    </td>
                    <td class="lbl">送样联系电话</td>
                    <td>
                        <input class="easyui-textbox" id="samplePhone" name="samplePhone" data-options="" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">送样地址</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="sampleAddress" name="sampleAddress" data-options="validType:'length[1,200]'" style="width: 99%" />
                    </td>
                </tr>

            </table>
            <div id="divSave" style="text-align: center; margin-top: 20px; margin-bottom: 10px">
                <asp:Button ID="btnSave" Text="保存" runat="server" OnClientClick="return save();" OnClick="btnSave_Click" />
            </div>
        </form>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="询价列表" data-options="fitColumns:false,border:false,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th data-options="field:'opt',align:'center',width:100,formatter:btn_formatter">操作</th>
                    <th data-options="field:'Voucher',align:'center',width:100,formatter:voucher_formatter">原厂批复凭证</th>
                    <th data-options="field:'ReplyDate',align:'center',width:80">批复时间</th>
                    <th data-options="field:'RFQ',align:'center',width:100">原厂RFQ</th>
                    <th data-options="field:'OriginModel',align:'center',width:100">原厂型号</th>
                    <th data-options="field:'MOQ',align:'center',width:100">最小起订量（MOQ）</th>
                    <th data-options="field:'MPQ',align:'center',width:100">最小包装量（MPQ）</th>
                    <th data-options="field:'ReplyPrice',align:'center',width:100">批复单价</th>
                    <th data-options="field:'CurrencyStr',align:'center',width:60">币种</th>
                    <th data-options="field:'ExchangeRate',align:'center',width:60">汇率</th>
                    <th data-options="field:'TaxRate',align:'center',width:60">税率(%)</th>
                    <th data-options="field:'Tariff',align:'center',width:60">关税点(%)</th>
                    <th data-options="field:'OtherRate',align:'center',width:100">其他附加点(%)</th>
                    <th data-options="field:'Cost',align:'center',width:100">含税人民币成本价</th>
                    <th data-options="field:'Validity',align:'center',width:80">有效时间</th>
                    <th data-options="field:'ValidityCount',align:'center',width:60">有效数量</th>
                    <th data-options="field:'SalePrice',align:'center',width:60">参考售价</th>
                    <th data-options="field:'Summary',align:'center',width:200">特殊备注</th>
                </tr>
            </thead>
        </table>
        <div id="tb" style="height: auto">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="add()">新增询价</a>
        </div>
    </div>
    <div style="width: 1px; height: 30px;"></div>
    <script>
        <%
        var product = this.Model.Product as NtErp.Crm.Services.Models.Projects.ProjectProductItem;
        %>
        var sampleTypeData = eval('<%=this.Model.SampleTypeData%>');

        var IsSample = eval('<%=product.ProductItem.IsSample%>'.toLowerCase());
        
        function voucher_formatter(value, row, index) {
            return '<a class=\"normal_a\" target=\"_blank\" href=\"' + row.VoucherUrl + '\">' + value + '</a>';
        }

        function btn_formatter(value, row, index) {
            var buttons = "";
            buttons += '<button class="btn_detail" onclick="edit(\'' + row.ID + '\')">编辑</button>';
            return buttons;
        }

        function edit(id) {
            top.$.myWindow({
                iconCls: "",
                url: location.pathname.replace(/Detail.aspx/ig, 'EditEnquiry.aspx') + "?productitemID=<%=product.ProductItem.ID%>&id=" + id,
                noheader: false,
                title: '编辑询价',
                width: '80%',
                height: '80%',
                onClose: function () {
                    $('#datagrid').bvgrid('flush');
                }
            }).open();
        }

        function add() {
            top.$.myWindow({
                iconCls: "",
                url: location.pathname.replace(/Detail.aspx/ig, 'EditEnquiry.aspx') + "?productitemID=<%=product.ProductItem.ID%>",
                noheader: false,
                title: '新增询价',
                width: '80%',
                height: '80%',
                onClose: function () {
                    $('#datagrid').bvgrid('flush');
                }
            }).open();
        }

        function save() {
            //校验必填项
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }

            //文件大小检验
            if ($("#ReportUpload")[0].files.length > 0 && $("#ReportUpload")[0].files[0].size > 4096 * 1024) {
                alert("上传的文件凭证超过4M!请重新选择文件上传!");
                return false;
            }
        }


        $(function () {
            $('#sampleType').combobox({
                data: sampleTypeData,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                editable: false,
                required: true,
                onChange: function (newValue, oldValue) {

                }
            });                        
            
            if (IsSample) {
                $('#sampleOn').prop('checked', true);
                $('#sampleOff').prop('checked', false);

                $('#sampleType').combobox({ required: true });
                $('#sampleQuantity').numberbox({ required: true });
                $('#samplePrice').numberbox({ required: true });
                $('#sampleDate').datebox({ required: true });
                $('#sampleContactor').textbox({ required: true });
                $('#samplePhone').textbox({ required: true });
            }
            else {
                $('#sampleOff').prop('checked', true);
                $('#sampleOn').prop('checked', false);

                $('#sampleType').combobox({ required: false });
                $('#sampleQuantity').numberbox({ required: false });
                $('#samplePrice').numberbox({ required: false });
                $('#sampleDate').datebox({ required: false });
                $('#sampleContactor').textbox({ required: false });
                $('#samplePhone').textbox({ required: false });
            }

            $('input[name="IsSample"]').change(function () {
                var checkedvalue = $('input[name="IsSample"]:checked').val();

                if (checkedvalue == 'true') {
                    $('#sampleType').combobox({ required: true, editable: false, readonly: false });
                    $('#sampleQuantity').numberbox({ required: true, readonly: false });
                    $('#samplePrice').numberbox({ required: true, readonly: false });
                    $('#sampleDate').datebox({ required: true, editable: false, readonly: false });
                    $('#sampleContactor').textbox({ required: true, readonly: false });
                    $('#samplePhone').textbox({ required: true, readonly: false });
                    $('#sampleAddress').textbox({ requied: true, readonly: false });
                }
                else {
                    $('#sampleType').combobox({ required: false, readonly: true }).combobox('clear');
                    $('#sampleQuantity').numberbox({ required: false, readonly: true }).numberbox('clear');
                    $('#samplePrice').numberbox({ required: false, readonly: true }).numberbox('clear');
                    $('#sampleDate').datebox({ required: false, editable: false, readonly: true }).datebox('clear');
                    $('#sampleContactor').textbox({ required: false, readonly: true }).textbox('clear');
                    $('#samplePhone').textbox({ required: false, readonly: true }).textbox('clear');
                    $('#sampleAddress').textbox({ requied: false, readonly: true }).textbox('clear');
                }
            })

            //表单赋值
            $('#form1').form('load', {
                clientName: '<%=product.Project.Client.Name%>',
                companyName: '<%=product.Project.Company.Name%>',
                currencyName: '<%=product.Project.Currency.GetDescription()%>',
                name: '<%=product.Project.Name%>',
                productName: '<%=product.Project.ProductName%>',
                industryName: '<%=product.Project.Industry.Name%>',
                itemName: '<%=product.ProductItem.StandardProduct.Name%>',
                itemOrigin: '<%=product.ProductItem.StandardProduct.Origin%>',
                itemMF: '<%=product.ProductItem.StandardProduct.Manufacturer.Name%>',
                refUnitQuantity: '<%=product.ProductItem.RefUnitQuantity%>',
                refQuantity: '<%=product.ProductItem.RefQuantity%>',
                refUnitPrice: '<%=product.ProductItem.RefUnitPrice%>',
                status: '<%=product.ProductItem.Status.GetDescription()%>',
                expectRate: '<%=product.ProductItem.ExpectRate%>',
                expectQuantity: '<%=product.ProductItem.ExpectQuantity%>',
                expectDate: '<%=product.ProductItem.ExpectDate?.ToString("yyyy-MM-dd")%>',
                competeModel: '<%=product.ProductItem.CompeteProduct?.Name%>',
                competeManu: '<%=product.ProductItem.CompeteProduct?.ManufacturerID%>',
                saleAdmin: '<%=product.ProductItem.SaleAdmin?.RealName%>',
                assistantAdmin: '<%=product.ProductItem.AssistantAdmin?.RealName%>',
                purchaseAdmin: '<%=product.ProductItem.PurChaseAdmin?.RealName%>',
                pmAdmin: '<%=product.ProductItem.PMAdmin?.RealName%>',
                faeAdmin: '<%=product.ProductItem.FAEAdmin?.RealName%>',                
                sampleQuantity: '<%=product.ProductItem.Sample?.Quantity%>',
                samplePrice: '<%=product.ProductItem.Sample?.UnitPrice%>',
                sampleDate: '<%=product.ProductItem.Sample?.Date%>',
                sampleContactor: '<%=product.ProductItem.Sample?.Contactor%>',
                samplePhone: '<%=product.ProductItem.Sample?.Phone%>',
                sampleAddress: '<%=product.ProductItem.Sample?.Address%>',
                summary: '<%=product.ProductItem.Summary%>',                
            });

            $('#sampleType').combobox('setValue', '<%=(int?)product.ProductItem.Sample?.Type%>');

            $('#datagrid').bvgrid({
                pageSize: 20,
                toolbar: '#tb'
            });


        })
    </script>
</body>
</html>
