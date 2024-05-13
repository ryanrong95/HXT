// 懒加载组件
function lazy(prentname,name) {
  if (name !== 'layout') {
    return () => import(`@/Pages/${prentname}/${name}.vue`)
  } else {
    return () => import(`@/components/${name}.vue`)
  }
}
export {lazy}

