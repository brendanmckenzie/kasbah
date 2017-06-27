import React from 'react'
import PropTypes from 'prop-types'
import { Provider } from 'react-redux'
import { ConnectedRouter } from 'react-router-redux'

import routes from 'routes'

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
          {routes}
        </ConnectedRouter>
      </Provider>
    )
  }
}

export default AppContainer
