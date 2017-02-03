import { connect } from 'react-redux'
import { action as listMediaRequest } from '../modules/listMedia'
import { action as uploadMediaRequest } from '../modules/uploadMedia'
import { action as deleteMediaRequest } from '../modules/deleteMedia'

import View from '../components/View'

const mapDispatchToProps = {
  listMediaRequest,
  uploadMediaRequest,
  deleteMediaRequest
}

const mapStateToProps = (state) => ({
  listMedia: state.listMedia,
  uploadMedia: state.uploadMedia,
  deleteMedia: state.deleteMedia
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
