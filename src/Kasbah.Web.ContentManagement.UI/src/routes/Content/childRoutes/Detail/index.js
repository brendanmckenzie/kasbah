import { injectReducer } from 'store/reducers'

export default (store) => ({
  path: ':id',
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'getDetail', reducer: require('./modules/getDetail').reducer })
      injectReducer(store, { key: 'putDetail', reducer: require('./modules/putDetail').reducer })
      injectReducer(store, { key: 'publish', reducer: require('./modules/publish').reducer })

      cb(null, require('./container').default)
    }, 'contentDetailRoute')
  }
})