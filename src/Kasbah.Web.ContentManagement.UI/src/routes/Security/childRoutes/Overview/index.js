// import { injectReducer } from '../../store/reducers'

export default (store) => ({
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      // injectReducer(store, { key: 'counter', reducer: require('./modules/counter').default })
      cb(null, require('./container').default)
    }, 'securityOverviewRoute')
  }
})
