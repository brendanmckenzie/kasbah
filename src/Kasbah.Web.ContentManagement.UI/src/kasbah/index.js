import { controls } from './editors/Controls'

class Kasbah {
  constructor() {
    this.editors = {}

    for (var key in controls) {
      const control = controls[key]

      this.registerEditor(control.alias, control)
    }
  }

  getEditor(alias) {
    if (alias in this.editors) {
      return this.editors[alias]
    }

    return 'input'
  }

  registerEditor(alias, control) {
    this.editors = {
      ...this.editors,
      [alias]: control
    }
  }

  loadModules() {
    // TODO: load external modules
  }
}

const kasbah = new Kasbah()

export default kasbah
