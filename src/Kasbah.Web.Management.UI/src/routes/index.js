import React from 'react'
import { Route, Switch } from 'react-router'
import withRouteOnEnter from 'utils/withRouteOnEnter'
import CoreLayout from 'layouts/index'

import HomeRoute from './Home'
import LoginRoute from './Login'
import ContentRoute from './Content'
import MediaRoute from './Media'
import SecurityRoute from './Security'
import SystemRoute from './System'
import AnalyticsRoute from './Analytics'

import kasbah from '../kasbah'

const checkAuth = (props) => {
  if (!props.auth.user) {
    props.push('/login')
  }
}

const checkAuthOnEnter = withRouteOnEnter(checkAuth)

export default (
  <Switch>
    <Route path="/login" component={LoginRoute} />
    <CoreLayout>
      <Route exact path="/" component={checkAuthOnEnter(HomeRoute)} />
      <Route path="/content" component={checkAuthOnEnter(ContentRoute)} />
      <Route path="/media" component={checkAuthOnEnter(MediaRoute)} />
      <Route path="/security" component={checkAuthOnEnter(SecurityRoute)} />
      <Route path="/system" component={checkAuthOnEnter(SystemRoute)} />
      <Route path="/analytics" component={checkAuthOnEnter(AnalyticsRoute)} />
      {kasbah.routes.map((ent) => (
        <Route key={ent.alias} path={ent.path} component={checkAuthOnEnter(ent.component)} />
      ))}
    </CoreLayout>
  </Switch>
)
