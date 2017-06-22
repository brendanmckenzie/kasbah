import React from 'react'
import PropTypes from 'prop-types'
import Loading from 'components/Loading'
import Error from 'components/Error'
import NodeTree from './NodeTree'
import CreateNode from './CreateNode'

class ContentTree extends React.Component {
  static propTypes = {
    describeTreeRequest: PropTypes.func.isRequired,
    describeTree: PropTypes.object.isRequired,
    createNodeRequest: PropTypes.func,
    createNode: PropTypes.object,
    listTypesRequest: PropTypes.func,
    listTypes: PropTypes.object,
    deleteNode: PropTypes.object,
    updateNodeAlias: PropTypes.object,
    changeType: PropTypes.object,
    moveNode: PropTypes.object,
    readOnly: PropTypes.bool,
    onSelect: PropTypes.func
  }

  static contextTypes = {
    router: PropTypes.object.isRequired
  }

  static childContextTypes = {
    expandedNodes: PropTypes.object,
    tree: PropTypes.array,
    types: PropTypes.array,
    onToggleNode: PropTypes.func,
    onCreateNode: PropTypes.func,
    readOnly: PropTypes.bool,
    onSelect: PropTypes.func
  }

  constructor() {
    super()

    this.state = {
      showCreateNode: false,
      expandedNodes: {}
    }
  }

  componentDidMount() {
    this.handleRefresh()
  }

  componentWillReceiveProps(nextProps) {
    if (!this.props.readOnly) {
      if (nextProps.createNode.success && nextProps.createNode.success !== this.props.createNode.success) {
        this.handleRefresh()
      }
      if (nextProps.deleteNode.success && nextProps.deleteNode.success !== this.props.deleteNode.success) {
        this.handleRefresh()
      }
      if (nextProps.updateNodeAlias.success && nextProps.updateNodeAlias.success !== this.props.updateNodeAlias.success) {
        this.handleRefresh()
      }
      if (nextProps.changeType.success && nextProps.changeType.success !== this.props.changeType.success) {
        this.handleRefresh()
      }
      if (nextProps.moveNode.success && nextProps.moveNode.success !== this.props.moveNode.success) {
        this.handleRefresh()

        if (!this.props.readOnly) {
          const getNode = id => this.props.describeTree.payload.filter(ent => ent.id === id)[0]
          let node = getNode(this.context.router.params.id)
          let expandedNodes = {}
          while (node.parent !== null) {
            expandedNodes[node.parent] = true
            node = getNode(node.parent)
          }
          this.setState({
            expandedNodes
          })
        }
      }
    }
    if (nextProps.describeTree.success && nextProps.describeTree.success !== this.props.describeTree.success) {
      if (this.context.router.params.id) {
        if (!this.props.readOnly) {
          const getNode = id => nextProps.describeTree.payload.filter(ent => ent.id === id)[0]
          let node = getNode(this.context.router.params.id)
          let expandedNodes = {}
          while (node.parent !== null) {
            expandedNodes[node.parent] = true
            node = getNode(node.parent)
          }
          this.setState({
            expandedNodes
          })
        }
      }
    }
  }

  getChildContext() {
    return {
      expandedNodes: this.state.expandedNodes,
      tree: this.props.describeTree.payload || [],
      types: this.props.listTypes ? this.props.listTypes.payload || [] : [],
      onToggleNode: this.handleToggleNode,
      onCreateNode: this.handleCreateNode,
      readOnly: this.props.readOnly,
      onSelect: this.props.onSelect
    }
  }

  handleRefresh = () => this.props.describeTreeRequest()

  handleCreateNode = (parent) => {
    this.setState({
      showCreateNode: true,
      createNodeParent: parent
    })
  }

  handleCancelCreateNode = () => {
    this.setState({
      showCreateNode: false,
      createNodeParent: null
    })
  }

  handleToggleNode = (id) => {
    this.setState({
      expandedNodes: {
        ...this.state.expandedNodes,
        [id]: !this.state.expandedNodes[id]
      }
    })
  }

  render() {
    if (!this.props.describeTree || this.props.describeTree.loading) {
      return <Loading />
    }

    if (!this.props.describeTree.success) {
      return <Error />
    }

    return (
      <div>
        <NodeTree parent={null} onCreateNode={this.handleCreateNode} />

        {!this.props.readOnly && (<CreateNode
          visible={this.state.showCreateNode}
          parent={this.state.createNodeParent}
          onCancel={this.handleCancelCreateNode}
          createNode={this.props.createNode}
          createNodeRequest={this.props.createNodeRequest}
          listTypes={this.props.listTypes}
          listTypesRequest={this.props.listTypesRequest} />)}
      </div>
    )
  }
}

export default ContentTree
