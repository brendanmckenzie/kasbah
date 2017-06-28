import auth from './auth'
import ui from './ui'
import content from './content'
import media from './media'

export default {
  auth: auth.reducer,
  ui: ui.reducer,
  content: content.reducer,
  media: media.reducer
}
