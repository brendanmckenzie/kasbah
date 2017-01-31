import React from 'react'
import Node from './Node'

class NodeTree extends React.Component {
  constructor() {
    super()

    this.handleCreateNode = this.handleCreateNode.bind(this)
  }

  handleCreateNode() {
    this.props.onCreateNode(this.props.parent)
  }

  render() {
    const { parent, tree } = this.props

    return (
      <ul className='content-tree'>
        {tree.filter(ent => ent.parent === parent).map(ent => (
          <Node key={ent.id} node={ent} tree={tree} onCreateNode={this.props.onCreateNode} />
        ))}
        <li>
          <button
            className='button is-primary is-small is-fullwidth'
            onClick={this.handleCreateNode}>Create node</button>
        </li>
      </ul>
    )
  }
}

NodeTree.propTypes = {
  parent: React.PropTypes.string,
  tree: React.PropTypes.array.isRequired,
  onCreateNode: React.PropTypes.func.isRequired
}

export default NodeTree
