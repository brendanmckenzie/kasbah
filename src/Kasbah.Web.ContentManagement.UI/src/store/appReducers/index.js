import auth from './auth'
import contentTypes from './contentTypes'
import ui from './ui'
import content from './content'

export default {
  auth: auth.reducer,
  contentTypes: contentTypes.reducer,
  ui: ui.reducer,
  content: content.reducer
}
