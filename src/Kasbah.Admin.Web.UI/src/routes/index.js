import CoreLayout from '../layouts/CoreLayout'
import HomeRoute from './Home'
import LoginRoute from './Login'
import ContentRoute from './Content'
import MediaRoute from './Media'
import AnalyticsRoute from './Analytics'
import SecurityRoute from './Security'

export const createRoutes = (store) => ({
  childRoutes: [
    LoginRoute(store),
    {
      path: '/',
      component: CoreLayout,
      indexRoute: HomeRoute(store),
      childRoutes: [
        ContentRoute(store),
        MediaRoute(store),
        AnalyticsRoute(store),
        SecurityRoute(store)
      ]
    }
  ]
})

export default createRoutes
