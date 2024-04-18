
//提送货费用

var Charges = {};

(function () {
    var deliveries = {};
    deliveries = {
        "name": "提、送货费",
        "subs": [
            {
                "input": "select",
                "data": [
                    {
                        "name": "葵涌码头"

                    },
                    {
                        "name": "东涌",
                        subs: [
                            {
                                "input": "select",
                                "data": [
                                    {
                                        "name": "小车",
                                        "currency": "Cny",
                                        "price": 550
                                    },
                                    {
                                        "name": "大车",
                                        "currency": "Cny",
                                        "price": 850
                                    }
                                ]

                            }
                        ]
                    },
                    {
                        "name": "机场"

                    }
                ]
            }
        ]
    };
    Charges = {
        "input": "", //text,num,select
        "precision": 2, //只在input=num中有作用
        "data": [
            {
                "name": "入仓费(港币）"
            },
            deliveries,
            {
                "name": "发货服务费(RMB)"
            }
        ]
    };

})();


