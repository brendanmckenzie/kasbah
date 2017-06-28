export const SHOW_MODAL = 'SHOW_MODAL'
export const HIDE_MODAL = 'HIDE_MODAL'

export const actions = {
  showModal: (title, control, buttons) => ({
    type: SHOW_MODAL,
    title,
    control,
    buttons
  }),
  hideModal: () => ({
    type: HIDE_MODAL
  })
}

const initialState = {
  modal: null
}

const actionHandlers = {
  [SHOW_MODAL]: (state, { buttons, title, control }) => ({ ...state, modal: { buttons, title, control } }),
  [HIDE_MODAL]: (state, action) => ({ ...state, modal: null })
}

const reducer = (state = initialState, action) => {
  const handler = actionHandlers[action.type]

  return handler ? handler(state, action) : state
}

export default {
  reducer,
  actions
}
