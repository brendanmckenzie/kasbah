import { connect } from 'react-redux'
import { action as getDetailRequest } from 'modules/getDetail'
import { action as putDetailRequest } from 'modules/putDetail'
import { action as deleteNodeRequest } from 'modules/deleteNode'
import { action as updateNodeAliasRequest } from 'modules/updateNodeAlias'
import { action as changeTypeRequest } from 'modules/changeType'

import View from '../components/View'

const mapDispatchToProps = {
  getDetailRequest,
  putDetailRequest,
  deleteNodeRequest,
  updateNodeAliasRequest,
  changeTypeRequest
}

const mapStateToProps = (state, ownProps) => ({
  id: ownProps.params.id,
  getDetail: state.getDetail,
  putDetail: state.putDetail,
  deleteNode: state.deleteNode,
  updateNodeAlias: state.updateNodeAlias,
  changeType: state.changeType
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
