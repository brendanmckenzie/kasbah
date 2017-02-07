import ProfileListRoute from './childRoutes/ProfileList'
import ProfileDetailRoute from './childRoutes/ProfileDetail'

export default (store) => ({
  path: 'analytics',
  getComponent(nextState, cb) {
    require.ensure([], (require) => {
      cb(null, require('./container').default)
    }, 'analyticsRoute')
  },
  indexRoute: ProfileListRoute(store),
  childRoutes: [
    ProfileDetailRoute(store)
  ]
})
