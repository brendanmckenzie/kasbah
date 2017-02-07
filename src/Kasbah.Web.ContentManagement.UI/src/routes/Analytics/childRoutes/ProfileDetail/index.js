import { injectReducer } from 'store/reducers'

export default (store) => ({
  path: 'profile/:id',
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'getProfile', reducer: require('./modules/getProfile').reducer })

      cb(null, require('./container').default)
    }, 'profileDetailRoute')
  }
})
