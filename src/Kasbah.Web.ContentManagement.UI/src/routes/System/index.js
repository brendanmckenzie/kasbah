import SitesRoute from './childRoutes/Sites'

export default (store) => ({
  path: 'system',
  getComponent(nextState, cb) {
    cb(null, require('./container').default)
  },
  indexRoute: SitesRoute(store),
  childRoutes: []
})
