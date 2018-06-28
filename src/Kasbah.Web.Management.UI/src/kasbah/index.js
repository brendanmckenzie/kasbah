import { controls } from './editors/Controls'
import { makeApiRequest } from 'store/util'

class Kasbah {
  editors = {}
  routes = []
  store = {}

  constructor() {
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
    return makeApiRequest({
      method: 'GET',
      url: '/system/summary'
    })
      .then(res => {
        console.log('System summary', res)
        res.externalModules.forEach(ent => {
          console.log(`Loading external modules: '${ent.name}'`)

          const el = document.createElement('script')
          el.src = `/${ent.entryPoint}`
          el.async = true
          document.body.appendChild(el)
        })
      })
  }

  registerModule(moduleDefinition) {
    if (moduleDefinition.routes) {
      this.routes = [
        ...this.routes,
        ...moduleDefinition.routes]
    }

    if (moduleDefinition.controls) {
      this.editors = {
        ...this.editors,
        ...moduleDefinition.editors
      }
    }
  }
}

const kasbah = new Kasbah()

window.kasbah = kasbah

export default kasbah
