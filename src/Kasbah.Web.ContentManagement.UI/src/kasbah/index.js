import { controls } from './editors/Controls'
import { makeApiRequest, API_BASE } from 'store/util'

class Kasbah {
  constructor() {
    this.editors = {}

    for (var key in controls) {
      const control = controls[key]

      this.registerEditor(control.alias, control)
    }

    this.loadModules()
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
    makeApiRequest({
      method: 'GET',
      url: '/system/external-modules/list'
    })
      .then(res => {
        res.forEach(ent => {
          console.log(`Loading external modules: '${ent.name}'`)
          fetch(`${API_BASE}${ent.entryPoint}`)
            .then(ent2 => ent2.text())
            .then(ent2 => {
              eval(ent2)
              // console.log(ent2)
            })

          // const el = document.createElement('script')
          // el.src = `${API_BASE}${ent.entryPoint}`
          // el.async = true
          // document.body.appendChild(el)
        })
      })
  }

  registedExternalModule(module) {
    if (module.controls) {
      for (var key in module.controls) {
        const control = module.controls[key]

        this.registerEditor(control.alias, control)
      }
    }
  }
}

const kasbah = new Kasbah()

export default kasbah
