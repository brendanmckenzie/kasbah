import React from 'react'
import { Link } from 'react-router'

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
        <li><button className='button is-small' onClick={this.handleCreateNode}>Create node</button></li>
      </ul>
    )
  }
}

NodeTree.propTypes = {
  parent: React.PropTypes.string,
  tree: React.PropTypes.array.isRequired,
  onCreateNode: React.PropTypes.func.isRequired
}

const Node = ({ node, tree, onCreateNode }) => (
  <li>
    <Link to={`/content/${node.id}`} activeClassName='is-active'>{node.displayName}</Link>
    <NodeTree parent={node.id} tree={tree} onCreateNode={onCreateNode} />
  </li>
)

Node.propTypes = {
  node: React.PropTypes.object.isRequired,
  tree: React.PropTypes.array.isRequired,
  onCreateNode: React.PropTypes.func.isRequired
}

export default NodeTree