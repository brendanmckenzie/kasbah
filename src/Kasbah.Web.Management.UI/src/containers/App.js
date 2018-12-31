import React from 'react'
import { Provider } from 'react-redux'
import { ConnectedRouter } from 'connected-react-router'
import { actions as authActions } from '../store/appReducers/auth'
import { store, history } from '../store/configureStore'
import routes from '../routes'

class App extends React.Component {
  componentDidMount() {
    store.dispatch(authActions.watchRefreshToken())
  }

  render() {
    return (
      <Provider store={store}>
        <ConnectedRouter history={history} children={routes} />
      </Provider>
    )
  }
}

export default App
