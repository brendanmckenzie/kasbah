import auth from './auth'
import ui from './ui'
import content from './content'
import media from './media'
import security from './security'
import system from './system'

export default {
  auth: auth.reducer,
  ui: ui.reducer,
  content: content.reducer,
  media: media.reducer,
  security: security.reducer,
  system: system.reducer
}
