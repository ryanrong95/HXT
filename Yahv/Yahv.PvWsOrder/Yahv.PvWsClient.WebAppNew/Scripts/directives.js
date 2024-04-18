
//el-dialog支持拖拽
function dialogDrag(dialogHeaderEl, dragDom) {
    dialogHeaderEl.style.cursor = 'move';
    // 获取原有属性 ie dom元素.currentStyle 火狐谷歌 window.getComputedStyle(dom元素, null);
    const sty = dragDom.currentStyle || window.getComputedStyle(dragDom, null);

    // 鼠标按下事件
    dialogHeaderEl.onmousedown = (e) => {
        // 鼠标按下，计算当前元素距离可视区的距离 (鼠标点击位置距离可视窗口的距离)
        const disX = e.clientX - dialogHeaderEl.offsetLeft;
        const disY = e.clientY - dialogHeaderEl.offsetTop;

        // 获取到的值带px 正则匹配替换
        let styL, styT;

        // 注意在ie中 第一次获取到的值为组件自带50% 移动之后赋值为px
        if (sty.left.includes('%')) {
            styL = +document.body.clientWidth * (+sty.left.replace(/\%/g, '') / 100);
            styT = +document.body.clientHeight * (+sty.top.replace(/\%/g, '') / 100);
        } else {
            styL = +sty.left.replace(/\px/g, '');
            styT = +sty.top.replace(/\px/g, '');
        };

        // 鼠标拖拽事件
        document.onmousemove = function (e) {
            // 通过事件委托，计算移动的距离 （开始拖拽至结束拖拽的距离）
            const l = e.clientX - disX;
            const t = e.clientY - disY;

            let finallyL = l + styL;
            let finallyT = t + styT;

            // 边界值判定 注意clientWidth scrollWidth区别 要减去之前的top left值
            // dragDom.offsetParent表示弹窗阴影部分
            if (finallyL < 0) {
                finallyL = 0;
            } else if (finallyL > dragDom.offsetParent.clientWidth - dragDom.clientWidth - dragDom.offsetParent.offsetLeft) {
                finallyL = dragDom.offsetParent.clientWidth - dragDom.clientWidth - dragDom.offsetParent.offsetLeft;
            }

            if (finallyT < 0) {
                finallyT = 0;
            } else if (finallyT > dragDom.offsetParent.clientHeight - dragDom.clientHeight - dragDom.offsetParent.offsetLeft) {
                finallyT = dragDom.offsetParent.clientHeight - dragDom.clientHeight - dragDom.offsetParent.offsetLeft;
            }

            // 移动当前元素
            dragDom.style.left = `${finallyL}px`;
            dragDom.style.top = `${finallyT}px`;

            //将此时的位置传出去
            //binding.value({x:e.pageX,y:e.pageY})
        };

        document.onmouseup = function () {
            document.onmousemove = null;
            document.onmouseup = null;
        };
    }

}

//获取相对于浏览器窗口的位置信息
function getAbsolutePosition(obj) {
    //如果函数没有传入值的话返回对象为空的
    if (!obj) return null;
    var w = obj.offsetWidth, h = obj.offsetHeight;
    //从目标元素开始向外遍历，累加top和left值
    var t, l;
    for (t = obj.offsetTop, l = obj.offsetLeft; obj = obj.offsetParent;) {
        t += obj.offsetTop;
        l += obj.offsetLeft;
    }
    var r = document.body.offsetWidth - w - l;
    var b = document.body.offsetHeight - h - t;

    //返回定位元素的坐标集合
    return { width: w, height: h, top: t, left: l, right: r, bottom: b };
}