const createReducer = (types, initialStateDefaults = {}) => {
  const [requestType, successType, failureType] = types
  const actionHandlers = {
    [requestType]: (state, { payload }) => ({ loading: true, success: false, error: null }),
    [successType]: (state, { payload }) => ({ loading: false, success: true, payload }),
    [failureType]: (state, { payload }) => ({ loading: false, success: false, payload })
  }

  const initialState = {
    loading: true,
    success: false,
    error: null,
    payload: null,
    ...initialStateDefaults
  }

  const reducer = (state = initialState, action) => {
    const handler = actionHandlers[action.type]

    return handler ? handler(state, action) : state
  }

  return reducer
}

export default createReducer
