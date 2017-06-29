import React from 'react'
import { Route, Switch } from 'react-router'
import withRouteOnEnter from 'utils/withRouteOnEnter'
import CoreLayout from 'layouts/CoreLayout'

import HomeRoute from './Home'
import LoginRoute from './Login'
import ContentRoute from './Content'
import MediaRoute from './Media'
import SecurityRoute from './Security'

// import SystemRoute from './System'

const checkAuth = (props) => {
  if (!props.auth.user) {
    props.push('/login')
  }
}

export default (
  <Switch>
    <Route path='/login' component={LoginRoute} />
    <CoreLayout>
      <Route exact path='/' component={withRouteOnEnter(checkAuth)(HomeRoute)} />
      <Route path='/content' component={ContentRoute} />
      <Route path='/media' component={MediaRoute} />
      <Route path='/security' component={SecurityRoute} />
    </CoreLayout>
  </Switch>
)
