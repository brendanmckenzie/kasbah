import { injectReducer } from 'store/reducers'

export default (store) => ({
  path: 'users',
  getComponent(nextState, cb) {
    injectReducer(store, require('modules/listUsers').default)

    cb(null, require('./container').default)
  }
})
