import { injectReducer } from 'store/reducers'

export default (store) => ({
  path: 'login',
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'login', reducer: require('./modules/login').reducer })

      cb(null, require('./container').default)
    }, 'loginRoute')
  }
})
