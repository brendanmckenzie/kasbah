import { connect } from 'react-redux'
import { action as listMediaRequest } from '../modules/listMedia'
import { action as uploadMediaRequest } from '../modules/uploadMedia'

import View from '../components/View'

const mapDispatchToProps = {
  listMediaRequest,
  uploadMediaRequest
}

const mapStateToProps = (state) => ({
  listMedia: state.listMedia,
  uploadMedia: state.uploadMedia
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
