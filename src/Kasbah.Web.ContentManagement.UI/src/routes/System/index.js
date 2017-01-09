import { injectReducer } from '../../store/reducers'

export default (store) => ({
  path: 'system',
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'listInstances', reducer: require('./modules/listInstances').reducer })

      cb(null, require('./container').default)
    }, 'systemRoute')
  }
})
