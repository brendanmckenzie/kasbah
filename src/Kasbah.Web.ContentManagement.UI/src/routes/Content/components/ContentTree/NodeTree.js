import React from 'react'
import Node from './Node'

class NodeTree extends React.Component {
  static propTypes = {
    parent: React.PropTypes.string
  }

  static contextTypes = {
    tree: React.PropTypes.array.isRequired,
    onCreateNode: React.PropTypes.func.isRequired
  }

  constructor() {
    super()

    this.handleCreateNode = this.handleCreateNode.bind(this)
  }

  handleCreateNode() {
    this.context.onCreateNode(this.props.parent)
  }

  render() {
    const { parent } = this.props
    const { tree } = this.context

    return (
      <ul className='content-tree'>
        {tree.filter(ent => ent.parent === parent).map(ent => (
          <Node key={ent.id} node={ent} />
        ))}
        <li>
          <button
            className='button is-primary is-small is-fullwidth is-outlined'
            onClick={this.handleCreateNode}>Create node</button>
        </li>
      </ul>
    )
  }
}

export default NodeTree
