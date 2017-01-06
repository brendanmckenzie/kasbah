import { injectReducer } from '../../store/reducers'

export default (store) => ({
  path: 'media',
  getComponent (nextState, cb) {
    require.ensure([], (require) => {
      injectReducer(store, { key: 'listMedia', reducer: require('./modules/listMedia').reducer })
      injectReducer(store, { key: 'uploadMedia', reducer: require('./modules/uploadMedia').reducer })

      cb(null, require('./container').default)
    }, 'mediaRoute')
  }
})
