import { injectReducer } from 'store/reducers'

export default (store) => ({
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'listProfiles', reducer: require('./modules/listProfiles').reducer })

      cb(null, require('./container').default)
    }, 'profileListRoute')
  }
})
