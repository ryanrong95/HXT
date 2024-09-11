(function ($) {
    $.fn.extend({
        chargeStd: function (options) {
            var sender = $(this);
            var that = this;

            that.allStds = [];
            that.selectedStdID = null;  // 用于保存选中的 StdID
            that.chargeSum = 0;

            var itemInputWidth = 60;
            if (options.itemInputWidth != undefined) {
                itemInputWidth = options.itemInputWidth;
            }

            var getStdData = function (callback) {
                $.post('/hxtadmin/order/fee/DecChargeStandard/List.aspx?action=DecChargeStandardOneObj', {}, function (res) {
                    var result = JSON.parse(res);
                    return callback(result.obj);
                });
            };

            var getOneFromAllStds = function (stdID) {
                var theStd = null;
                for (var i = 0; i < that.allStds.length; i++) {
                    if (that.allStds[i].ID == stdID) {
                        theStd = that.allStds[i];
                        break;
                    }
                }
                return theStd;
            };

            var getChildrenFromAllStds = function (fatherID) {
                var children = [];
                for (var i = 0; i < that.allStds.length; i++) {
                    if (that.allStds[i].FatherID == fatherID) {
                        children.push(that.allStds[i]);
                    }
                }
                return children;
            };

            var objToHtml = function (stdOneObj) {
                if (stdOneObj.IsMenuLeaf) {
                    if (stdOneObj.Type == 2) {
                        return '<div id="std-leaf-' + stdOneObj.ID + '" leaf-type="2">' + stdOneObj.Name + '</div>';
                    } else if (stdOneObj.Type == 3) {
                        return '<div id="std-leaf-' + stdOneObj.ID + '" leaf-type="3">' + stdOneObj.Name + '</div>';
                    } else {
                        return '<div id="std-leaf-' + stdOneObj.ID + '">' + stdOneObj.Name + '</div>';
                    }
                } else if (stdOneObj.Children != null && stdOneObj.FatherID == null) {
                    // top, as menu's shell
                    var html = '';

                    that.allStds = stdOneObj.All; // save all stds

                    html += '<div class="charge-std-menu" class="easyui-menu">';

                    for (var i = 0; i < stdOneObj.Children.length; i++) {
                        html += objToHtml(stdOneObj.Children[i]);
                    }

                    html += '</div>';

                    return html;
                } else if (stdOneObj.Children != null && stdOneObj.FatherID != null) {
                    // has children
                    var html = '';

                    html += '<div>';
                    html += '<span>' + stdOneObj.Name + '</span>';

                    html += '<div>';
                    for (var i = 0; i < stdOneObj.Children.length; i++) {
                        html += objToHtml(stdOneObj.Children[i]);
                    }
                    html += '</div>';

                    html += '</div>';

                    return html;
                }
            };


            var defaults = {};
            options = $.extend({}, defaults, options);

            var stdInput = sender.find('.charge-std-input');
            stdInput.textbox();
            var stdBtn = sender.find('.charge-std-btn');
            stdBtn.linkbutton();
            var stdRemark = sender.find('.charge-std-remark');
            stdRemark.textbox();
            var stdCalc = sender.find('.charge-std-calc');



            var selectOneStdCommon = function (theStd) {
                $(sender.find('.charge-std-result')[0]).html('');
                that.chargeSum = 0;

                stdInput.textbox('setValue', null);
                stdRemark.textbox('setValue', null);
                that.selectedStdID = null;

                if (theStd != null) {
                    that.selectedStdID = theStd.ID;
                    stdInput.textbox('setValue', theStd.Name);

                    var remark = '';
                    remark += theStd.Name;

                    var remarkFields = ['Remark1', 'Remark2'];
                    for (var i = 0; i < remarkFields.length; i++) {
                        var field = remarkFields[i];
                        if (theStd[field] != null && theStd[field] != '') {
                            if (remark != '') {
                                remark += '\r\n';
                            }
                            remark += theStd[field];
                        }
                    }

                    stdRemark.textbox('setValue', remark);
                }
            };

            var createCalcItem = function (unit, fixedCount) {
                var calcItem = $('<div></div>');

                var input = $('<input class="charge-std-calc-item-input" type="text" />');
                if (fixedCount != null) {
                    input = $('<input class="charge-std-calc-item-input" type="text" data-options="value:' + fixedCount + ',disabled:true," />');
                }

                input.width(itemInputWidth);
                calcItem.append(input);
                calcItem.append($('<label>' + unit + '</label>'));

                return calcItem;
            };

            var createCalcStd = function (theStd) {
                var oneCalcStd = $('<div></div>');
                oneCalcStd.attr('class', 'charge-std-calc-item');
                if (theStd.Type != 2 && theStd.Type != 3) {
                    oneCalcStd.addClass('active');
                }
                oneCalcStd.attr('stdid', theStd.ID);

                if (theStd.Type == 2) {
                    oneCalcStd.append($('<input class="charge-std-calc-item-checkbox" name="std-checkbox" />'));
                } else if (theStd.Type == 3) {
                    oneCalcStd.append($('<input name="std-radio-btn" class="charge-std-calc-item-radiobutton" type="radio" />'));
                }

                if (theStd.Type == 2 || theStd.Type == 3) {
                    oneCalcStd.append($('<label>' + theStd.Name + '</label>'));
                }

                var unitFields = ['Unit1', 'Unit2'];
                var fixedCountFields = ['FixedCount1', 'FixedCount2'];
                for (var i = 0; i < unitFields.length; i++) {
                    var unit = theStd[unitFields[i]];
                    var fixedCount = theStd[fixedCountFields[i]];
                    if (unit != null) {
                        oneCalcStd.append(createCalcItem(unit, fixedCount));
                    }

                }

                oneCalcStd.append($('<label>（单价：' + theStd.Price + ' ）<span style="color: red;">' + theStd.CurrencyCN + '</span></label>'));

                return oneCalcStd;
            };

            var selectOneStdSingle = function (theStd) {
                selectOneStdCommon(theStd);
                stdCalc.empty();

                stdCalc.append(createCalcStd(theStd));
            };

            var selectOneStdMultiple = function (theStd) {
                selectOneStdCommon(theStd);
                stdCalc.empty();

                var children = getChildrenFromAllStds(theStd.ID);
                for (var i = 0; i < children.length; i++) {
                    stdCalc.append(createCalcStd(children[i]));
                }
            };

            var rmAllRadioChecked = function () {
                var targets = sender.find('.charge-std-calc-item-radiobutton').next();
                for (var i = 0; i < targets.length; i++) {
                    var input = $(targets[i]).find('input')[0];
                    var span = $(targets[i]).find('span')[0];

                    $(span).attr('checked', false);
                    $(span).hide();
                    $(input).attr('checked', false);
                }

                var inputs = sender.find('.charge-std-calc-item-radiobutton').parent().find('.charge-std-calc-item-input');
                for (var i = 0; i < inputs.length; i++) {
                    $(inputs[i]).numberbox('setValue', null);
                    $(inputs[i]).numberbox('disable');
                }
            };

            var calcResult = function () {
                var stdCalcItems = sender.find('.charge-std-calc-item');
                var result = 0;

                for (var i = 0; i < stdCalcItems.length; i++) {
                    var stdID = $(stdCalcItems[i]).attr('stdid');
                    var theStd = getOneFromAllStds(stdID);
                    var price = theStd.Price;
                    var values = [];
                    var itemSum = 0;

                    var inputs = $(stdCalcItems[i]).find('.charge-std-calc-item-input');
                    for (var j = 0; j < inputs.length; j++) {
                        var v = $(inputs[j]).numberbox('getValue');
                        if (v == undefined || v == '') {
                            v = 0;
                        }
                        values.push(v);
                    }

                    if (values.length <= 0) {
                        itemSum = 0;
                    } else {
                        itemSum = price;
                        for (var j = 0; j < values.length; j++) {
                            itemSum = itemSum * values[j];
                        }
                    }

                    result += itemSum;
                }

                $(sender.find('.charge-std-result')[0]).html(result);
                that.chargeSum = result;
            };

            getStdData(function (stdOneObj) {
                var menu = '';

                menu += objToHtml(stdOneObj);

                sender.append(menu);

                var stdMenu = sender.find('.charge-std-menu');
                stdMenu.menu({});

                var stdBtnTop = $(stdBtn).offset().top;
                var stdBtnLeft = $(stdBtn).offset().left;
                var stdBtnHeight = $(stdBtn).height();

                stdBtn.click(function () {
                    stdMenu.menu('show', {
                        left: stdBtnLeft,
                        top: stdBtnTop + stdBtnHeight,
                    });
                });

                var checkRightLeaf = function (t1, t2) {
                    if (t1.id != undefined && t1.id != '') {
                        return t1;
                    } else {
                        return t2;
                    }
                }

                for (var i = 0; i < that.allStds.length; i++) {
                    var oneLeaf = $('#std-leaf-' + that.allStds[i].ID);

                    oneLeaf.click(function (e) {
                        var leaf = checkRightLeaf($(e.target)[0], $(e.target).parent()[0]);

                        var theStd = null;
                        for (var i = 0; i < that.allStds.length; i++) {
                            if ('std-leaf-' + that.allStds[i].ID == leaf.id) {
                                theStd = that.allStds[i];
                                break;
                            }
                        }

                        var leafType = $(leaf).attr('leaf-type');
                        if (leafType == 2 || leafType == 3) {
                            selectOneStdMultiple(theStd);
                        } else {
                            selectOneStdSingle(theStd);
                        }

                        var itemInputs = sender.find('.charge-std-calc-item-input');
                        for (var i = 0; i < itemInputs.length; i++) {
                            if (leafType == 2 || leafType == 3) {
                                $(itemInputs[i]).numberbox({
                                    min: 0,
                                    precision: 4,
                                    onChange: function (newValue, oldValue) {
                                        calcResult();
                                    },
                                    disabled: true,
                                });
                            } else {
                                $(itemInputs[i]).numberbox({
                                    min: 0,
                                    precision: 4,
                                    onChange: function (newValue, oldValue) {
                                        calcResult();
                                    },
                                });
                            }
                        }



                        // 点中 radio 的外围的处理
                        var clickedFromOutside = function (e) {
                            var input = $(e.target).find('input')[0];
                            var span = $(e.target).find('span')[0];

                            var inputs = $(e.target).parent().find('.charge-std-calc-item-input');

                            if ($(span).attr('checked') == undefined || $(span).attr('checked') == false) {
                                $(span).attr('checked', 'checked');
                                $(span).show();
                                $(input).attr('checked', true);

                                for (var i = 0; i < inputs.length; i++) {
                                    $(inputs[i]).numberbox('enable');
                                }
                            } else {
                                $(span).attr('checked', false);
                                $(span).hide();
                                $(input).attr('checked', false);

                                for (var i = 0; i < inputs.length; i++) {
                                    $(inputs[i]).numberbox('setValue', null);
                                    $(inputs[i]).numberbox('disable');
                                }
                            }
                        }

                        // 点中 radio 的内部的处理
                        var clickedFromInside = function (e) {
                            e.stopPropagation();

                            var inputs = $(e.target).parent().parent().find('.charge-std-calc-item-input');

                            if ($(e.target).attr('checked') == 'checked') {
                                $(e.target).attr('checked', false);
                                $(e.target).next().attr('checked', false);
                                $(e.target).css('display', 'none');

                                for (var i = 0; i < inputs.length; i++) {
                                    $(inputs[i]).numberbox('setValue', null);
                                    $(inputs[i]).numberbox('disable');
                                }
                            } else {
                                $(e.target).attr('checked', 'checked');
                                $(e.target).next().attr('checked', true);
                                $(e.target).css('display', 'block');

                                for (var i = 0; i < inputs.length; i++) {
                                    $(inputs[i]).numberbox('enable');
                                }
                            }
                        }

                        // 要让 checkbox 互斥，并且选中后激活对应的 charge-std-calc-item
                        var itemCheckBoxs = sender.find('.charge-std-calc-item-checkbox');
                        for (var i = 0; i < itemCheckBoxs.length; i++) {
                            $(itemCheckBoxs[i]).radiobutton();
                        }

                        sender.find('.charge-std-calc-item-checkbox').next().click(function (e) {
                            clickedFromOutside(e);
                        });

                        sender.find('.charge-std-calc-item-checkbox').next().find('span').click(function (e) {
                            clickedFromInside(e);
                        });

                        // 要让 radiobutton 互斥，并且选中后激活对应的 charge-std-calc-item
                        var itemRadioButtons = sender.find('.charge-std-calc-item-radiobutton');
                        for (var i = 0; i < itemRadioButtons.length; i++) {
                            $(itemRadioButtons[i]).radiobutton();
                        }

                        sender.find('.charge-std-calc-item-radiobutton').next().click(function (e) {
                            rmAllRadioChecked();

                            clickedFromOutside(e);
                        });

                        sender.find('.charge-std-calc-item-radiobutton').next().find('span').click(function (e) {
                            rmAllRadioChecked();

                            clickedFromInside(e);
                        });

                    });
                }

            });

            var convertNeedFields = function (std) {
                if (std == null) {
                    return null;
                } else {
                    var resultModel = {};
                    var fields = ['ID', 'FatherID', 'Type', 'IsMenuLeaf', 'Name', 'Unit1', 'Unit2', 'Price', 'Remark1', 'Remark2'];
                    for (var i = 0; i < fields.length; i++) {
                        resultModel[fields[i]] = std[fields[i]];
                    }
                    resultModel['Children'] = null;
                    return resultModel;
                }
            }

            that.getSelectedStd = function () {
                if (that.selectedStdID == null) {
                    return null;
                } else {
                    var std = getOneFromAllStds(that.selectedStdID);
                    std = convertNeedFields(std);

                    var children = getChildrenFromAllStds(std.ID);
                    if (children != null && children.length > 0) {
                        var reals = [];
                        for (var i = 0; i < children.length; i++) {
                            reals.push(convertNeedFields(children[i]));
                        }
                        std['Children'] = reals;
                    }

                    return std;
                }
            };

            that.getChargeSum = function () {
                calcResult();
                return that.chargeSum;
            };

            that.getChargeInputs = function () {
                var inputResults = [];
                var stdCalcItems = sender.find('.charge-std-calc-item');

                for (var i = 0; i < stdCalcItems.length; i++) {
                    var stdID = $(stdCalcItems[i]).attr('stdid');
                    var theStd = getOneFromAllStds(stdID);
                    var price = theStd.Price;
                    var currency = theStd.Currency;
                    var currencyCN = theStd.CurrencyCN;

                    var values = [];

                    var inputs = $(stdCalcItems[i]).find('.charge-std-calc-item-input');
                    for (var j = 0; j < inputs.length; j++) {

                        var options = $(inputs[j]).numberbox('options');

                        var v = $(inputs[j]).numberbox('getValue');
                        if (v == undefined || v == '') {
                            v = 0;
                        }
                        var unit = $(inputs[j]).next().next().html();
                        values.push({
                            Unit: unit,
                            Value: v,
                            disabled: options.disabled,
                        });
                    }

                    var radioChecked = null;
                    var radio = $(stdCalcItems[i]).find('.charge-std-calc-item-checkbox');
                    if (radio.length == 0) {
                        // not has checkbox, default checked
                        radioChecked = true;
                    } else {
                        radioChecked = $(radio[0]).radiobutton('options').checked;
                    }

                    inputResults.push({
                        StdID: stdID,
                        Price: price,
                        Currency: currency,
                        CurrencyCN: currencyCN,
                        Values: values,
                        radioChecked: radioChecked,
                    });
                }

                return inputResults;
            };

            that.getStdCurrency = function () {
                var currency = '';
                var stdCalcItems = sender.find('.charge-std-calc-item');
                if (stdCalcItems.length > 0) {
                    var stdID = $(stdCalcItems[0]).attr('stdid');
                    var theStd = getOneFromAllStds(stdID);
                    currency = theStd.Currency;
                }
                return currency;
            };

            return that;
        },
    });
})(window.jQuery);


//function dataOptionToObj(dataOption) {
//    var options = dataOption.split(',');

//    for (var i = 0; i < length; i++) {

//    }

//    return {};
//}

