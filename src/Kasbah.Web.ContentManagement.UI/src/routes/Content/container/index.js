import { connect } from 'react-redux'
import { action as describeTreeRequest } from '../modules/describeTree'
import { action as createNodeRequest } from '../modules/createNode'
import { action as listTypesRequest } from '../modules/listTypes'

import View from '../components/View'

const mapDispatchToProps = {
  describeTreeRequest,
  createNodeRequest,
  listTypesRequest
}

const mapStateToProps = (state) => ({
  describeTree: state.describeTree,
  createNode: state.createNode,
  listTypes: state.listTypes
})

export default connect(mapStateToProps, mapDispatchToProps)(View)