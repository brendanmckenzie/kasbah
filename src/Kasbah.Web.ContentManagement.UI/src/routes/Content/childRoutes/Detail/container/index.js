import { connect } from 'react-redux'
import { action as getDetailRequest } from '../modules/getDetail'
import { action as putDetailRequest } from '../modules/putDetail'
import { action as deleteNodeRequest } from '../modules/deleteNode'

import View from '../components/View'

const mapDispatchToProps = {
  getDetailRequest,
  putDetailRequest,
  deleteNodeRequest
}

const mapStateToProps = (state, ownProps) => ({
  id: ownProps.params.id,
  getDetail: state.getDetail,
  putDetail: state.putDetail,
  deleteNode: state.deleteNode
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
