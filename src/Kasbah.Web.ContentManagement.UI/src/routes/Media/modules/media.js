const MEDIA_SET_LOADED = 'MEDIA_SET_LOADED'

const actions = {
  setLoaded: (loaded) => ({ type: MEDIA_SET_LOADED, payload: { loaded } })
}

const key = 'media'

const initialState = { loaded: false }

const actionHandlers = {
  [MEDIA_SET_LOADED]: (state, { payload: { loaded } }) => ({ ...state, loaded })
}

const reducer = (state = initialState, action) => {
  const handler = actionHandlers[action.type]

  return handler ? handler(state, action) : state
}

export default {
  key,
  actions,
  reducer
}
