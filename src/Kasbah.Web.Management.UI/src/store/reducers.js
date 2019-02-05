import { combineReducers } from 'redux'
import { reducer as formReducer } from 'redux-form'
import appReducers from './appReducers'
import { connectRouter } from 'connected-react-router'

const createRootReducer = (history) => {
  return combineReducers({
    form: formReducer,
    router: connectRouter(history),
    ...appReducers,
  })
}

export default createRootReducer
