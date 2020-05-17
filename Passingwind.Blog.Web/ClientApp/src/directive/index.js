import directive from './directives'

const importDirective = Vue => {
    Vue.directive('permission', directive.permission)
}

export default importDirective
