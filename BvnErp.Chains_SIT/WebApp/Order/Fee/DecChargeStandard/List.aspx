<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Order.Fee.DecChargeStandard.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报关收费标准</title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/handlebars.min.js"></script>
    <script src="../../../Scripts/Customize/charge-standard-1.js?a=<%=this.Model.timestamp%>"></script>
    <script type="text/javascript">
        $(function () {
            $("#charge-std").chargeStd({
                itemInputWidth: 80,
            });

            $.post('?action=DecChargeStandardList', {}, function (res) {
                var result = JSON.parse(res);
                var stds = result.stds;

                var templateStr = $("#oneline-template").html();
                var template = Handlebars.compile(templateStr);

                for (var i = 0; i < stds.length; i++) {
                    if (stds[i].Level == 1) {
                        var kongHangHtml = template({
                            KongHang: 'konghang',
                        });
                        $("#std-table").append(kongHangHtml);
                    }

                    var stdHtml = template({
                        KongHang: '',
                        ID: stds[i].ID,
                        FatherID: stds[i].FatherID,
                        EnumValue: stds[i].EnumValue,
                        SerialNo: stds[i].SerialNo,
                        Type: stds[i].Type,
                        IsMenuLeaf: stds[i].IsMenuLeaf,
                        Level: stds[i].Level,
                        Name: stds[i].Name,
                        Unit1: stds[i].Unit1,
                        FixedCount1: stds[i].FixedCount1,
                        Unit2: stds[i].Unit2,
                        FixedCount2: stds[i].FixedCount2,
                        Price: stds[i].Price,
                        Currency: stds[i].Currency,
                        CurrencyCN: stds[i].CurrencyCN,
                        Remark1: stds[i].Remark1,
                        Remark2: stds[i].Remark2,
                    });
                    $("#std-table").append(stdHtml);
                }
            });
        });
    </script>
    <style>
        table, td, th {
            border: 1px solid black;
            font-size: 10px;
        }

        table {
            border-collapse: collapse;
            width: 100%;
        }

        th {
            text-align: left;
        }

        tr:hover:not(.tr-level-konghang) {
            background-color: #dedede;
        }

        .level-1 {
            padding-left: 0;
        }

        .level-2 {
            padding-left: 15px;
        }

        .level-3 {
            padding-left: 30px;
        }

        .level-4 {
            padding-left: 45px;
        }

        .level-5 {
            padding-left: 60px;
        }

        .tr-level-konghang {
            line-height: 20px;
            height: 20px;
            background: #888888;
        }

        .tr-leaf-true {
            background: #bccc94;
        }

        #charge-std {
            padding: 5px;
            font-size: 10px;
        }

        .charge-std-calc {
            padding: 2px;
        }

        .charge-std-menu .menu-text span {
            font-size: 8px;
        }

        .charge-std-menu .menu-text {
            font-size: 8px;
        }

        .charge-std-calc-item {
            display: flex;
            margin: 2px 0;
        }

        .charge-std-calc-item label {
            margin: 0 8px 0 2px;
        }
    </style>
</head>
<body>
    <div id="charge-std">
        <div>
            <label>请选择收费项目：</label>
            <input class="charge-std-input" type="text" data-options="readonly: true," style="width: 150px;" />
            <a class="charge-std-btn" data-options="iconCls: 'icon-edit',">点击看到菜单</a>
        </div>
        <div style="margin-top: 5px;">
            <label>备注：</label>
            <input class="charge-std-remark" data-options="multiline:true,readonly: true,"
                style="width: 319px; height: 100px" />
        </div>
        <div class="charge-std-calc" style="margin-top: 5px;"></div>
        <div>
            <label style="color: red;">参考总价：</label>
            <span class="charge-std-result"></span>
        </div>
    </div>
    <div style="margin-top: 20px;">
        <table id="oneline-template" style="display: none;">
            <tbody>
                <tr class="tr-level-{{KongHang}} tr-leaf-{{IsMenuLeaf}}">
                    <td>{{ID}}</td>
                    <td>{{FatherID}}</td>
                    <td>{{EnumValue}}</td>
                    <td>{{SerialNo}}</td>
                    <td>{{Type}}</td>
                    <td>{{IsMenuLeaf}}</td>
                    <td>{{Level}}</td>
                    <td class="level-{{Level}}">{{Name}}</td>
                    <td>{{Unit1}}</td>
                    <td>{{FixedCount1}}</td>
                    <td>{{Unit2}}</td>
                    <td>{{FixedCount2}}</td>
                    <td>{{Price}}</td>
                    <td>{{Currency}} {{CurrencyCN}}</td>
                    <td>{{Remark1}}</td>
                    <td>{{Remark2}}</td>
                </tr>
            </tbody>
        </table>
    </div>
    <div>
        <div>
            <table id="std-table">
                <tbody>
                    <tr>
                        <th style="width: 230px;">ID</th>
                        <th style="width: 230px;">FatherID</th>
                        <th style="width: 80px;">EnumValue</th>
                        <th style="width: 60px;">SerialNo</th>
                        <th style="width: 40px;">Type</th>
                        <th style="width: 80px;">IsMenuLeaf</th>
                        <th style="width: 10px;">Level</th>
                        <th style="width: 260px;">Name</th>
                        <th style="width: 70px;">Unit1</th>
                        <th style="width: 70px;">FixedCount1</th>
                        <th style="width: 70px;">Unit2</th>
                        <th style="width: 70px;">FixedCount2</th>
                        <th style="width: 70px;">Price</th>
                        <th style="width: 70px;">Currency</th>
                        <th style="width: 260px;">Remark1</th>
                        <th style="width: 260px;">Remark2</th>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</body>
</html>
