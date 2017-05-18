import { injectReducer } from '../../store/reducers'
import DetailRoute from './childRoutes/Detail'
export default (store) => ({
  path: 'content',
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'describeTree', reducer: require('modules/describeTree').reducer })
      injectReducer(store, { key: 'listTypes', reducer: require('modules/listTypes').reducer })
      injectReducer(store, { key: 'createNode', reducer: require('modules/createNode').reducer })
      injectReducer(store, { key: 'deleteNode', reducer: require('modules/deleteNode').reducer })
      injectReducer(store, { key: 'updateNodeAlias', reducer: require('modules/updateNodeAlias').reducer })
      injectReducer(store, { key: 'moveNode', reducer: require('modules/moveNode').reducer })
      injectReducer(store, { key: 'changeType', reducer: require('modules/changeType').reducer })

      cb(null, require('./container').default)
    }, 'contentRoute')
  },
  childRoutes: [
    DetailRoute(store)
  ]
})
