import { injectReducer } from 'store/reducers'

export default (store) => ({
  path: ':id',
  getComponent(nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'getDetail', reducer: require('modules/getDetail').reducer })
      injectReducer(store, { key: 'putDetail', reducer: require('modules/putDetail').reducer })
      injectReducer(store, { key: 'deleteNode', reducer: require('modules/deleteNode').reducer })
      injectReducer(store, { key: 'updateNodeAlias', reducer: require('modules/updateNodeAlias').reducer })
      injectReducer(store, { key: 'changeType', reducer: require('modules/changeType').reducer })
      injectReducer(store, { key: 'moveNode', reducer: require('modules/moveNode').reducer })
      injectReducer(store, { key: 'describeTree', reducer: require('modules/describeTree').reducer })
      injectReducer(store, require('modules/putNode').default)

      cb(null, require('./container').default)
    }, 'contentDetailRoute')
  }
})
