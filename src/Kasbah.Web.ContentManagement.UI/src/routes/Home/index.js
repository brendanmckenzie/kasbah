import { injectReducer } from 'store/reducers'

export default (store) => ({
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'listLatestUpdates', reducer: require('./modules/listLatestUpdates').reducer })

      cb(null, require('./container').default)
    }, 'homeRoute')
  }
})
