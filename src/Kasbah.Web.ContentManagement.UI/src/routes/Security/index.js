import OverviewRoute from './childRoutes/Overview'
import UsersRoute from './childRoutes/Users'
import RolesRoute from './childRoutes/Roles'

export default (store) => ({
  path: 'security',
  getComponent(nextState, cb) {
    require.ensure([], (require) => {
      cb(null, require('./container').default)
    }, 'securityRoute')
  },
  indexRoute: OverviewRoute(store),
  childRoutes: [
    UsersRoute(store),
    RolesRoute(store)
  ]
})
