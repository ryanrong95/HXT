Vue.use(Vuex);

var store = new Vuex.Store({
    state: {
        tabArray: [{ text: "首页", url: "/Home/Index" }],
        selectTab: { text: "首页", url: "/Home/Index" },
        selectIndex: -1,
    },
    mutations: {
        //把左侧列表中选中的某一项添加到tab数组中
        addToTabArray(state, item) {


            //改完1
            if (state.tabArray == null) {
                state.tabArray.push(item)
            } else {
                var ret = state.tabArray.find(v => v.text == item.text);
                if (!ret) {
                    state.tabArray.push(item)

                    if (state.tabArray.length > 7) {
                        state.tabArray.splice(1, 1);
                    }
                    window.sessionStorage.setItem("userMsg", JSON.stringify(state.tabArray))
                    state.selectTab = item
                } else {
                    state.selectTab = item
                    var n = state.tabArray.findIndex((v) => v.text === state.selectTab.text);
                    //console.log($(".rb_content>div:nth-child(2)"))
                    //console.log(n);
                    $(".rb_content").children("div").eq(n).children("iframe").attr('src', state.selectTab.url);



                }
            }







        },
        //tab栏切换
        changeTab(state, n) {

            state.selectTab = n;


        },
        //tab栏删除某一项
        deleteTabItem(state, n) {

            var newsrr = state.tabArray

            var newselect = state.selectTab;

            if (n >= 1) {


                if (newsrr[n].text == newselect.text) {
                    newselect = newsrr[n - 1]
                    state.selectTab = newselect;
                } else {
                    newselect = JSON.parse(window.sessionStorage.getItem("selectMsg"))
                }


            } else {
                newselect = newsrr[n + 1]
            }



            window.sessionStorage.setItem("selectMsg", JSON.stringify(newselect))



            if (newsrr.length > 1) {
                newsrr.splice(n, 1);
            }

            window.sessionStorage.setItem("userMsg", JSON.stringify(newsrr))




        },


    },
    actions: {

        addToTabArray: ({ commit }, item) => {

            commit('addToTabArray', item)
        },




        changeTab: ({
            commit
        }, n) => {
            window.sessionStorage.setItem("selectMsg", JSON.stringify(n))


            commit('changeTab', n)
        },




        deleteTabItem: ({
            commit
        }, n) => {





            commit('deleteTabItem', n)
        },

    }
});