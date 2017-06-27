import { makeApiRequest } from 'store/util'
import { push } from 'react-router-redux'

export const API_FAILURE = 'API_FAILURE'

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

    const [requestType, successType, failureType] = types

    dispatch({ type: requestType })
    return makeApiRequest(request)
      .then(res => {
        if (res.error) {
          dispatch({ type: failureType, payload: { errorMessage: 'An error has occurred', detail: res.error } })
        } else {
          dispatch({ type: successType, payload: res })
        }
      })
      .catch(ex => {
        dispatch({ type: failureType, payload: { errorMessage: 'An error has occurred', detail: ex } })
        if (ex.response) {
          switch (ex.response.status) {
            case 401: dispatch(push('/login'))
          }
        }
      })
  }
}
