<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" CodeBehind="BankRiskArea.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Banks.BankRiskArea" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                singleSelect: false,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery()
            });

            $('#Districts').combobox({
                data: model.Districts,
                valueField: "value",
                textField: "text",
                multiple: false,
                filter: function(q, row){
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
            };
            return params;
        };
        
        function btnFormatter(value, row) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="deleteBankRiskArea(\'' + row.BankRiskAreaID + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
        
        function deleteBankRiskArea(BankRiskAreaID) {
            ajaxLoading();
            $.post('?action=deleteBankRiskArea', {
                BankRiskAreaID: BankRiskAreaID,
            }, function (data) {
                ajaxLoadEnd();
                var dataJson = JSON.parse(data);
                if(dataJson.success == true) {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: dataJson.message,
                        type: "success"
                    });
                } else {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: dataJson.message,
                        type: "error"
                    });
                }

                $('#tab1').myDatagrid('search', getQuery());
            });
        }
        
        //添加
        function Add() {
            var Districts = $.trim($('#Districts').combobox('getValue'));
            
            var formatok = true;

            if(Districts == "") {
                top.$.timeouts.alert({ position: "TC", msg: "请正确选择地区", type: "error" });
                formatok = false;
            }

            if(formatok == false) {
                return;
            }
            
            ajaxLoading();
            $.post('?action=Add', {
                BankID: model.BankID,
                Districts: Districts,
            }, function (data) {
                ajaxLoadEnd();
                var dataJson = JSON.parse(data);
                if(dataJson.success == true) {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: dataJson.message,
                        type: "success"
                    });
                } else {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: dataJson.message,
                        type: "error"
                    });
                }

                $('#tab1').myDatagrid('search', getQuery());
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">
                    <input id="Districts" name="Districts" class="easyui-combobox" data-options="editable:true,required:false," style="width: 180px;"/>
                    <a href="javascript:;" class="l-btn l-btn-small" style="height: 22px; margin-left: 5px;" onclick="Add()">
		                <span class="l-btn-left l-btn-icon-left" style="margin-top: -4px;">
			                <span class="l-btn-text">添加</span>
			                <span class="l-btn-icon icon-yg-confirm">&nbsp;</span>
		                </span>
	                </a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1">
        <thead>
            <tr>
                <th data-options="field:'DistrictName',align:'center',width:fixWidth(50)">地区名称</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(35)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
