// import 'whatwg-fetch'
import { makeApiRequest } from 'store/util'

export const middleware = ({ dispatch, getState }) => {
  return next => action => {
    const {
      request,
      types
    } = action

    if (!request) {
      // Normal action: pass it on
      return next(action)
    }

    if (!Array.isArray(types) || types.length !== 3 || !types.every(type => typeof type === 'string')) {
      throw new Error('Expected an array of three string types.')
    }

    const [ requestType, successType, failureType ] = types

    dispatch({ type: requestType })
    return makeApiRequest(request)
      .then(res => {
        dispatch({ type: successType, payload: res })
      })
      .catch(ex => dispatch({ type: failureType, payload: { errorMessage: 'An error has occurred', detail: ex } }))
  }
}