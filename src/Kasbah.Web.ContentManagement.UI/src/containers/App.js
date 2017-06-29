import React from 'react'
import PropTypes from 'prop-types'
import { Provider } from 'react-redux'
import { ConnectedRouter } from 'react-router-redux'
import { actions as authActions } from 'store/appReducers/auth'

class AppContainer extends React.Component {
  static propTypes = {
    store: PropTypes.object.isRequired,
    routes: PropTypes.any.isRequired
  }

  componentDidMount() {
    this.props.store.dispatch(authActions.watchRefreshToken())
  }

  render() {
    const { store, routes } = this.props

    return (
      <Provider store={store}>
        <ConnectedRouter history={store.history} children={routes} />
      </Provider>
    )
  }
}

export default AppContainer
