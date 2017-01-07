import React from 'react'
import Loading from 'components/Loading'
import Error from 'components/Error'
import NodeTree from './NodeTree'
import CreateNode from './CreateNode'

class ContentTree extends React.Component {
  static propTypes = {
    describeTreeRequest: React.PropTypes.func.isRequired,
    describeTree: React.PropTypes.object.isRequired,
    createNodeRequest: React.PropTypes.func.isRequired,
    createNode: React.PropTypes.object.isRequired,
    listTypesRequest: React.PropTypes.func.isRequired,
    listTypes: React.PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = { showCreateNode: false }

    this.handleCreateNode = this.handleCreateNode.bind(this)
    this.handleCancelCreateNode = this.handleCancelCreateNode.bind(this)
  }

  componentDidMount() {
    this.handleRefresh()
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.createNode.success && nextProps.createNode.success !== this.props.createNode.success) {
      this.handleRefresh()
    }
  }

  handleRefresh() {
    this.props.describeTreeRequest()
  }

  handleCreateNode(parent) {
    this.setState({
      showCreateNode: true,
      createNodeParent: parent
    })
  }

  handleCancelCreateNode() {
    this.setState({
      showCreateNode: false,
      createNodeParent: null
    })
  }

  render() {
    if (this.props.describeTree.loading) {
      return <Loading />
    }

    if (!this.props.describeTree.success) {
      return <Error />
    }

    return (
      <div>
        <NodeTree tree={this.props.describeTree.payload} parent={null} onCreateNode={this.handleCreateNode} />

        <CreateNode
          visible={this.state.showCreateNode}
          parent={this.state.createNodeParent}
          onCancel={this.handleCancelCreateNode}
          createNode={this.props.createNode}
          createNodeRequest={this.props.createNodeRequest}
          listTypes={this.props.listTypes}
          listTypesRequest={this.props.listTypesRequest} />
      </div>
    )
  }
}

export default ContentTree
