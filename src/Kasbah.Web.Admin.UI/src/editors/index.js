import { controls } from './Controls'

let editors = {}

export const registerEditor = (alias, control) => {
  editors[alias] = control
}

export const getEditor = (alias) => (alias in editors ? editors[alias] : 'input')

for (var key in controls) {
  const control = controls[key]

  registerEditor(control.alias, control)
}
