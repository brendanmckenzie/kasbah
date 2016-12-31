// import { injectReducer } from '../../store/reducers'
import DetailRoute from './childRoutes/Detail'
export default (store) => ({
  path: 'content',
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      // injectReducer(store, { key: 'counter', reducer: require('./modules/counter').default })
      cb(null, require('./container').default)
    }, 'contentRoute')
  },
  childRoutes: [
    DetailRoute(store)
  ]
})
