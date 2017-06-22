import UsersRoute from './childRoutes/Users'
import RolesRoute from './childRoutes/Roles'

export default (store) => ({
  path: 'security',
  getComponent(nextState, cb) {
    require.ensure([], (require) => {
      cb(null, require('./container').default)
    }, 'securityRoute')
  },
  indexRoute: UsersRoute(store),
  childRoutes: [
    RolesRoute(store)
  ]
})
