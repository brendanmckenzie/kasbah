import { connect } from 'react-redux'
import { action as describeTreeRequest } from '../modules/describeTree'
import { action as createNodeRequest } from '../modules/createNode'

import View from '../components/View'

const mapDispatchToProps = {
  describeTreeRequest,
  createNodeRequest
}

const mapStateToProps = (state) => ({
  describeTree: state.describeTree,
  createNode: state.createNode
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
