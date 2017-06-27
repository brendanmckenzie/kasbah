import { connect } from 'react-redux'
import { action as getDetailRequest } from 'modules/getDetail'
import { action as putDetailRequest } from 'modules/putDetail'
import { action as deleteNodeRequest } from 'modules/deleteNode'
import { action as moveNodeRequest } from 'modules/moveNode'
import { action as describeTreeRequest } from 'modules/describeTree'
import putNode from 'modules/putNode'

import View from '../components/View'

const mapDispatchToProps = {
  getDetailRequest,
  putDetailRequest,
  deleteNodeRequest,
  moveNodeRequest,
  describeTreeRequest,
  putNodeRequest: putNode.action
}

const mapStateToProps = (state, ownProps) => ({
  id: ownProps.params.id,
  getDetail: state.getDetail,
  putDetail: state.putDetail,
  deleteNode: state.deleteNode,
  moveNode: state.moveNode,
  describeTree: state.describeTree,
  [putNode.key]: state[putNode.key]
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
