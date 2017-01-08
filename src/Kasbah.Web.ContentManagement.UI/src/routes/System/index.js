// import { injectReducer } from '../../store/reducers'

export default (store) => ({
  path: 'system',
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      // injectReducer(store, { key: 'counter', reducer: require('./modules/counter').default })
      cb(null, require('./container').default)
    }, 'systemRoute')
  }
})
