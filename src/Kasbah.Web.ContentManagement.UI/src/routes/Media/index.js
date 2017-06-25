import { injectReducer } from 'store/reducers'

import { connect } from 'react-redux'
import listMedia from './modules/listMedia'
import uploadMedia from './modules/uploadMedia'
import deleteMedia from './modules/deleteMedia'
import putMedia from './modules/putMedia'
import media from './modules/media'

import View from './components/View'

const mapDispatchToProps = {
  listMediaRequest: listMedia.action,
  uploadMediaRequest: uploadMedia.action,
  deleteMediaRequest: deleteMedia.action,
  putMediaRequest: putMedia.action,
  ...media.actions
}

const mapStateToProps = (state) => ({
  [listMedia.key]: state[listMedia.key],
  [uploadMedia.key]: state[uploadMedia.key],
  [deleteMedia.key]: state[deleteMedia.key],
  [putMedia.key]: state[putMedia.key],
  [media.key]: state[media.key]
})

const Container = connect(mapStateToProps, mapDispatchToProps)(View)

export default (store) => ({
  path: 'media',
  getComponent(nextState, cb) {
    injectReducer(store, listMedia)
    injectReducer(store, uploadMedia)
    injectReducer(store, deleteMedia)
    injectReducer(store, putMedia)
    injectReducer(store, media)

    cb(null, Container)
  }
})
