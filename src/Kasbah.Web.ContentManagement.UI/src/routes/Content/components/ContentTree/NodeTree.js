import React from 'react'
import PropTypes from 'prop-types'
import Node from './Node'

class NodeTree extends React.Component {
  static propTypes = {
    parent: PropTypes.string,
    parentAlias: PropTypes.string
  }

  static contextTypes = {
    tree: PropTypes.array.isRequired,
    onCreateNode: PropTypes.func.isRequired,
    readOnly: PropTypes.bool
  }

  handleCreateNode = () => this.context.onCreateNode(this.props.parent)

  render() {
    const { parent, parentAlias } = this.props
    const { tree, readOnly } = this.context

    return (
      <ul className='content-tree'>
        {tree.filter(ent => ent.parent === parent).map(ent => (
          <Node key={ent.id} node={ent} />
        ))}
        {!readOnly && (<li>
          <button
            className='button is-small add-node'
            onClick={this.handleCreateNode}>{parentAlias ? (<span>New node under <strong>{parentAlias}</strong></span>) : 'New node'}</button>
        </li>)}
      </ul>
    )
  }
}

export default NodeTree
