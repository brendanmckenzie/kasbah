import { createBrowserHistory } from 'history'
import { applyMiddleware, compose, createStore } from 'redux'
import thunk from 'redux-thunk'
import { routerMiddleware } from 'connected-react-router'
import createRootReducer from './reducers'
import { middleware as executeApiMiddleware } from './middleware/executeApi'

export const history = createBrowserHistory()

export const store = createStore(
  createRootReducer(history), // root reducer with router state
  {},
  compose(
    applyMiddleware(
      routerMiddleware(history), // for dispatching history actions
      thunk,
      executeApiMiddleware
      // ... other middlewares ...
    )
  )
)
