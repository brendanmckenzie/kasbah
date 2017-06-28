import auth from './auth'
import ui from './ui'
import content from './content'

export default {
  auth: auth.reducer,
  ui: ui.reducer,
  content: content.reducer
}
