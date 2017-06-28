import React from 'react'
import { Provider, connect } from 'react-redux'
import { ConnectedRouter, push } from 'react-router-redux'
import PropTypes from 'prop-types'
import { Route, Switch } from 'react-router'

import CoreLayout from 'layouts/CoreLayout'

import HomeRoute from './Home'
import LoginRoute from './Login'
import ContentRoute from './Content'
import MediaRoute from './Media'

import SecurityRoute from './Security'
import SystemRoute from './System'

const withRouteOnEnter = callback => BaseComponent => {
  const routeOnEnterCallback = (props) => {
    if (callback && typeof callback === 'function') {
      callback(props)
    }
  }

  class RouteOnEnterComponent extends React.Component {
    static propTypes = {
      location: PropTypes.object.isRequired
    }

    componentWillMount() {
      routeOnEnterCallback(this.props)
    }

    componentWillReceiveProps(nextProps) {
      // not 100% sure about using `location.key` to distinguish between routes
      if (nextProps.location.key !== this.props.location.key) {
        routeOnEnterCallback(nextProps)
      }
    }

    render() {
      return <BaseComponent {...this.props} />
    }
  }

  const mapStateToProps = (state) => ({
    auth: state.auth
  })

  const mapDispatchToProps = {
    push
  }

  return connect(mapStateToProps, mapDispatchToProps)(RouteOnEnterComponent)
}

const checkAuth = (props) => {
  // TODO: check refresh token

  if (!props.auth.user) {
    props.push('/login')
  }
}

class AppContainer extends React.Component {
  static propTypes = {
    store: PropTypes.object.isRequired
  }

  shouldComponentUpdate() {
    return false
  }

  render() {
    const { store } = this.props

    return (
      <Provider store={store}>
        <ConnectedRouter history={store.history}>
          <Switch>
            <Route path='/login' component={LoginRoute} />
            <CoreLayout>
              <Route exact path='/' component={withRouteOnEnter(checkAuth)(HomeRoute)} />
              <Route path='/content' component={ContentRoute} />
              <Route path='/media' component={MediaRoute} />
            </CoreLayout>
          </Switch>
        </ConnectedRouter>
      </Provider>
    )
  }
}

export default AppContainer
